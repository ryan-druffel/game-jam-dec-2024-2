using System;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class JamCyanCreature : JamCreature
{
    static string[] typeTags = { ActorTags.Creature, ActorTags.Cyan };

    protected new void Start()
    {
        // pick a random vertical direction
        _direction = (Random.value > 0.5f) ? GridDirections.North : GridDirections.South;
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

    public override void PreEvaluate()
    {
        // vertical movement
        ShowInidcator();
    }

    public override void Step()
    {
        CalculateSimpleMovement();
        base.Step();
    }

    public override void PostEvaluate() 
    {
        if (gridData.Grid != null) {
            // if i am sharing a cell with a cyan guy, i'm out peace ya'll
            var entities = gridData.Grid.GetCellEntities(gridData.Column, gridData.Row);
            if (entities.Any(i => i.GetActor().IsOfType(ActorTags.Red) && i.GetActor().IsOfType(ActorTags.Creature)))
            {
                Debug.Log("grrr red bad grrr");
                ObliteratedByOtherCreature();
            }
        }
    }
}
