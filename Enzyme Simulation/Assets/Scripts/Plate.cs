using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Plate : MonoBehaviour
{
    [SerializeField] private Image background;
    [SerializeField] private Transform rows, cols;

    public void SetColors(Color plate, Color text)
    {
        background.color = plate;

        foreach(TextMeshProUGUI t in rows.GetComponentsInChildren<TextMeshProUGUI>()) { t.color = text; }
        foreach (TextMeshProUGUI t in cols.GetComponentsInChildren<TextMeshProUGUI>()) { t.color = text; }
    }
}
