using System.ComponentModel;
using UnityEngine;
using UnityEngine.EventSystems;

public class EffectCard : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{

    [SerializeField]
    bool isTargeted = false;

    [SerializeField]
    public Transform targetPos;

    [SerializeField]
    bool hovered;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        MoveTowardsTargetPos(Time.deltaTime);
    }
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        hovered = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        hovered = false;
    }

    public bool IsHovered() => hovered;

    public void ActOn(JamGrid grid, int x, int y) { 
        Destroy(gameObject);
    }

    public bool CanBeUsedOn(JamGrid grid, int x, int y) { return true; }
    public void Select() {
        isTargeted = true;
    }
    public void Unselect() {
        isTargeted = false;
    }

    void MoveTowardsTargetPos(float delta) {
        if (targetPos != null) {
            float positionChange = Mathf.Log((transform.position - targetPos.position).magnitude + 1.0f) * (transform.position - targetPos.position).magnitude / 0.1f;
            transform.position = Vector3.MoveTowards(transform.position, targetPos.position, positionChange * delta);
        }
    }
}
