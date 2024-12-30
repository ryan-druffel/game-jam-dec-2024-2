using System.Collections.Generic;
using System.Linq;
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
    
    protected override void Start()
    {
        base.Start();
        Vector3 zAligned = transform.position;
        zAligned.z = priority;
        transform.position = zAligned;
    }

    // JamGridActor Implementation
    public override bool IsOfType(string type)
    {
        return type.Equals(ActorTags.Food);
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
                if (!IsTileSolid(pushDir)) {
                    entity.MoveRelative(pushDir.x, pushDir.y);
                    if (entity.GetActor().IsOfType(ActorTags.Creature)) {
                        JamCreature creature = entity.GetActor().GetComponent<JamCreature>();
                        creature.RecomputeMove(2);
                    }
                }
                usesRemaining--;
            }
        }
    }

    public override void PostEvaluate() {
        if (usesRemaining <= 0) Destroy (gameObject);
    }

    // Update is called once per frame
    protected override void Update()
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

    // is the tile in the specified direction not solid?
    protected bool IsTileSolid(Vector2Int direction)
    {
        // look at the tile we're about to enter, return true if there's nothing there
        var entities = gridData.Grid.GetCellEntities(gridData.Column + direction.x, gridData.Row + direction.y);
        return entities.Any(i => i.GetActor().IsOfType(ActorTags.Solid));
    }
}
