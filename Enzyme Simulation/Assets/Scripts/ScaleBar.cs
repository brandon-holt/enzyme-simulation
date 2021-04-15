using UnityEngine;
using TMPro;

public class ScaleBar : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI min, max, label;
    [SerializeField] private Material material;

    public void SetText(string minVal, string maxVal, string labelVal)
    {
        min.text = minVal;
        max.text = maxVal;
        label.text = labelVal;
    }

    public void SetColors(Color low, Color high)
    {
        material.SetColor("_TopColor", low);
        material.SetColor("_BottomColor", high);
    }
}
