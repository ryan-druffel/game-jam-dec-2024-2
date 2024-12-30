using UnityEngine;
using UnityEngine.Rendering;

public class FollowOtherTransform : MonoBehaviour
{
    [SerializeField]
    RectTransform other;
    [SerializeField]
    Vector2Int sizeOffset;

    // Update is called once per frame
    void Update()
    {
        if (other != null) {
            RectTransform rt = transform.GetComponent<RectTransform>();
            rt.position = other.position;
            rt.sizeDelta = other.sizeDelta + sizeOffset;
        }
    }
}
