using System.Collections.Generic;
using UnityEngine;

public class ColorFollower : MonoBehaviour
{
    [SerializeField]
    ColorCoordinator coordinator;

    enum FollowMode {
        Fade = 0,
        InverseFade = 1,
        LightDark = 2,
    }
    [SerializeField]
    FollowMode followMode = FollowMode.Fade;
    [SerializeField]
    SpriteRenderer spriteRenderer;
    [SerializeField]
    List<Color> colorParameters;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (spriteRenderer is null) spriteRenderer = transform.GetComponent<SpriteRenderer>();
        Debug.Assert(spriteRenderer is not null, "Could not find sprite renderer");
        if (colorParameters == null) colorParameters = new List<Color>();
    }

    // Update is called once per frame
    void Update()
    {
        if (coordinator is null) {
            coordinator = FindAnyObjectByType<ColorCoordinator>();
        }
        if (coordinator is not null) {
            Color newColor;
            switch (followMode) {
                case FollowMode.Fade:
                    newColor = spriteRenderer.color;
                    newColor.a = 0.5f + Mathf.Sqrt(coordinator.theme.r * coordinator.theme.r + coordinator.theme.g * coordinator.theme.g + coordinator.theme.b * coordinator.theme.b) / (2 * Mathf.Sqrt(3));
                    spriteRenderer.color = newColor;
                    break;
                case FollowMode.InverseFade:
                    newColor = spriteRenderer.color;
                    newColor.a = 1f - Mathf.Sqrt(coordinator.theme.r * coordinator.theme.r + coordinator.theme.g * coordinator.theme.g + coordinator.theme.b * coordinator.theme.b) / (2 * Mathf.Sqrt(3));
                    spriteRenderer.color = newColor;
                    break;
                case FollowMode.LightDark:
                    if (colorParameters.Count >= 2) {
                        if (Mathf.Sqrt(coordinator.theme.r * coordinator.theme.r + coordinator.theme.g * coordinator.theme.g + coordinator.theme.b * coordinator.theme.b) / Mathf.Sqrt(3) > 0.5) {
                            // Theme is Light
                            spriteRenderer.color = ApplyRGB(colorParameters[0], spriteRenderer.color);
                        } else {
                            // Theme is Dark
                            spriteRenderer.color = ApplyRGB(colorParameters[1], spriteRenderer.color);
                        }
                    }
                    break;
            }
        }
    }

    Color ApplyRGB(Color fromColor, Color toColor) {
        Color newColor = fromColor;
        newColor.a = toColor.a;
        return newColor;
    }
}
