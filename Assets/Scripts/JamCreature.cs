using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using Random = UnityEngine.Random;

public class JamCreature : JamGridActor
{
    [SerializeField]
    protected bool autoconnect = false;
    public Vector2Int move;
    public int priority = 0;
    protected float _scaledTime;
    
    protected void Awake()
    {
        gridData = new JamGridEntity(initColumn, initRow, this);
        if (autoconnect) {
            gridData.ConnectToGrid(initGrid);
        }
    }

    protected void Start()
    {
        SnapToGrid();
    }

    protected void Update()
    {
        _scaledTime = Time.deltaTime * JamCoordinator.Instance.TimeScale;
    }

    public override bool IsOfType(string type)
    {
        return type.Equals(ActorTags.Creature);
    }

    IEnumerator walkAnim;
    protected void StartWalkAnimation (Vector2 destination) {
        StopWalkAnimation();
        walkAnim = WalkAnimation(destination);
        StartCoroutine(walkAnim);
    }
    protected void StopWalkAnimation () {
        if (walkAnim != null) StopCoroutine(walkAnim);
        SnapToGrid();
    }

    IEnumerator WalkAnimation(Vector2 destination)
    {
        // get target position
        var dest = new Vector3(destination.x, destination.y, transform.position.z);

        // move towards the destination until it is reached
        while (transform.position != dest)
        {
            if (JamCoordinator.Instance.TimeScale > 0) transform.position = Vector3.MoveTowards(transform.position, dest, _scaledTime / JamCoordinator.Instance.StepDuration);
            yield return null;
        }

        // snap to target position to be sure
        SnapToGrid();
    }

    // dies after a time delay
    IEnumerator FadeToDeath()
    {
        // get the sprite renderer
        var renderer = transform.GetComponentInChildren<SpriteRenderer>();
        Color currentColor = renderer.color;
        float fadeAmount = renderer.color.a / JamCoordinator.Instance.StepDuration;

        while (renderer.color.a > 0)
        {
            currentColor.a -= _scaledTime * fadeAmount;
            renderer.color = currentColor;

            yield return null;
        }

        Destroy(gameObject);
    }

    protected void ObliteratedByOtherCreature() {
        StartCoroutine(FadeToDeath());
    }

    // snap to the current position
    protected void SnapToGrid()
    {
        Vector2 gridPos = gridData.GetXY();
        transform.position = new Vector3(gridPos.x, gridPos.y, transform.position.z);
    }

    // JamGridActor Implementation
    public override JamGridEntity GetGridEntity() {
        return gridData;
    }

    public override int GetPriority() {
        return priority;
    }

    // is the tile in the specified direction free?
    protected bool IsTileFree(Vector2Int direction)
    {
        // look at the tile we're about to enter, return true if there's nothing there
        var entities = gridData.Grid.GetCellEntities(gridData.Column + direction.x, gridData.Row + direction.y);
        return entities.Count == 0;
    }

    // does the specified tile have an entity with a specific tag?
    protected bool DoesTileContain(Vector2Int direction, string tag)
    {
        var entities = gridData.Grid.GetCellEntities(gridData.Column + direction.x, gridData.Row + direction.y);
        return entities.Any(i => i.GetActor().IsOfType(tag));
    }

    // attempt to move onto a free orthagonal tile at random
    protected Vector2Int ChooseTileRandomly()
    {
        // first, get free tiles available
        List<Vector2Int> options = new List<Vector2Int>();
        foreach (var direction in GridDirections.Orthogonal)
        {
            if (IsTileFree(direction)) { options.Add(direction); }
        }

        // if all are occupied, return nothing
        if (options.Count == 0) return Vector2Int.zero;

        // pick one at random
        return options[(int)(options.Count * Random.value)];
    }

    protected delegate void StepIntent();
    public override void PreEvaluate() {
        StopWalkAnimation();

        // pick a random direction to move
        Vector2Int dir = ChooseTileRandomly();
        if (dir != Vector2Int.zero)
        {
            // schedule movement
            
        }

        /*
        if (DoesTileContain(dir, ActorTags.Wall))
        {
            Debug.Log("There's a wall here!");
            // try to turn around
            move *= -1;
        }
        */
    }

    public override void Step() {
        StartWalkAnimation(gridData.GetRelativeXY(move.x, move.y));
        gridData.MoveRelative(move.x, move.y);
        // Debug.Log(transform.name + " is at " + gridData.GetColumn() + ", " + gridData.GetRow());
    }

    public override void PostEvaluate() {

    }
}

public class GridDirections
{
    public static readonly Vector2Int North = new Vector2Int(0, -1);
    public static readonly Vector2Int East = new Vector2Int(1, 0);
    public static readonly Vector2Int South = new Vector2Int(0, 1);
    public static readonly Vector2Int West = new Vector2Int(-1, 0);
    public static readonly Vector2Int[] Orthogonal = new Vector2Int[] { North, East, South, West };
}
