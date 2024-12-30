using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class PopupController : MonoBehaviour
{
    [SerializeField]
    EffectCardUI effectCardUI;

    private static List<int> _popupsSeen = new List<int>();

    void Start()
    {
        Debug.Assert(effectCardUI != null, "Time Scale Button not connected to UI");
        InitializePopups();
    }

    // Update is called once per frame
    void Update()
    {
        if (effectCardUI && effectCardUI.GridBox && effectCardUI.GridBox.GetCoordinator()) CheckScoreForPopup();
    }

    void CheckScoreForPopup() {
        foreach (Popup popup in popupsStage) {
            if (!popup.triggered && popup.threshold == (int) effectCardUI.GridBox.GetCoordinator().Stage) DisplayPopup(popup);
        }
        foreach (Popup popup in popupsScore) {
            if (!popup.triggered && popup.threshold <= effectCardUI.GridBox.GetCoordinator().Score) DisplayPopup(popup);
        }
        if (!gameOverPopup.triggered && effectCardUI.GridBox.GetCoordinator().GameOver) DisplayPopup(gameOverPopup);
    }

    void DisplayPopup(Popup popup) {
        
        StartCoroutine(WaitForOpening(popup));
    }

    IEnumerator WaitForOpening(Popup popup)
    {
        // prevents reactivating popups
        if (popup.triggered) yield break;
        popup.triggered = true;

        // don't show previously seen popups
        if (!popup.isImportant && _popupsSeen.Contains(popup.prefab.GetInstanceID())) yield break;

        // wait until the coordinator has fully completed a step and has entered the delay period
        yield return new WaitUntil(() => JamCoordinator.Instance.NoActiveStep);

        // pause the game and open the popup (and deselect active card)
        effectCardUI.DeselectTarget();
        effectCardUI.GridBox.GetCoordinator().SetTimescale(0);
        GameObject newObject = Instantiate(popup.prefab);
        newObject.transform.SetParent(transform, false);

        // record the popup as seen
        _popupsSeen.Add(popup.prefab.GetInstanceID());
    }

    class Popup {
        public GameObject prefab;
        public int threshold;
        public bool triggered;
        public bool isImportant;
        public Popup(GameObject prefa, int score = 0, bool alwaysShow = false) {
            prefab = prefa;
            isImportant = alwaysShow;
            threshold = score;
            triggered = false;
        }
    }
    List<Popup> popupsScore;
    List<Popup> popupsStage;
    Popup gameOverPopup;

    void InitializePopups() {
        popupsScore = new List<Popup>();
        popupsStage = new List<Popup>();

        gameOverPopup = new Popup(Resources.Load<GameObject>("Prefabs/UI/GameOverPopup"), 0, true);

        // Add popups here
        popupsScore.Add(new Popup(Resources.Load<GameObject>("Prefabs/UI/IntroPopup"), 0));
        popupsScore.Add(new Popup(Resources.Load<GameObject>("Prefabs/UI/WallPopup"), 20));
        popupsScore.Add(new Popup(Resources.Load<GameObject>("Prefabs/UI/VictoryPopup"), 1000));
        
        popupsStage.Add(new Popup(Resources.Load<GameObject>("Prefabs/UI/StagePopupRedCyan"), (int) JamCoordinator.GameStage.RedCyan));
        popupsStage.Add(new Popup(Resources.Load<GameObject>("Prefabs/UI/StagePopupFood"), (int) JamCoordinator.GameStage.AddFood));
        popupsStage.Add(new Popup(Resources.Load<GameObject>("Prefabs/UI/StagePopupBlueYellow"), (int) JamCoordinator.GameStage.BlueYellow));
        popupsStage.Add(new Popup(Resources.Load<GameObject>("Prefabs/UI/StagePopupConveyor"), (int) JamCoordinator.GameStage.AddConveyor));
        popupsStage.Add(new Popup(Resources.Load<GameObject>("Prefabs/UI/StagePopupGreenPurple"), (int) JamCoordinator.GameStage.GreenPurple));
    }
}
