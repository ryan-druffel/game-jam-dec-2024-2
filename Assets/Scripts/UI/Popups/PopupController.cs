using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class PopupController : MonoBehaviour
{
    [SerializeField]
    EffectCardUI effectCardUI;

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
        foreach (Popup popup in popups) {
            if (!popup.triggered && popup.scoreThreshold <= effectCardUI.GridBox.GetCoordinator().Score) DisplayPopup(popup);
        }
        if (!gameOverPopup.triggered && effectCardUI.GridBox.GetCoordinator().GameOver) DisplayPopup(gameOverPopup);
    }

    void DisplayPopup(Popup popup) {
        popup.triggered = true;
        effectCardUI.GridBox.GetCoordinator().SetTimescale(0);
        
        Debug.Log("Popup Triggering");
        GameObject newObject = Instantiate(popup.prefab);
        newObject.transform.SetParent(transform, false);
    }

    class Popup {
        public GameObject prefab;
        public int scoreThreshold;
        public bool triggered;
        public Popup(GameObject prefa, int score = 0) {
            prefab = prefa;
            scoreThreshold = score;
            triggered = false;
        }
    }
    List<Popup> popups;
    Popup gameOverPopup;

    void InitializePopups() {
        popups = new List<Popup>();

        gameOverPopup = new Popup(Resources.Load<GameObject>("Prefabs/UI/GameOverPopup"));

        // Add popups here
        popups.Add(new Popup(Resources.Load<GameObject>("Prefabs/UI/IntroPopup"), 0));
    }
}
