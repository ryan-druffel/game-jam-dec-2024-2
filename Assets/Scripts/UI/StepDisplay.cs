using TMPro;
using UnityEngine;

public class StepDisplay : MonoBehaviour
{
    [SerializeField]
    EffectCardUI effectCardUI;

    void Start() {
        Debug.Assert(effectCardUI != null, "Step Display not connected to UI");
    }

    // Update is called once per frame
    void Update()
    {
        if (GetComponent<TMP_Text>() && effectCardUI.GridBox && effectCardUI.GridBox.GetCoordinator()) GetComponent<TMP_Text>().text = "STEP: " + effectCardUI.GridBox.GetCoordinator().StepCount;
    }
}
