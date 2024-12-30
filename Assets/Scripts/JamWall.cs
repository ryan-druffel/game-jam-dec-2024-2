using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JamWall : JamGridActor
{
    [SerializeField]
    private bool autoconnect = false;
    [SerializeField]
    private int lifetime = 15;

    void Awake()
    {
        gridData = new JamGridEntity(0, 0, this);
        if (autoconnect)
        {
            gridData.Move(initColumn, initRow);
            gridData.ConnectToGrid(initGrid);
        }
    }

    public override JamGridEntity GetGridEntity()
    {
        return gridData;
    }

    public override int GetPriority()
    {
        return -1;
    }

    public override bool IsOfType(string type)
    {
        return type.ToLower().Equals(ActorTags.Solid.ToLower());
    }

    public override void PreEvaluate()
    {
        
    }

    public override void Step()
    {
        lifetime--;
        if (lifetime < 0) StartCoroutine(FadeToDeath());
    }

    public override void PostEvaluate()
    {
        transform.position = gridData.GetXY();
    }

    protected IEnumerator FadeToDeath()
    {
        // get the sprite renderer
        var renderer = transform.GetComponent<SpriteRenderer>();
        float alpha = renderer.color.a;
        Color baseColor = renderer.color;

        while (alpha > 0)
        {
            alpha -= _scaledTime;
            renderer.color = new Color(baseColor.r, baseColor.g, baseColor.b, alpha);
            yield return null;
        }

        Debug.Log("I'm dead bruh");
        Destroy(gameObject);
    }
}
