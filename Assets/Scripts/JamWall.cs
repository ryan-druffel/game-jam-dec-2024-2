using UnityEngine;

public class JamWall : JamGridActor
{
    [SerializeField]
    private bool autoconnect = false;

    void Awake()
    {
        gridData = new JamGridEntity(0, 0, this);
        if (autoconnect)
        {
            gridData.Move(initColumn, initRow);
            gridData.ConnectToGrid(initGrid);
        }
    }

    void Update() {
        transform.position = gridData.GetXY();
    }

    public override JamGridEntity GetGridEntity()
    {
        return gridData;
    }

    public override int GetPriority()
    {
        return -1;
    }

    public override bool IsOfType(string type)
    {
        return type.ToLower().Equals(ActorTags.Wall.ToLower());
    }

    public override void PreEvaluate()
    {
        
    }

    public override void Step()
    {
        
    }

    public override void PostEvaluate()
    {
        transform.position = gridData.Grid.GetCellXY(initColumn, initRow);
    }
}
