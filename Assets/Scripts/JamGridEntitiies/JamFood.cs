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
        if (stepsUntilActive > 0) stepsUntilActive --;
    }

    public override void PostEvaluate() {
        List<JamGridEntity> entities = gridData.Grid.GetCellEntities(gridData.Column, gridData.Row);
        if (toSpawn == null && stepsUntilActive <= 0) {
            foreach (JamGridEntity entity in entities) {
                if (entity.GetActor().IsOfType(ActorTags.Creature)) {
                    if (entity.GetActor().IsOfType(ActorTags.Red)) {
                        toSpawn = ActorPrefabs.RedCreature;
                    } else if (entity.GetActor().IsOfType(ActorTags.Cyan)) {
                        toSpawn = ActorPrefabs.CyanCreature;
                    } else if (entity.GetActor().IsOfType(ActorTags.Blue)) {
                        toSpawn = ActorPrefabs.BlueCreature;
                    } else if (entity.GetActor().IsOfType(ActorTags.Yellow)) {
                        toSpawn = ActorPrefabs.YellowCreature;
                    } else if (entity.GetActor().IsOfType(ActorTags.Green)) {
                        toSpawn = ActorPrefabs.GreenCreature;
                    } else if (entity.GetActor().IsOfType(ActorTags.Pink)) {
                        toSpawn = ActorPrefabs.PinkCreature;
                    }
                }
            }
        } else if (toSpawn != null) {
            bool noCreaturesOnCell = true;
            foreach (JamGridEntity entity in entities) {
                if (entity.GetActor().IsOfType(ActorTags.Creature)) {
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
    protected override void Update()
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
}
