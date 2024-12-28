using UnityEngine;

public class ColorFollower : MonoBehaviour
{
    ColorCoordinator coordinator;

    enum FollowMode {
        Fade = 0,
        InverseFade = 1,
    }
    [SerializeField]
    FollowMode followMode = FollowMode.Fade;
    [SerializeField]
    SpriteRenderer spriteRenderer;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (spriteRenderer is null) spriteRenderer = transform.GetComponent<SpriteRenderer>();
        Debug.Assert(spriteRenderer is not null, "Could not find sprite renderer");
    }

    // Update is called once per frame
    void Update()
    {
        if (coordinator is null) {
            coordinator = GameObject.FindAnyObjectByType<ColorCoordinator>();
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
            }
        }
    }
}
