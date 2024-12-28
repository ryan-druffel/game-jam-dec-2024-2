using System;
using System.Linq;

using UnityEngine;

public class JamCreature : JamGridActor
{
    [SerializeField]
    private bool autoconnect = false;

    [SerializeField]
    public Vector2Int move;

    [SerializeField]
    public int priority = 0;
    
    void Awake()
    {
        gridData = new JamGridEntity(0, 0, this);
        if (autoconnect) {
            gridData.Move(initColumn, initRow);
            gridData.ConnectToGrid(grid);
        }
    }
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    void Update() {
        if (isWalking) {
            AnimateWalk(Time.deltaTime);
        } else {
            Vector2 gridPos = gridData.GetXY();
            transform.position = new Vector3(gridPos.x, gridPos.y, transform.position.z);
        }
    }
    
    void FixedUpdate()
    {
    }

    public override bool IsOfType(string type)
    {
        return type.ToLower().Equals(ActorTypes.Creature.ToLower());
    }

    Vector2 animWalkStart = Vector2.zero;
    Vector2 animWalkDest = Vector2.zero;
    bool isWalking = false;
    void StartWalkAnimation (Vector2 destination) {
        animWalkStart = transform.position;
        animWalkDest = destination;
        isWalking = true;
    }
    void AnimateWalk(float delta) {
        transform.position = Vector3.MoveTowards(transform.position, new Vector3(animWalkDest.x, animWalkDest.y, transform.position.z), delta * 2);
        if (animWalkDest == new Vector2(transform.position.x, transform.position.y)) { isWalking = false; }
    }

    // JamGridActor Implementation
    public override JamGridEntity GetGridEntity() {
        return gridData;
    }

    public override int GetPriority() {
        return priority;
    }

    public override void PreEvaluate() {
        // look at the tile we're about to enter
        var entities = grid.GetCellEntities(gridData.Column + move.x, gridData.Row + move.y);

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
        Debug.Log(transform.name + " is at " + gridData.GetColumn() + ", " + gridData.GetRow());
    }

    public override void PostEvaluate() {

    }
}
