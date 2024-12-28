using UnityEngine;

public class GridWrapDuplication : MonoBehaviour
{
    [SerializeField]
    JamGridActor actor;
    [SerializeField]
    SpriteRenderer duplicateThis;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Wrap Images
    private bool wrapImagesExist;
    SpriteRenderer wrapImageL;
    SpriteRenderer wrapImageLT;
    SpriteRenderer wrapImageT;
    SpriteRenderer wrapImageRT;
    SpriteRenderer wrapImageR;
    SpriteRenderer wrapImageRB;
    SpriteRenderer wrapImageB;
    SpriteRenderer wrapImageLB;

    // Update is called once per frame
    void Update()
    {
        if (actor is not null && duplicateThis is not null) {
            JamGridEntity gridData = actor.GetGridEntity();
            if (gridData.GetGridWorldWidth() != 0 && gridData.GetGridWorldHeight() != 0) {
                AnimateWrapImages(Time.deltaTime);
            } else {
                DestroyWrapImages();
            }
        } else {
            DestroyWrapImages();
        }
    }

    void CreateWrapImages() {
        DestroyWrapImages();
        wrapImageL = CreateWrapImage();
        wrapImageLT = CreateWrapImage();
        wrapImageT = CreateWrapImage();
        wrapImageRT = CreateWrapImage();
        wrapImageR = CreateWrapImage();
        wrapImageRB = CreateWrapImage();
        wrapImageB = CreateWrapImage();
        wrapImageLB = CreateWrapImage();
        wrapImagesExist = true;
    }

    SpriteRenderer CreateWrapImage() {
        GameObject wrapImage = new GameObject("Wrap Image");
        wrapImage.transform.parent = transform;
        wrapImage.AddComponent<SpriteRenderer>();
        SpriteRenderer wrapImageSprite = wrapImage.GetComponent<SpriteRenderer>();
        return wrapImageSprite;
    }

    void DestroyWrapImages() {
        if (wrapImagesExist) {
            Destroy(wrapImageL.gameObject); wrapImageL = null;
            Destroy(wrapImageLT.gameObject); wrapImageLT = null;
            Destroy(wrapImageT.gameObject); wrapImageT = null;
            Destroy(wrapImageRT.gameObject); wrapImageRT = null;
            Destroy(wrapImageR.gameObject); wrapImageR = null;
            Destroy(wrapImageRB.gameObject); wrapImageRB = null;
            Destroy(wrapImageB.gameObject); wrapImageB = null;
            Destroy(wrapImageLB.gameObject); wrapImageLB = null;
            wrapImagesExist = false;
        }
    }

    void AnimateWrapImages(float delta) {
        if (!wrapImagesExist) {CreateWrapImages();}
        float w = actor.GetGridEntity().GetGridWorldWidth();
        float h = actor.GetGridEntity().GetGridWorldHeight();
        AnimateWrapImage(wrapImageL, new Vector3(-w, 0, 0));
        AnimateWrapImage(wrapImageLT, new Vector3(-w, h, 0));
        AnimateWrapImage(wrapImageT, new Vector3(0, h, 0));
        AnimateWrapImage(wrapImageRT, new Vector3(w, h, 0));
        AnimateWrapImage(wrapImageR, new Vector3(w, 0, 0));
        AnimateWrapImage(wrapImageRB, new Vector3(w, -h, 0));
        AnimateWrapImage(wrapImageB, new Vector3(0, -h, 0));
        AnimateWrapImage(wrapImageLB, new Vector3(-w, -h, 0));
    }

    void AnimateWrapImage(SpriteRenderer image, Vector3 offset) {
        image.sprite = duplicateThis.sprite;
        image.color = duplicateThis.color;
        image.flipX = duplicateThis.flipX;
        image.flipY = duplicateThis.flipY;
        image.drawMode = duplicateThis.drawMode;
        image.gameObject.transform.localPosition = duplicateThis.gameObject.transform.localPosition + offset;
        image.gameObject.transform.localRotation = duplicateThis.gameObject.transform.localRotation;
        image.gameObject.transform.localScale = duplicateThis.gameObject.transform.localScale;
    }
}
