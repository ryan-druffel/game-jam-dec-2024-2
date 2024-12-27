using UnityEngine;

public class GameTester : MonoBehaviour
{
    [SerializeField] Sprite testerSprite;

    JamGrid grid;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        grid = new JamGrid(10, 10);
        grid.PeterGriffin();
        TickEntities();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void TickEntities()
    {
        var entities = grid.GetAllEntities();
        foreach (var entity in entities)
        {
            entity.GetTransform().position = new Vector2(entity.Column, -entity.Row);
            entity.GetDisplay().sprite = testerSprite;
        }
    }
}
