using System.Collections.Generic;
using UnityEngine;

public class Conveyor : JamGridActor
{
    [SerializeField]
    protected bool autoconnect = false;
    public int priority = 4;
    [SerializeField]
    public int usesRemaining = 5;
    [SerializeField]
    SpriteRenderer conveyorSprite;
    [SerializeField]
    CountdownSprite countdownSprite;
    [SerializeField]
    Vector2Int pushDir = new Vector2Int(1, 0);
    
    protected void Awake()
    {
        gridData = new JamGridEntity(initColumn, initRow, this);
        if (autoconnect) {
            gridData.ConnectToGrid(initGrid);
        }
    }
    
    protected void Start()
    {
        Vector3 zAligned = transform.position;
        zAligned.z = priority;
        transform.position = zAligned;
    }

    // JamGridActor Implementation
    public override bool IsOfType(string type)
    {
        return type.Equals(ActorTypes.Food);
    }

    public override JamGridEntity GetGridEntity() {
        return gridData;
    }

    public override int GetPriority() {
        return priority;
    }
    public override void PreEvaluate() {
        
    }

    public override void Step() {
        List<JamGridEntity> entities = gridData.Grid.GetCellEntities(gridData.Column, gridData.Row);
        foreach (JamGridEntity entity in entities) {
            if (entity != gridData) {
                entity.MoveRelative(pushDir.x, pushDir.y);
                usesRemaining--;
                if (entity.GetActor().IsOfType(ActorTypes.Creature)) {
                    JamCreature creature = entity.GetActor().GetComponent<JamCreature>();
                    creature.RecomputeMove(2);
                }
            }
        }
    }

    public override void PostEvaluate() {
        if (usesRemaining <= 0) Destroy (gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        // snap to target position to be sure
        SnapToGrid();

        RotateSprite();

        // Update countdown timer
        if (countdownSprite != null) {
            countdownSprite.count = usesRemaining;
            if (usesRemaining > 0) {
                countdownSprite.visible = true;
            } else {
                countdownSprite.visible = false;
            }
        }
    }

    // snap to the current position
    protected void SnapToGrid()
    {
        Vector2 gridPos = gridData.GetXY();
        transform.position = new Vector3(gridPos.x, gridPos.y, transform.position.z);
    }

    // snap to the current position
    protected void RotateSprite()
    {
        if (pushDir == new Vector2Int(1, 0)) {
            conveyorSprite.transform.eulerAngles = new Vector3(0,0,90);
        } else if (pushDir == new Vector2Int(0, 1)) {
            conveyorSprite.transform.eulerAngles = new Vector3(0,0,0);
        } else if (pushDir == new Vector2Int(-1, 0)) {
            conveyorSprite.transform.eulerAngles = new Vector3(0,0,270);
        } else if (pushDir == new Vector2Int(0, -1)) {
            conveyorSprite.transform.eulerAngles = new Vector3(0,0,180);
        }
    }
}
