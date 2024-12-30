using System.Collections.Generic;
using UnityEngine;

public class JamFood : JamGridActor
{
    [SerializeField]
    protected bool autoconnect = false;
    public int priority = 5;
    [SerializeField]
    public int stepsUntilActive = 0;
    [SerializeField]
    CountdownSprite countdownSprite;
    GameObject toSpawn;
    
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
        if (stepsUntilActive > 0) stepsUntilActive --;
    }

    public override void PostEvaluate() {
        List<JamGridEntity> entities = gridData.Grid.GetCellEntities(gridData.Column, gridData.Row);
        if (toSpawn == null && stepsUntilActive <= 0) {
            foreach (JamGridEntity entity in entities) {
                if (entity.GetActor().IsOfType(ActorTypes.Creature)) {
                    if (entity.GetActor().IsOfType(ActorTypes.Red)) {
                        toSpawn = ActorPrefabs.RedCreature;
                    } else if (entity.GetActor().IsOfType(ActorTypes.Cyan)) {
                        toSpawn = ActorPrefabs.CyanCreature;
                    }
                }
            }
        } else if (toSpawn != null) {
            bool noCreaturesOnCell = true;
            foreach (JamGridEntity entity in entities) {
                if (entity.GetActor().IsOfType(ActorTypes.Creature)) {
                    noCreaturesOnCell = false;
                    break;
                }
            }
            if (noCreaturesOnCell) {
                gridData.Grid.SpawnActorAtCell(gridData.Column, gridData.Row, toSpawn);
                Destroy(gameObject);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        // snap to target position to be sure
        SnapToGrid();

        // Update countdown timer
        if (countdownSprite != null) {
            countdownSprite.count = stepsUntilActive;
            if (stepsUntilActive > 0) {
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
}