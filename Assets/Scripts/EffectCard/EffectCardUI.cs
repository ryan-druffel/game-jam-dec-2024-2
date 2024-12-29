using System.Collections.Generic;
using System.Data;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class EffectCardUI : MonoBehaviour
{
    enum SelectionState {
        Empty = 0,
        Hovering = 1,
        Selecting = 2,
    }
    [SerializeField]
    SelectionState state;
    [SerializeField]
    GridBox gridBox;
    [SerializeField]

    EffectCard targetCard;
    [SerializeField]

    List<EffectCard> effectCards;
    [SerializeField]

    List<Transform> effectCardsSpots;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        if (effectCards == null) effectCards = new List<EffectCard>();
        if (effectCardsSpots == null) effectCardsSpots = new List<Transform>();
    }

    // Update is called once per frame
    bool mouseDownLastUpdate = false;
    bool mouseClicked = false;
    void FixedUpdate()
    {
        // Handle Inputs
        mouseClicked = !mouseDownLastUpdate && Input.GetMouseButton(0);
        mouseDownLastUpdate = Input.GetMouseButton(0);

        
        EffectCard hoveredCard;
        // State Machine
        switch (state) {
            case SelectionState.Empty:
                hoveredCard = GetHoveredCard();
                if (hoveredCard != null) {
                    SelectTarget(hoveredCard);
                    state = SelectionState.Hovering;
                }
                break;
            case SelectionState.Hovering:
                if (targetCard is null) state = SelectionState.Empty;
                hoveredCard = GetHoveredCard();
                if (mouseClicked) {
                    state = SelectionState.Selecting;
                }
                if (hoveredCard == null) {
                    DeselectTarget();
                    state = SelectionState.Empty;
                }
                break;
            case SelectionState.Selecting:
                if (targetCard is null) state = SelectionState.Empty;
                if (!Input.GetMouseButton(0) && IsOverGrid()) {
                    Vector2Int cell = GetGridCellUnderCursor();
                    if (targetCard.CanBeUsedOn(gridBox.GetGrid(), cell.x, cell.y)) {
                        targetCard.ActOn(gridBox.GetGrid(), cell.x, cell.y);
                    }
                }
                if (!Input.GetMouseButton(0)) {
                    DeselectTarget();
                    state = SelectionState.Empty;
                }
                break;
        }

        // Move Cards to Slots
        for (int i = 0; i < effectCardsSpots.Count; i++) {
            if (i < effectCards.Count && effectCards[i] != null && effectCardsSpots[i] != null) {
                effectCards[i].targetPos = effectCardsSpots[i];
            }
        }
    }

    void SelectTarget(EffectCard card) {
        card.Select();
        targetCard = card;
    }

    void DeselectTarget() {
        if (targetCard != null) targetCard.Unselect();
        targetCard = null;
    }

    EffectCard GetHoveredCard() {
        foreach (EffectCard card in effectCards) {
            if (card.IsHovered()) return card;
        }
        return null;
    }

    bool IsOverGrid() {
        if (gridBox.GetGrid() == null) return false;
        Vector2Int cell = GetGridCellUnderCursor();
        return gridBox.GetGrid().CellExists(cell.x, cell.y);
    }

    Vector2Int GetGridCellUnderCursor() {
        if (gridBox.GetGrid() == null || gridBox.GetDisplay() == null) return new Vector2Int(0,0);

        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(gridBox.GetDisplay(), Input.mousePosition, Camera.main, out localPoint);

        Vector2 localPointFromCenter = localPoint * new Vector2(1.0f, -1.0f) / gridBox.GetDisplay().rect.size;

        // Compute CRT Distortion
        Vector2 localPointFromCenterAbs = localPointFromCenter.Abs();
        Vector2 distortion = localPointFromCenter * Mathf.Pow(Vector2.Dot(localPointFromCenterAbs, localPointFromCenterAbs), 2);

        // Compute grid position
        Vector2 gridPosNormalized = (localPointFromCenter + new Vector2(0.5f, 0.5f)) + distortion;
        int col = Mathf.RoundToInt(gridPosNormalized.x * gridBox.GetGrid().Width - 0.5f);
        int row = Mathf.RoundToInt(gridPosNormalized.y * gridBox.GetGrid().Height - 0.5f);

        return new Vector2Int(col, row);
    }

    void AddCard(EffectCard card) {
        for (int i = 0; i < effectCards.Count; i++) {
            if (effectCards[i] == null) {
                effectCards[i] = card;
            }
        }
        effectCards.Add(card);
    }

    bool SpaceForNewCard() {
        return CardCount() <= effectCardsSpots.Count;
    }

    int CardCount() {
        int count = 0;
        for (int i = 0; i < effectCards.Count; i++) {
            if (effectCards[i] == null) {
                count ++;
            }
        }
        return count;
    }
}

  