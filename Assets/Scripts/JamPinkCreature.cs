using System;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class JamPinkCreature : JamCreature
{
    static string[] typeTags = { ActorTags.Creature, ActorTags.Pink };

    protected new void Start()
    {
        // pick a random diagonal direction
        _direction = GridDirections.Diagonal[(int)(Random.value * 4)];
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
        // show intent
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
            if (entities.Any(i => i.GetActor().IsOfType(ActorTags.Green) && i.GetActor().IsOfType(ActorTags.Creature)))
            {
                Debug.Log("grrr green bad grrr");
                ObliteratedByOtherCreature();
            }
        }
    }


}
