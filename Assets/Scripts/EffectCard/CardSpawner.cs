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
}
