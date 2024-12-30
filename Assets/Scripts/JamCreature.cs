using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using Random = UnityEngine.Random;

public class JamCreature : JamGridActor
{
    [SerializeField]
    protected bool autoconnect = false;
    public int priority = 0;
    protected float _scaledTime;
    
    protected void Awake()
    {
        gridData = new JamGridEntity(initColumn, initRow, this);
        if (autoconnect) {
            gridData.ConnectToGrid(initGrid);
        }
    }

    protected void Update()
    {
        _scaledTime = Time.deltaTime * JamCoordinator.Instance.TimeScale;
    }

    public override bool IsOfType(string type)
    {
        return type.Equals(ActorTags.Creature);
    }

    IEnumerator walkAnim;
    protected void StartWalkAnimation (Vector2 destination) {
        StopWalkAnimation();
        walkAnim = WalkAnimation(destination);
        StartCoroutine(walkAnim);
    }
    protected void StopWalkAnimation (bool snap = true) {
        if (walkAnim != null) StopCoroutine(walkAnim);
        if (snap) SnapToGrid();
    }

    IEnumerator WalkAnimation(Vector2 destination, float speedMult = 1)
    {
        // get target position
        var dest = new Vector3(destination.x, destination.y, transform.position.z);

        // move towards the destination until it is reached
        while (transform.position != dest)
        {
            if (JamCoordinator.Instance.TimeScale > 0) transform.position = Vector3.MoveTowards(transform.position, dest, Time.deltaTime * speedMult / JamCoordinator.Instance.StepDuration);
            yield return null;
        }

        // snap to target position to be sure
        SnapToGrid();
    }

    public void RecomputeMove(float speedMult = 1) {
        StopWalkAnimation(false);
        walkAnim = WalkAnimation(gridData.GetXY(), speedMult);
        StartCoroutine(walkAnim);
    }

    // dies after a time delay
    IEnumerator FadeToDeath()
    {
        // get the sprite renderer
        var renderer = transform.GetComponentInChildren<SpriteRenderer>();
        Color currentColor = renderer.color;
        float fadeAmount = renderer.color.a / JamCoordinator.Instance.StepDuration;

        while (renderer.color.a > 0)
        {
            currentColor.a -= _scaledTime * fadeAmount;
            renderer.color = currentColor;

            yield return null;
        }

        Destroy(gameObject);
    }

    protected void ObliteratedByOtherCreature() {
        StartCoroutine(FadeToDeath());
    }

    // JamGridActor Implementation
    public override JamGridEntity GetGridEntity() {
        return gridData;
    }

    public override int GetPriority() {
        return priority;
    }

    // is the tile in the specified direction not solid?
    protected bool IsTileFree(Vector2Int direction)
    {
        // look at the tile we're about to enter, return true if there's nothing there
        var entities = gridData.Grid.GetCellEntities(gridData.Column + direction.x, gridData.Row + direction.y);
        return !entities.Any(i => i.GetActor().IsOfType(ActorTags.Solid));
    }

    // does the specified tile have an entity with a specific tag?
    protected bool DoesTileContain(Vector2Int direction, string tag)
    {
        var entities = gridData.Grid.GetCellEntities(gridData.Column + direction.x, gridData.Row + direction.y);
        return entities.Any(i => i.GetActor().IsOfType(tag));
    }

    protected Vector2Int ChooseFreeTileRandomly()
    {
        // first, get free tiles available
        List<Vector2Int> options = new List<Vector2Int>();
        foreach (var direction in GridDirections.Orthogonal)
        {
            if (IsTileFree(direction)) { options.Add(direction); }
        }

        // if all are occupied, return nothing
        if (options.Count == 0) return Vector2Int.zero;

        // pick one at random
        return options[(int)(options.Count * Random.value)];
    }

    protected Action StepIntent;
    public override void PreEvaluate() {
        StopWalkAnimation();

        // pick a random direction to move
        Vector2Int dir = ChooseFreeTileRandomly();
        if (dir != Vector2Int.zero)
        {
            // schedule movement
            StepIntent = () => {
                StartWalkAnimation(gridData.GetRelativeXY(dir.x, dir.y));
                gridData.MoveRelative(dir.x, dir.y);
            };
        }
    }

    public override void Step() {
        StepIntent?.Invoke();
    }

    public override void PostEvaluate() {

    }

    // does nothing but move either horizontally or vertically
    protected void SingleAxisMovement(ref Vector2Int direction)
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
    }
}

public class GridDirections
{
    public static readonly Vector2Int North = new Vector2Int(0, -1);
    public static readonly Vector2Int East = new Vector2Int(1, 0);
    public static readonly Vector2Int South = new Vector2Int(0, 1);
    public static readonly Vector2Int West = new Vector2Int(-1, 0);
    public static readonly Vector2Int[] Orthogonal = new Vector2Int[] { North, East, South, West };
}
