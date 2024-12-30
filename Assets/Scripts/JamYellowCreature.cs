using System;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class JamYellowCreature : JamCreature
{
    static string[] typeTags = { ActorTags.Creature, ActorTags.Yellow };
    private int _flop;

    protected new void Start()
    {
        _direction = GridDirections.Orthogonal[(int)(Random.value * 4)];
        _flop = (Random.value > 0.5f) ? 1 : -1;

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
        CalculateZigZagMovement();
        base.Step();
    }

    public override void PostEvaluate()
    {
        if (gridData.Grid != null)
        {
            // if i am sharing a cell with a cyan guy, i'm out peace ya'll
            var entities = gridData.Grid.GetCellEntities(gridData.Column, gridData.Row);
            if (entities.Any(i => i.GetActor().IsOfType(ActorTags.Blue) && i.GetActor().IsOfType(ActorTags.Creature)))
            {
                Debug.Log("grrr blue bad grrr");
                ObliteratedByOtherCreature();
            }
        }
    }

    protected void CalculateZigZagMovement()
    {
        // move vertically, turn around if hitting a wall
        if (!IsTileFree(_direction)) { _direction *= -1; _flop *= -1; } // turn around if next space isn't free

        // if the other way isn't free either, do nothing 
        if (!IsTileFree(_direction)) { StepIntent = () => { }; return; }

        // show intent
        ShowInidcator();

        // if it's free, schedule movement
        Vector2Int dir = _direction;
        StepIntent = () => {
            StartWalkAnimation(gridData.GetRelativeXY(dir.x, dir.y));
            gridData.MoveRelative(dir.x, dir.y);
        };

        // change direction based on steering, then adjust steering
        _direction = new Vector2Int(_direction.y, _direction.x) * _flop;
    }
}
