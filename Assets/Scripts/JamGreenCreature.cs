using System;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class JamGreenCreature : JamCreature
{
    static string[] typeTags = { ActorTags.Creature, ActorTags.Green };
    private Vector2Int _steering; // should I steer left or right?

    protected new void Start()
    {
        // start random diamond pattern
        _direction = GridDirections.Diagonal[(int)(Random.value * 4)];
        _steering = (Random.value > 0.5f) ? new Vector2Int(-1, 1) : new Vector2Int(1, -1);
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
        // rising diagonal movement
        CalculateDiamondMovement();
    }

    public override void PostEvaluate() 
    {
        if (gridData.Grid != null) {
            // if i am sharing a cell with a cyan guy, i'm out peace ya'll
            var entities = gridData.Grid.GetCellEntities(gridData.Column, gridData.Row);
            if (entities.Any(i => i.GetActor().IsOfType(ActorTags.Pink) && i.GetActor().IsOfType(ActorTags.Creature)))
            {
                Debug.Log("grrr pink bad grrr");
                ObliteratedByOtherCreature();
            }
        }
    }

    // rotate direction by 90 degrees everytime i move
    protected void CalculateDiamondMovement()
    {
        // move vertically, turn around if hitting a wall
        if (!IsTileFree(_direction)) _direction *= -1; // turn around if next space isn't free

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
        _direction *= _steering;
        _steering = new Vector2Int(_steering.y, _steering.x);
    }
}
