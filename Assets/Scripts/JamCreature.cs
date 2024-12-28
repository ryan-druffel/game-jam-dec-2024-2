using System;
using Unity.VisualScripting;
using UnityEngine;

public class JamCreature : JamGridActor
{

    [SerializeField]
    private bool autoconnect = false;
    [SerializeField]
    private int initRow = 0;
    [SerializeField]
    private int initColumn = 0;
    [SerializeField]
    private JamGrid grid;

    [SerializeField]
    public JamGridEntity gridData;

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

    }

    public override void Step() {
        StartWalkAnimation(gridData.GetRelativeXY(move.x, move.y));
        gridData.MoveRelative(move.x, move.y);
        Debug.Log(gridData.GetXY());
    }

    public override void PostEvaluate() {

    }
}
