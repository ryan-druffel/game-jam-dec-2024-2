using System;
using System.Collections;
using System.Linq;

using UnityEngine;

public class JamCreature : JamGridActor
{
    [SerializeField]
    protected bool autoconnect = false;
    public Vector2Int move;
    public int priority = 0;
    
    protected void Awake()
    {
        gridData = new JamGridEntity(initColumn, initRow, this);
        if (autoconnect) {
            gridData.ConnectToGrid(initGrid);
        }
    }

    protected void Start()
    {
        SnapToGrid();
    }

    public override bool IsOfType(string type)
    {
        return type.Equals(ActorTypes.Creature);
    }

    IEnumerator walkAnim;
    protected void StartWalkAnimation (Vector2 destination) {
        StopWalkAnimation();
        walkAnim = WalkAnimation(destination);
        StartCoroutine(walkAnim);
    }
    protected void StopWalkAnimation () {
        if (walkAnim != null) StopCoroutine(walkAnim);
        SnapToGrid();
    }

    IEnumerator WalkAnimation(Vector2 destination)
    {
        // get target position
        var dest = new Vector3(destination.x, destination.y, transform.position.z);

        // move towards the destination until it is reached
        while (transform.position != dest)
        {
            if (JamCoordinator.Instance.TimeScale > 0) transform.position = Vector3.MoveTowards(transform.position, dest, Time.deltaTime / JamCoordinator.Instance.StepDuration);
            yield return null;
        }

        // snap to target position to be sure
        SnapToGrid();
    }

    protected void ObliteratedByOtherCreature() {
        JamCoordinator.Instance.GameOver = true;
        // eliminate self
        Destroy(gameObject);
    }

    // snap to the current position
    protected void SnapToGrid()
    {
        Vector2 gridPos = gridData.GetXY();
        transform.position = new Vector3(gridPos.x, gridPos.y, transform.position.z);
    }

    // JamGridActor Implementation
    public override JamGridEntity GetGridEntity() {
        return gridData;
    }

    public override int GetPriority() {
        return priority;
    }

    public override void PreEvaluate() {
        StopWalkAnimation();

        // look at the tile we're about to enter
        var entities = gridData.Grid.GetCellEntities(gridData.Column + move.x, gridData.Row + move.y);

        // are any of these a wall?
        if (entities.Any(i => i.GetActor().IsOfType(ActorTypes.Wall)))
        {
            Debug.Log("There's a wall here!");
            // try to turn around
            move *= -1;
        }
    }

    public override void Step() {
        StartWalkAnimation(gridData.GetRelativeXY(move.x, move.y));
        gridData.MoveRelative(move.x, move.y);
        // Debug.Log(transform.name + " is at " + gridData.GetColumn() + ", " + gridData.GetRow());
    }

    public override void PostEvaluate() {

    }
}
