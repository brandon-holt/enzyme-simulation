using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BarGraph : MonoBehaviour
{
    [SerializeField] private Transform graphArea;
    [SerializeField] private Transform axes;
    [SerializeField] private TextMeshProUGUI[] titles;
    [SerializeField] private GameObject barPrefab;

    public void GenerateGraph(float[] data, string[] labels)
    { StartCoroutine(Graph(data, labels)); }
    private IEnumerator Graph(float[] data, string[] labels)
    {
        foreach (float _ in data) { Instantiate(barPrefab, graphArea); }

        yield return null;

        graphArea.GetComponent<HorizontalLayoutGroup>().enabled = false;

        float height = graphArea.GetComponent<RectTransform>().rect.height;
        float max = Mathf.Max(data);
        if (max == 0f) { max = 1f; }

        for (int i = 0; i < data.Length; i++)
        { SetHeight(graphArea.GetChild(i).GetComponent<RectTransform>(), height * data[i] / max); }

        for (int i = 0; i < labels.Length; i++) { titles[i].text = labels[i]; }
    }

    private void SetHeight(RectTransform t, float height)
    { t.sizeDelta = new Vector2(t.sizeDelta.x, height); }

    public void CustomizeGraph(Color barColor, Color axisColor)
    {
        axes.GetComponent<Image>().color = axisColor;
        foreach (Transform t in graphArea) { t.GetComponent<Image>().color = barColor; }
    }

    public void ClearGraph()
    {
        foreach (Transform child in graphArea)
        { if (child.gameObject != null) { Destroy(child.gameObject); } }
        graphArea.GetComponent<HorizontalLayoutGroup>().enabled = true;
    }
}
