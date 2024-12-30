using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Mono.Cecil;
using Unity.Collections;
using UnityEditor;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI.Table;

public class JamGrid : MonoBehaviour
{
    Dictionary<int, JamGridEntity> _entities;
    Dictionary<int, JamGridEntity> Entities { get { if (_entities is null) _entities = new Dictionary<int, JamGridEntity>(); return _entities; } }
    [SerializeField]
    int _width, _height = 4;
    public int Width { get => _width; }
    public int Height { get => _height; }
    public float CellWidth { get => (bottomRight.transform.position.x - topLeft.transform.position.x) / _width; }
    public float CellHeight { get => (bottomRight.transform.position.y - topLeft.transform.position.y) / _height; }
    public float WorldWidth { get => bottomRight.transform.position.x - topLeft.transform.position.x; }
    public float WorldHeight { get => bottomRight.transform.position.y - topLeft.transform.position.y; }

    [SerializeField]
    Transform topLeft;
    [SerializeField]
    Transform bottomRight;
    [SerializeField]
    SpriteRenderer gridTile;

    void Awake() {
        Debug.Assert(_width != 0 && _height != 0, "0 Height or Width");
        Debug.Assert(topLeft is not null && bottomRight is not null, "Grid missing corners");
        Debug.Assert(gridTile is not null, "Missing tile sprite renderer");
    }

    void Update() {
        gridTile.drawMode = SpriteDrawMode.Tiled;
        gridTile.size = new Vector2(_width, _height);
        gridTile.transform.localScale = new Vector3(CellWidth, CellHeight, 1);
        gridTile.transform.position = (bottomRight.transform.position + topLeft.transform.position) / 2;
    }

    public bool Connect(JamGridEntity entity) {
        if (!Entities.ContainsKey(entity.ID)) {
            Entities.Add(entity.ID, entity);
            return true;
        }
        return false;
    }

    public bool Disconnect(JamGridEntity entity)
    {
        if (Entities.ContainsKey(entity.ID)) {
            Entities.Remove(entity.ID);
            return true;
        }
        return false;
    }

    private void SortEntitiesByPriority(List<JamGridEntity> list) {
        list.Sort(
            (JamGridEntity a, JamGridEntity b) => {
                if (a.GetActor() is null && b.GetActor() is null) {
                    return 0;
                } else if (a.GetActor() is null) {
                    return -1;
                } else if (b.GetActor() is null) {
                    return 1;
                } else {
                    return a.GetActor().GetPriority() - b.GetActor().GetPriority();
                }
            }
        );
    }
    public List<JamGridEntity> GetCellEntities(int col, int row)
    {
        col = (col % Width + Width) % Width;
        row = (row % Height + Height) % Height;

        if (col < _width && col >= 0 && row < _height && row >= 0) {
            List<JamGridEntity> list = new List<JamGridEntity>();
            foreach (JamGridEntity entity in Entities.Values.ToList()) {
                if (entity.Column == col && entity.Row == row) {
                    list.Add(entity);
                }
            }
            SortEntitiesByPriority(list);
            return list;
        }
        return new List<JamGridEntity>();
    }

    public List<JamGridEntity> GetAllEntities()
    {
        List<JamGridEntity> list = Entities.Values.ToList();
        SortEntitiesByPriority(list);
        return list;
    }

    public JamGridEntity GetEntityFromID(int ID)
    {
        return Entities[ID];
    }

    public Vector2 GetCellXY(int col, int row)
    {
        float x = topLeft.position.x + (bottomRight.position.x - topLeft.position.x) * ((float) (col + 0.5) / _width);
        float y = topLeft.position.y + (bottomRight.position.y - topLeft.position.y) * ((float) (row + 0.5) / _height);
        return new Vector2(x, y);
    }

    public bool CellExists(int col, int row)
    {
        return col >= 0 && col < Width && row >= 0 && row < Height;
    }

    public JamGridActor SpawnActorAtCell(int col, int row, GameObject prefab) {
        Debug.Log("Summoning");
        GameObject newObject = Instantiate(prefab);
        newObject.transform.parent = transform.parent;
        JamGridActor actor = newObject.GetComponent<JamGridActor>();
        if (actor == null) return null;
        actor.gridData.Move(col, row);
        actor.gridData.ConnectToGrid(this);
        return actor;
    }
}


// ===================================================================================================================================
[Serializable]
public class JamGridEntity
{
    static int _usedIDs = 1; // ensures all IDs are unique

    [SerializeField, ReadOnly]
    public readonly int ID;
    public int Column { get; private set; }
    public int Row { get; private set; }
    public Transform Transform { get; private set; }
    public JamGrid Grid { get => _grid; }

    private bool isOnGrid = false;
    private JamGrid _grid;
    private JamGridActor _actor;

    // constructor (also creates an accompanying gameobject and sprite renderer)
    public JamGridEntity(int col, int row, JamGridActor actor)
    {
        // assign new ID to self
        ID = _usedIDs++;

        // move into position
        Move(col, row);

        // Assign actor
        _actor  = actor;
    }

    // move this in gridspace
    public void Move(int col, int row)
    {
        Column = col;
        Row = row;
    }

    // move this in gridspace
    public void MoveRelative(int colDelta, int rowDelta)
    {
        if (isOnGrid) {
            Column = ((Column + colDelta) % _grid.Width + _grid.Width) % _grid.Width;
            Row = ((Row + rowDelta) % _grid.Height + _grid.Height) % _grid.Height;
        } else {
            Column += colDelta;
            Row += rowDelta;
        }
    }

    public int GetColumn() { return Column; }
    public int GetRow() { return Row; }
    public JamGridActor GetActor() { return _actor; }

    public bool ConnectToGrid(JamGrid targetGrid) {
        DisconnectFromGrid();
        if (targetGrid) {
            if (targetGrid.Connect(this)) {
                _grid = targetGrid;
                isOnGrid = true;
                return true;
            }
        }
        return false;
    }
    public bool DisconnectFromGrid() {
        if (isOnGrid) {
            if (_grid) {
                _grid.Disconnect(this);
                _grid = null;
            }
        }
        isOnGrid = false;
        return true;
    }

    public Vector2 GetXY() {
        return GetXY(Column, Row);
    }

    public Vector2 GetXY(int col, int row) {
        if (isOnGrid) {
            return _grid.GetCellXY(col, row);
        } else {
            return Vector2.zero;
        }
    }

    public Vector2 GetRelativeXY(int col, int row) {
        return GetXY(Column + col, Row + row);
    }

    public float GetGridWorldWidth() {
        return isOnGrid ? _grid.WorldWidth : 0;
    }

    public float GetGridWorldHeight() {
        return isOnGrid ? _grid.WorldHeight : 0;
    }
}

public abstract class JamGridActor : MonoBehaviour
{
    [SerializeField]
    public JamGridEntity gridData;
    [SerializeField]
    protected int initColumn = 0;
    [SerializeField]
    protected int initRow = 0;
    [SerializeField]
    protected JamGrid initGrid;

    protected float _scaledTime;

    public abstract JamGridEntity GetGridEntity();
    public abstract int GetPriority();
    public abstract bool IsOfType(string type);
    public abstract void PreEvaluate();
    public abstract void Step();
    public abstract void PostEvaluate();

    protected virtual void Start()
    {
        SnapToGrid();
    }

    protected void Update()
    {
        _scaledTime = Time.deltaTime * JamCoordinator.Instance.TimeScale;
    }


    // snap to the current position
    protected void SnapToGrid()
    {
        Vector2 gridPos = gridData.GetXY();
        transform.position = new Vector3(gridPos.x, gridPos.y, transform.position.z);
    }

    // auto disconnect from grid on destruction
    protected void OnDestroy()
    {
        gridData.DisconnectFromGrid();
    }
}

public class ActorTags
{
    public static string Creature { get { return "Creature"; } }
    public static string Solid { get { return "Solid"; } }
    public static string Food { get { return "Food"; } }
    public static string Cyan { get { return "Cyan"; } }
    public static string Red { get { return "Red"; } }
    public static string Yellow { get { return "Yellow"; } }
    public static string Blue { get { return "Blue"; } }
    public static string Green { get { return "Green"; } }
    public static string Pink { get { return "Pink"; } }
}

public class ActorPrefabs
{
    private static GameObject GetPrefab(String path, ref GameObject loadedObject) {
        if (loadedObject == null) loadedObject = Resources.Load<GameObject>(path);
        return loadedObject;
    }
    static GameObject redCreature;
    public static GameObject RedCreature => GetPrefab("Prefabs/Red Creature", ref redCreature);
    static GameObject cyanCreature;
    public static GameObject CyanCreature => GetPrefab("Prefabs/Cyan Creature", ref cyanCreature);
    static GameObject blueCreature;
    public static GameObject BlueCreature => GetPrefab("Prefabs/Blue Creature", ref blueCreature);
    static GameObject yellowCreature;
    public static GameObject YellowCreature => GetPrefab("Prefabs/Yellow Creature", ref yellowCreature);
    static GameObject greenCreature;
    public static GameObject GreenCreature => GetPrefab("Prefabs/Green Creature", ref greenCreature);
    static GameObject pinkCreature;
    public static GameObject PinkCreature => GetPrefab("Prefabs/Pink Creature", ref pinkCreature);
}