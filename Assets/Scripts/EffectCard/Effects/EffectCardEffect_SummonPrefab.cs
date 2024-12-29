using System.Collections.Generic;
using NUnit.Framework.Internal;
using UnityEngine;

public class EffectCardEffect_SummonPrefab : EffectCardEffect
{
    [SerializeField]
    List<GameObject> prefabs;

    [SerializeField]
    int chosenPrefab = -1;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (chosenPrefab < 0) Randomize();
    }
    
    public override void TriggerEffect(JamGrid grid, int x, int y)
    {
        Debug.Log("Effect Triggered");
        Debug.Log(chosenPrefab);
        if (chosenPrefab >= 0 && chosenPrefab < prefabs.Count) SpawnObjectAtLocation(grid, x, y, prefabs[chosenPrefab]);
    }

    public override void Randomize()
    {
        chosenPrefab = Random.Range(0, prefabs.Count);
    }
}
