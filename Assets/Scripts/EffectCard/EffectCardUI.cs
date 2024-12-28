using System.Data;
using UnityEngine;
using UnityEngine.UIElements;

public class EffectCardUI : MonoBehaviour
{

    enum SelectionState {
        Empty = 0,
        Hovering = 1,
        Selecting = 2,
    }
    SelectionState state;

    EffectCard targetCard;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        switch (state) {
            case SelectionState.Empty:
                break;
            case SelectionState.Hovering:
                if (targetCard is null) state = SelectionState.Empty;
                if (Input.GetMouseButton(0)) state = SelectionState.Selecting;
                break;
            case SelectionState.Selecting:
                if (targetCard is null) state = SelectionState.Empty;
                // if (!Input.GetMouseButton(0) && IsOverGrid) {
                //     if (targetCard.CanBeUsedOn(Grid, Column, Row)) {

                //     } else {
                //         UnselectTarget();
                //     }
                // }
                break;
        }
    }

    void SelectTarget(EffectCard card) {
        card.Select();
        targetCard = card;
    }

    void UnselectTarget() {
        targetCard.Unselect();
        targetCard = null;
    }
}

  