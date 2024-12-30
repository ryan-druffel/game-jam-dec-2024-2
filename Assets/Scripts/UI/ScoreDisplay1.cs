using TMPro;
using UnityEngine;

public class ScoreDisplay1 : MonoBehaviour
{

    // Update is called once per frame
    void Update()
    {
        if (JamCoordinator.Instance) GetComponent<TMP_Text>().text = "SCORE: " + JamCoordinator.Instance.Score;
    }
}
