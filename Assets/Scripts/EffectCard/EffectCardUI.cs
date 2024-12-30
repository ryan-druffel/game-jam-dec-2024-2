using System.Collections.Generic;
using System.Data;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

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
    public GridBox GridBox { get => gridBox; }
    [SerializeField]
    Image selectImage;
    [SerializeField]
    SpriteRenderer selectCellImage;
    [SerializeField]

    EffectCard targetCard;
    public bool targetCellValid { get; private set; }
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

    void Start()
    {
        Debug.Assert(gridBox != null, "Grid Box not supplied to UI");
    }

    // Update is called once per frame
    bool mouseDownLastUpdate = false;
    bool mouseClicked = false;
    void FixedUpdate()
    {
        // Handle Inputs
        mouseClicked = !mouseDownLastUpdate && Input.GetMouseButton(0);
        mouseDownLastUpdate = Input.GetMouseButton(0);

        targetCellValid = IsOverValidTarget();

        
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
                if (hoveredCard != targetCard) {
                    DeselectTarget();
                    SelectTarget(hoveredCard);
                }
                break;
            case SelectionState.Selecting:
                if (targetCard is null) state = SelectionState.Empty;
                if (!Input.GetMouseButton(0) && IsOverGrid()) {
                    Vector2Int cell = GetGridCellUnderCursor();
                    if (targetCellValid) {
                        targetCard.ActOn(gridBox.GetGrid(), cell.x, cell.y);
                    }
                }
                if (!Input.GetMouseButton(0)) {
                    DeselectTarget();
                    state = SelectionState.Empty;
                }
                break;
        }

        // Shift overflow cards to spots
        ShiftOverflowCardsUp();

        // Move Cards to Slots
        for (int i = 0; i < effectCardsSpots.Count; i++) {
            if (i < effectCards.Count && effectCards[i] != null && effectCardsSpots[i] != null) {
                effectCards[i].targetPos = effectCardsSpots[i];
            }
        }

        // Animate Selection
        if (selectImage) {
            Color newColor;
            if (state == SelectionState.Selecting) {
                newColor = selectImage.color;
                newColor.a = 1;
                selectImage.color = newColor;
                // selectImage.rectTransform.position = Input.mousePosition / new Vector2(Screen.width, Screen.height);
                Vector2 pos;
                RectTransformUtility.ScreenPointToLocalPointInRectangle(selectImage.canvas.transform as RectTransform, Input.mousePosition, selectImage.canvas.worldCamera, out pos);
                selectImage.transform.position = selectImage.canvas.transform.TransformPoint(pos);
            } else {
                newColor = selectImage.color;
                newColor.a = 0;
                selectImage.color = newColor;
            }
        }

        // Animate Cell Selection Preview
        if (selectCellImage) {
            Color newColor;
            if (state == SelectionState.Selecting && targetCellValid) {
                newColor = selectCellImage.color;
                newColor.a = 1;
                selectCellImage.color = newColor;
                // selectImage.rectTransform.position = Input.mousePosition / new Vector2(Screen.width, Screen.height);
                Vector2Int cell = GetGridCellUnderCursor();
                selectCellImage.transform.position = gridBox.GetGrid().GetCellXY(cell.x, cell.y);
            } else {
                newColor = selectCellImage.color;
                newColor.a = 0;
                selectCellImage.color = newColor;
            }
        }
    }

    void SelectTarget(EffectCard card) {
        card.Select();
        targetCard = card;
    }

    public void DeselectTarget() {
        if (targetCard != null) targetCard.Unselect();
        targetCard = null;
    }

    EffectCard GetHoveredCard() {
        foreach (EffectCard card in effectCards) {
            if (card != null && card.IsHovered()) return card;
        }
        return null;
    }

    bool IsOverValidTarget() {
        Vector2Int cell = GetGridCellUnderCursor();
        return IsOverGrid() && targetCard != null && gridBox != null && targetCard.CanBeUsedOn(gridBox.GetGrid(), cell.x, cell.y);
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
        Vector2 localPointFromCenterAbs = new Vector2(Mathf.Abs(localPointFromCenter.x), Mathf.Abs(localPointFromCenter.y));
        Vector2 distortion = localPointFromCenter * Mathf.Pow(Vector2.Dot(localPointFromCenterAbs, localPointFromCenterAbs), 2);

        // Compute grid position
        Vector2 gridPosNormalized = (localPointFromCenter + new Vector2(0.5f, 0.5f)) + distortion;
        int col = Mathf.RoundToInt(gridPosNormalized.x * gridBox.GetGrid().Width - 0.5f);
        int row = Mathf.RoundToInt(gridPosNormalized.y * gridBox.GetGrid().Height - 0.5f);

        return new Vector2Int(col, row);
    }

    public void AddCard(EffectCard card) {
        for (int i = 0; i < effectCards.Count; i++) {
            if (effectCards[i] == null) {
                effectCards[i] = card;
                return;
            }
        }
        effectCards.Add(card);
    }

    public bool SpaceForNewCard() {
        Debug.Log(CardCount() + " | " + effectCardsSpots.Count);
        return CardCount() < effectCardsSpots.Count;
    }

    public int CardCount() {
        int count = 0;
        for (int i = 0; i < effectCards.Count; i++) {
            if (effectCards[i] != null) {
                count ++;
            }
        }
        return count;
    }

    public void ShiftOverflowCardsUp() {
        // if more cards refs in list than spots
        if (effectCards.Count > effectCardsSpots.Count) {
            // shift up
            for (int i = effectCardsSpots.Count; i < effectCards.Count; i++) {
                // if card is here, try to place it higher
                if (effectCards[i] != null) {
                    bool shifted = false;
                    for (int j = 0; j < effectCards.Count && j < effectCardsSpots.Count && !shifted; j++) {
                        if (effectCards[j] == null) {
                            effectCards[j] = effectCards[i];
                            effectCards[i] = null;
                            shifted = true;
                        }
                    }
                }
            }
            // trim excess
            if (CardCount() <= effectCardsSpots.Count) {
                effectCards.TrimExcess();
            }
        }
    }
}

  