using System;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class JamYellowCreature : JamCreature
{
    static string[] typeTags = { ActorTags.Creature, ActorTags.Yellow };
    private Vector2Int _dir; // the current direction
    private Vector2Int _steering; // should I steer left or right?
    private int flop;

    protected new void Start()
    {
        // pick a random diagonal
        /*
        _dir = GridDirections.Diagonal[(int)(Random.value * 4)];
        _steering = (Random.value > 0.5f) ? new Vector2Int(-1, 1) : new Vector2Int(1, -1);
        */

        _dir = GridDirections.Orthogonal[(int)(Random.value * 4)];
        flop = (Random.value > 0.5f) ? 1 : -1;
        // _steering = (Random.value > 0.5f) ? new Vector2Int(-1, 1) : new Vector2Int(1, -1);

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
        // move in a diamond pattern
        ZigZagMovement(ref _dir);
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
            if (entities.Any(i => i.GetActor().IsOfType(ActorTags.Blue) && i.GetActor().IsOfType(ActorTags.Creature)))
            {
                Debug.Log("grrr blue bad grrr");
                ObliteratedByOtherCreature();
            }
        }
    }

    // rotate direction by 90 degrees everytime i move
    protected void DiamondMovement(ref Vector2Int direction)
    {
        // move vertically, turn around if hitting a wall
        if (!IsTileFree(direction)) direction *= -1; // turn around if next space isn't free

        // if the other way isn't free either, do nothing 
        if (!IsTileFree(direction)) { StepIntent = () => { }; return; }

        // if it's free, schedule movement
        Vector2Int dir = direction;
        StepIntent = () => {
            StartWalkAnimation(gridData.GetRelativeXY(dir.x, dir.y));
            gridData.MoveRelative(dir.x, dir.y);
        };

        // change direction based on steering, then adjust steering
        direction *= _steering;
        _steering = new Vector2Int(_steering.y, _steering.x);
    }

    protected void ZigZagMovement(ref Vector2Int direction)
    {
        // move vertically, turn around if hitting a wall
        if (!IsTileFree(direction)) { direction *= -1; flop *= -1; } // turn around if next space isn't free

        // if the other way isn't free either, do nothing 
        if (!IsTileFree(direction)) { StepIntent = () => { }; return; }

        // if it's free, schedule movement
        Vector2Int dir = direction;
        StepIntent = () => {
            StartWalkAnimation(gridData.GetRelativeXY(dir.x, dir.y));
            gridData.MoveRelative(dir.x, dir.y);
        };

        // change direction based on steering, then adjust steering
        direction = new Vector2Int(direction.y, direction.x) * flop;
    }
}
