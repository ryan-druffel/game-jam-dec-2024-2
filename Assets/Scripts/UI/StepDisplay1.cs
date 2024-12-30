using TMPro;
using UnityEngine;

public class StepDisplay1 : MonoBehaviour
{

    // Update is called once per frame
    void Update()
    {
        if (JamCoordinator.Instance) GetComponent<TMP_Text>().text = "STEP: " + JamCoordinator.Instance.StepCount;
    }
}
