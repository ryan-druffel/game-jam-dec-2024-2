using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine;
using UnityEngine.EventSystems;

public class TimeScaleButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField]
    EffectCardUI effectCardUI;
    [SerializeField]
    float timeScale = 1;

    enum ButtonState {
        Idle,
        Hovered,
        Pressed,
    }
    ButtonState state = ButtonState.Idle;
    bool hovered = false;

    [SerializeField]
    Sprite idleSprite;
    [SerializeField]
    Sprite hoveredSprite;
    [SerializeField]
    Sprite pressedSprite;

    void Start() {
        Debug.Assert(effectCardUI != null, "Time Scale Button not connected to UI");
    }

    
    bool mouseDownLastUpdate = false;
    bool mouseClicked = false;
    void Update()
    {
        // Handle Inputs
        mouseClicked = !mouseDownLastUpdate && Input.GetMouseButton(0);
        mouseDownLastUpdate = Input.GetMouseButton(0);

        switch (state) {
            case ButtonState.Idle:
                GetComponent<UnityEngine.UI.Image>().sprite = idleSprite;
                if (hovered) state = ButtonState.Hovered;
                break;
            case ButtonState.Hovered:
                GetComponent<UnityEngine.UI.Image>().sprite = hoveredSprite;
                if (!hovered) state = ButtonState.Idle;
                if (mouseClicked) {
                    AssignTimeScale();
                    state = ButtonState.Pressed;
                }
                break;
            case ButtonState.Pressed:
                GetComponent<UnityEngine.UI.Image>().sprite = pressedSprite;
                if (!hovered || !Input.GetMouseButton(0)) state = ButtonState.Idle;
                break;
        }
        
        
    }

    void AssignTimeScale() {
        effectCardUI.GridBox.GetCoordinator().SetTimescale(timeScale);
    }
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        hovered = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        hovered = false;
    }
}
