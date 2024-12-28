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

        if (Input.GetMouseButton(0)) Debug.Log(GetGridCellUnderCursor());
        if (Input.GetMouseButton(0)) Debug.Log(IsOverGrid());
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

        Vector2 localPointFromCenter = localPoint / gridBox.GetDisplay().rect.size;

        // Compute CRT Distortion
        Vector2 localPointFromCenterAbs = localPointFromCenter.Abs();
        Vector2 distortion = localPointFromCenter * Mathf.Pow(Vector2.Dot(localPointFromCenterAbs, localPointFromCenterAbs), 2);

        // Compute grid position
        Vector2 gridPosNormalized = (localPointFromCenter + new Vector2(0.5f, 0.5f)) /*+ distortion*/;
        int col = Mathf.RoundToInt(gridPosNormalized.x * gridBox.GetGrid().Width + 0.5f);
        int row = Mathf.RoundToInt(gridPosNormalized.y * gridBox.GetGrid().Height + 0.5f);

        return new Vector2Int(col, row);
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

  