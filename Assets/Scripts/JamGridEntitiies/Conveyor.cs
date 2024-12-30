using System.Collections.Generic;
using UnityEngine;

public class Conveyor : JamGridActor
{
    [SerializeField]
    protected bool autoconnect = false;
    public int priority = 4;
    [SerializeField]
    public int usesRemaining = 5;
    [SerializeField]
    CountdownSprite countdownSprite;
    [SerializeField]
    Vector2Int pushDir = new Vector2Int(1, 0);
    
    protected void Awake()
    {
        gridData = new JamGridEntity(initColumn, initRow, this);
        if (autoconnect) {
            gridData.ConnectToGrid(initGrid);
        }
    }

    // JamGridActor Implementation
    public override bool IsOfType(string type)
    {
        return type.Equals(ActorTypes.Food);
    }

    public override JamGridEntity GetGridEntity() {
        return gridData;
    }

    public override int GetPriority() {
        return priority;
    }
    public override void PreEvaluate() {
        
    }

    public override void Step() {
        List<JamGridEntity> entities = gridData.Grid.GetCellEntities(gridData.Column, gridData.Row);
        foreach (JamGridEntity entity in entities) {
            if (entity != gridData) {
                entity.MoveRelative(pushDir.x, pushDir.y);
                usesRemaining--;
            }
        }
    }

    public override void PostEvaluate() {
        if (usesRemaining <= 0) Destroy (gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        // snap to target position to be sure
        SnapToGrid();

        // Update countdown timer
        if (countdownSprite != null) {
            countdownSprite.count = usesRemaining;
            if (usesRemaining > 0) {
                countdownSprite.visible = true;
            } else {
                countdownSprite.visible = false;
            }
        }
    }

    // snap to the current position
    protected void SnapToGrid()
    {
        Vector2 gridPos = gridData.GetXY();
        transform.position = new Vector3(gridPos.x, gridPos.y, transform.position.z);
    }
}
