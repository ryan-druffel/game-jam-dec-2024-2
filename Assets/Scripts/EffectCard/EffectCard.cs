using System;
using System.Collections;
using System.ComponentModel;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EffectCard : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{

    [SerializeField, TextArea(1,20)]
    String briefText = "Breif Text";
    [SerializeField, TextArea(5,20)]
    String extendedText = "Extended Text";
    [SerializeField]
    Sprite image;
    [SerializeField]
    Image iconObject;
    [SerializeField]
    TMP_Text textBoxObject;
    Animator animator;
    [SerializeField]
    bool isTargeted;

    [SerializeField]
    public Transform targetPos;

    [SerializeField]
    bool hovered;

    [SerializeField]
    bool used;

    public EffectCardEffect effect;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = GetComponent<Animator>();
        Debug.Assert(animator != null, "EffectCard Missing Animator");
        effect = GetComponent<EffectCardEffect>();
        if (effect == null) {
            Debug.LogWarning("EffectCard Missing Effect");
        } else {
            effect.Randomize();
        }
    }

    // Update is called once per frame
    void Update()
    {
        MoveTowardsTargetPos(Time.deltaTime);

        // Assign to card
        iconObject.sprite = image;
        textBoxObject.text = briefText;

        // Animate card
        animator.SetBool("Targeted", isTargeted);
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
        if (effect != null) effect.TriggerEffect(grid, x, y);
        if (!used) AnimateUsed();
        used = true;
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

    void AnimateUsed() {
        StartCoroutine(AnimateUsedCoroutine());
    }
    
    IEnumerator AnimateUsedCoroutine()
    {
        animator.SetBool("Used", true);
        yield return new WaitForSeconds(1);
        Destroy(gameObject);
    }
}


public abstract class EffectCardEffect : MonoBehaviour
{
    public abstract void Randomize();
    public abstract void TriggerEffect(JamGrid grid, int x, int y);
    public JamGridActor SpawnObjectAtLocation(JamGrid grid, int x, int y, GameObject prefab) {
        Debug.Log("Summoning");
        GameObject newObject = Instantiate(prefab);
        newObject.transform.parent = grid.transform.parent;
        JamGridActor actor = newObject.GetComponent<JamGridActor>();
        if (actor == null) return null;
        actor.gridData.Move(x, y);
        actor.gridData.ConnectToGrid(grid);
        return actor;
    }
}