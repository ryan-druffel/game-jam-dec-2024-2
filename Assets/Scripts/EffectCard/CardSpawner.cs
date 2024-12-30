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
            CheckIfNewStageCards();
        }
    }

    void CheckIfNewCardNeedsToSpawn() {
        int currentStep = effectCardUI.GridBox.GetCoordinator().StepCount;
        if (lastSpawnStep <= currentStep - stepsPerSpawn && effectCardUI.SpaceForNewCard()) {
            SpawnRandomCard(effectCardUI.GridBox.GetCoordinator().Stage);
            lastSpawnStep = currentStep;
        }
    }

    void CheckIfNewStageCards() {
        JamCoordinator.GameStage stage = effectCardUI.GridBox.GetCoordinator().Stage;
        switch (stage) {
            case JamCoordinator.GameStage.RedCyan:
                if (!redAndBlueSummoned) SpawnCards(redAndBlueStartCards);
                redAndBlueSummoned = true;
                break;
            case JamCoordinator.GameStage.AddFood:
                if (!addFoodSummoned) SpawnCards(addFoodStartCards);
                addFoodSummoned = true;
                break;
            case JamCoordinator.GameStage.BlueYellow:
                if (!blueYellowSummoned) SpawnCards(blueYellowStartCards);
                blueYellowSummoned = true;
                break;
            case JamCoordinator.GameStage.AddConveyor:
                if (!addConveyorSummoned) SpawnCards(addConveyorStartCards);
                addConveyorSummoned = true;
                break;
            case JamCoordinator.GameStage.GreenPurple:
                if (!greenPurpleSummoned) SpawnCards(greenPurpleStartCards);
                greenPurpleSummoned = true;
                break;
        }
    }

    void SpawnRandomCard(JamCoordinator.GameStage stage = 0) {
        GameObject prefab = null;
        switch (stage) {
            case JamCoordinator.GameStage.RedCyan:
                Debug.Log("Red and Cyan Card");
                prefab = ChooseRandomPrefabFromList(redAndBlueCards, redAndBlueChances);
                break;
            case JamCoordinator.GameStage.AddFood:
                prefab = ChooseRandomPrefabFromList(addFoodCards, addFoodChances);
                break;
            case JamCoordinator.GameStage.BlueYellow:
                prefab = ChooseRandomPrefabFromList(blueYellowCards, blueYellowChances);
                break;
            case JamCoordinator.GameStage.AddConveyor:
                prefab = ChooseRandomPrefabFromList(addConveyorCards, addConveyorChances);
                break;
            case JamCoordinator.GameStage.GreenPurple:
                prefab = ChooseRandomPrefabFromList(greenPurpleCards, greenPurpleChances);
                break;
        }
        GameObject go = SpawnCard(prefab);
        if (go != null) go.GetComponent<EffectCardEffect>().Randomize();
    }

    GameObject ChooseRandomPrefabFromList(List<GameObject> prefabList, List<float> weightList) {
        float totalweight = 0;
        foreach (float weight in weightList) {
            totalweight += weight;
        }
        float rand = Random.Range(0.0f, totalweight);
        Debug.Log("Total Weight: " + totalweight);
        float accumWeight = 0;
        int indexOfSelection = -1;
        for (int i = 0; i < weightList.Count && indexOfSelection == -1; i++) {
            accumWeight += weightList[i];
            if (rand <= accumWeight) {
                indexOfSelection = i;
            }
        }
        if (indexOfSelection < prefabList.Count && indexOfSelection >= 0) {
            return prefabList[indexOfSelection];
        }
        return null;
    }

    GameObject SpawnCard(GameObject prefab) {
        Debug.Log("Summoning Card");
        GameObject newObject = null;
        if (prefab != null) {
            newObject = Instantiate(prefab);
            newObject.transform.SetParent(transform.parent, false);
            newObject.transform.position = spawnLocation.position;
            effectCardUI.AddCard(newObject.GetComponent<EffectCard>());
        }
        return newObject;
    }

    void SpawnCards(List<GameObject> prefabList) {
        foreach (GameObject prefab in prefabList) {
            SpawnCard(prefab);
        }
    }

    // Cards to spawn when entering stage
    bool redAndBlueSummoned = false;
    [SerializeField]
    List<GameObject> redAndBlueStartCards;
    bool addFoodSummoned = false;
    [SerializeField]
    List<GameObject> addFoodStartCards;
    bool blueYellowSummoned = false;
    [SerializeField]
    List<GameObject> blueYellowStartCards;
    bool addConveyorSummoned = false;
    [SerializeField]
    List<GameObject> addConveyorStartCards;
    bool greenPurpleSummoned = false;
    [SerializeField]
    List<GameObject> greenPurpleStartCards;


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
