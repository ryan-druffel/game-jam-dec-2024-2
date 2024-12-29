using TMPro;
using UnityEngine;

public class ScoreDisplay : MonoBehaviour
{
    [SerializeField]
    EffectCardUI effectCardUI;

    void Start() {
        Debug.Assert(effectCardUI != null, "Time Scale Button not connected to UI");
    }

    // Update is called once per frame
    void Update()
    {
        GetComponent<TMP_Text>().text = "SCORE: " + effectCardUI.GridBox.GetCoordinator().Score;
    }
}
