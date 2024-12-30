using System.Collections.Generic;
using UnityEngine;

public class CardSpawner : MonoBehaviour
{
    
    [SerializeField]
    EffectCardUI effectCardUI;
    [SerializeField]
    Transform spawnLocation;

    [SerializeField]
    int stepsPerSpawn = 8;
    [SerializeField]
    int lastSpawnStep = 0;
    
 
    void Start()
    {
        Debug.Assert(effectCardUI != null, "Card Spawner not connected to UI");
    }


    void FixedUpdate()
    {
        if (effectCardUI != null && effectCardUI.GridBox != null && effectCardUI.GridBox.GetCoordinator() != null) {
            CheckIfNewCardNeedsToSpawn();
        }
    }

    void CheckIfNewCardNeedsToSpawn() {
        int currentStep = effectCardUI.GridBox.GetCoordinator().StepCount;
        if (lastSpawnStep <= currentStep - stepsPerSpawn && effectCardUI.SpaceForNewCard()) {
            SpawnRandomCard(effectCardUI.GridBox.GetCoordinator().Stage);
            lastSpawnStep = currentStep;
        }
    }

    [SerializeField]
    GameObject testPrefab;

    void SpawnRandomCard(JamCoordinator.GameStage stage = 0) {
        GameObject go = SpawnCard(testPrefab);
        go.GetComponent<EffectCardEffect>().Randomize();
    }

    GameObject SpawnCard(GameObject prefab) {
        Debug.Log("Summoning Card");
        GameObject newObject = Instantiate(prefab);
        newObject.transform.SetParent(transform.parent, false);
        newObject.transform.position = spawnLocation.position;
        effectCardUI.AddCard(newObject.GetComponent<EffectCard>());
        return newObject;
    }

    // Cards per State
    [SerializeField]
    List<GameObject> redAndBlueCards;
    [SerializeField]
    List<float> redAndBlueChances;
    [SerializeField]
    List<GameObject> addFoodCards;
    [SerializeField]
    List<float> addFoodChances;
    [SerializeField]
    List<GameObject> blueYellowCards;
    [SerializeField]
    List<float> blueYellowChances;
    [SerializeField]
    List<GameObject> addConveyorCards;
    [SerializeField]
    List<float> addConveyorChances;
    [SerializeField]
    List<GameObject> greenPurpleCards;
    [SerializeField]
    List<float> greenPurpleChances;
}
