using System.Collections.Generic;
using UnityEngine;

public class CountdownSprite : MonoBehaviour
{
    [SerializeField]
    public int count;
    [SerializeField]
    public bool visible;
    [SerializeField]
    Sprite defaultSprite;
    [SerializeField]
    List<Sprite> numberSprites;

    SpriteRenderer spriteRenderer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (numberSprites == null) numberSprites = new List<Sprite>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        Debug.Assert(spriteRenderer != null, "Countdown Sprite could not find Sprite Renderer");
    }

    // Update is called once per frame
    void Update()
    {
        if (spriteRenderer) {
            if (!visible) {
                spriteRenderer.sprite = null;
            } else if (count >=0 && count < numberSprites.Count) {
                spriteRenderer.sprite = numberSprites[count];
            } else {
                spriteRenderer.sprite = defaultSprite;
            }
        }
    }
}
