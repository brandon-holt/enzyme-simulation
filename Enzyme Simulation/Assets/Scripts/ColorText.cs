using UnityEngine;
using TMPro;

public class ColorText : MonoBehaviour
{
    [SerializeField] private float[] limits;
    [SerializeField] private Color[] colors;
    private TextMeshProUGUI text;

    private void Awake() { text = GetComponent<TextMeshProUGUI>(); }

    public void SetValue(float value, string F, string suffix)
    {
        for (int i = 0; i < limits.Length; i++)
        {
            if (value < limits[i])
            {
                text.text = value.ToString(F) + suffix;
                text.color = colors[i];
                break;
            }
        }
    }
}
