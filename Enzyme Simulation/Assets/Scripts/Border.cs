using UnityEngine;

public class Border : MonoBehaviour
{
    public RectTransform target;
    public float thickness;
    private RectTransform rt;
    private Vector2 adj;

    private void Start()
    {
        rt = GetComponent<RectTransform>();

        rt.anchorMax = target.anchorMax;
        rt.anchorMin = target.anchorMin;
        rt.pivot = target.pivot;
        adj.x = 2 * (rt.pivot.x - .5f);
        adj.y = 2 * (rt.pivot.y - .5f);
    }

    private void FitObject()
    {
        rt.position = new Vector2(target.position.x + adj.x * thickness, target.position.y + adj.y * thickness);
        rt.sizeDelta = new Vector2(target.sizeDelta.x + 2 * thickness, target.sizeDelta.y + 2 * thickness);
    }

    private void Update()
    {
        FitObject();
    }
}
