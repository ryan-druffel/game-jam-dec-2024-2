using System.Collections.Generic;
using NUnit.Framework.Internal;
using UnityEngine;
using System.Linq;
using Unity.VisualScripting;

public class EffectCardEffect_EraseEntity : EffectCardEffect
{
    [SerializeField]
    List<string> tags;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }
    
    public override void TriggerEffect(JamGrid grid, int x, int y)
    {
        List<JamGridEntity> entitiesAtCell = grid.GetCellEntities(x, y);
        foreach (JamGridEntity entity in entitiesAtCell.Where((i) => {
            foreach (string tag in tags) {
                if (i.GetActor().IsOfType(tag)) return true;
            }
            return false;
        })) {
            Destroy(entity.GetActor().gameObject);
        }
    }
    
    public override bool CanUseAt(JamGrid grid, int x, int y)
    {
        List<JamGridEntity> entitiesAtCell = grid.GetCellEntities(x, y);
        return entitiesAtCell.Any((i) => {
            foreach (string tag in tags) {
                if (i.GetActor().IsOfType(tag)) return true;
            }
            return false;
        });
    }

    public override void Randomize()
    {
    }
}
