using System;
using System.Linq;
using UnityEngine;

public class JamRedCreature : JamCreature
{
    static string[] typeTags = { ActorTypes.Creature, ActorTypes.Red };
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
        base.PreEvaluate();
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
            if (entities.Any(i => i.GetActor().IsOfType(ActorTypes.Cyan) && i.GetActor().IsOfType(ActorTypes.Creature)))
            {
                Debug.Log("grrr cyan bad grrr");
                // eliminate self
                Destroy(gameObject);
            }
        }
    }


}
