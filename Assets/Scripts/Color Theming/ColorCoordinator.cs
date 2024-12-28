using UnityEngine;

public class ColorCoordinator : MonoBehaviour
{
    public Color theme;
    public Camera controlCamera;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (controlCamera is not null) controlCamera.backgroundColor = theme;
    }
}
