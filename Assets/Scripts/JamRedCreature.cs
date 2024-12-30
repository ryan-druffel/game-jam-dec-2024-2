using System;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class JamRedCreature : JamCreature
{
    static string[] typeTags = { ActorTags.Creature, ActorTags.Red };
    private Vector2Int _dir; // the current direction

    protected new void Start()
    {
        // pick a random vertical direction
        _dir = (Random.value > 0.5f) ? GridDirections.East : GridDirections.West;
        base.Start();
    }

    public override bool IsOfType(string type)
    {
        return typeTags.Contains(type);
    }

    // JamGridActor Implementation
    public override JamGridEntity GetGridEntity() {
        return gridData;
    }

    public override int GetPriority() {
        return priority;
    }

    public override void PreEvaluate() {
        // horizontal movement
        SingleAxisMovement(ref _dir);
    }

    public override void Step()
    {
        base.Step();
    }

    public override void PostEvaluate() 
    {
        if (gridData.Grid != null) {
            // if i am sharing a cell with a cyan guy, i'm out peace ya'll
            var entities = gridData.Grid.GetCellEntities(gridData.Column, gridData.Row);
            if (entities.Any(i => i.GetActor().IsOfType(ActorTags.Cyan) && i.GetActor().IsOfType(ActorTags.Creature)))
            {
                Debug.Log("grrr cyan bad grrr");
                ObliteratedByOtherCreature();
            }
        }
    }


}
