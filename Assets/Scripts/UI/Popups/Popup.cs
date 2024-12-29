using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Popup : MonoBehaviour
{
    [SerializeField]
    bool freezeGame = true;

    public void ClosePopup() {
        Destroy(gameObject);
    }
    public static void RestartGame() {
        SceneManager.LoadScene(1);
    }
    void Update() {
        if (freezeGame) {
            JamCoordinator coordinator = FindAnyObjectByType<JamCoordinator>();
            if (coordinator) coordinator.SetTimescale(0);
        }
    }
}
