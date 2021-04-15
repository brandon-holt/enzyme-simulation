using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LineGraph : MonoBehaviour
{
    [SerializeField] private GameObject pointPrefab;
    [SerializeField] private Transform graphArea;
    [SerializeField] private Transform axes;
    [SerializeField] private TextMeshProUGUI[] titles;

    public void GenerateGraph(float[] data, string[] labels)
    { StartCoroutine(Graph(data, labels)); }
    private IEnumerator Graph(float[] data, string[] labels)
    {
        foreach (float _ in data) { Instantiate(pointPrefab, graphArea); }
        yield return null;
        //Destroy(graphArea.GetComponent<HorizontalLayoutGroup>());
        graphArea.GetComponent<HorizontalLayoutGroup>().enabled = false;

        float height = graphArea.GetComponent<RectTransform>().rect.height;
        float max = MaxValue(data);
        if (max == 0f) { max = 1f; }

        for (int i = 0; i < data.Length; i++)
        { SetHeight(graphArea.GetChild(i), height * data[i] / max); }

        for (int i = 0; i < labels.Length; i++) { titles[i].text = labels[i]; }
    }

    public void GenerateGraph(int[] data, string[] labels)
    { StartCoroutine(Graph(data, labels)); }
    private IEnumerator Graph(int[] data, string[] labels)
    {
        foreach (int _ in data) { Instantiate(pointPrefab, graphArea); }
        yield return null;
        //Destroy(graphArea.GetComponent<HorizontalLayoutGroup>());
        graphArea.GetComponent<HorizontalLayoutGroup>().enabled = false;

        float height = graphArea.GetComponent<RectTransform>().rect.height;
        float max = MaxValue(data);
        if (max == 0f) { max = 1f; }

        for (int i = 0; i < data.Length; i++)
        { SetHeight(graphArea.GetChild(i), height * data[i] / max); }

        for (int i = 0; i < labels.Length; i++) { titles[i].text = labels[i]; }
    }

    public void GenerateGraph(int[] data, string[] labels, int max)
    { StartCoroutine(Graph(data, labels, max)); }
    private IEnumerator Graph(int[] data, string[] labels, int max)
    {
        foreach (int _ in data) { Instantiate(pointPrefab, graphArea); }
        yield return null;
        //Destroy(graphArea.GetComponent<HorizontalLayoutGroup>());
        graphArea.GetComponent<HorizontalLayoutGroup>().enabled = false;

        float height = graphArea.GetComponent<RectTransform>().rect.height;
        if (max == 0) { max = 1; }

        for (int i = 0; i < data.Length; i++)
        { SetHeight(graphArea.GetChild(i), height * data[i] / max); }

        for (int i = 0; i < labels.Length; i++) { titles[i].text = labels[i]; }
    }

    private void SetHeight(Transform t, float pos)
    { t.localPosition = new Vector2(t.localPosition.x, pos); }

    public void CustomizeGraph(Color lineColor, Color axisColor)
    {
        axes.GetComponent<Image>().color = axisColor;
        foreach(Transform t in graphArea) { t.GetComponent<Image>().color = lineColor; }
    }

    public void ClearGraph()
    {
        foreach(Transform child in graphArea)
        { if(child.gameObject != null) { Destroy(child.gameObject); } }
        graphArea.GetComponent<HorizontalLayoutGroup>().enabled = true;
    }

    public void RevealGraph()
    {
        Color pointColor = Color.black;
        if (graphArea.childCount > 0)
        { pointColor = graphArea.GetChild(0).GetComponent<Image>().color; }

        Color transparent = new Color(0f, 0f, 0f, 0f);
        CustomizeGraph(transparent, axes.GetComponent<Image>().color);

        StartCoroutine(RevealPointsOverTime(pointColor));
    }
    private IEnumerator RevealPointsOverTime(Color pointColor)
    {
        float waitTime = 2f;
        float minStepTime = 1f / 30; // technical constraint - 60 fps
        float stepTime = waitTime / graphArea.childCount;
        int maxSteps = Mathf.FloorToInt(waitTime / minStepTime);

        int numSteps = graphArea.childCount;
        int numChildrenPerStep = 1;
        if (numSteps > maxSteps)
        {
            numSteps = maxSteps;
            numChildrenPerStep = Mathf.CeilToInt(graphArea.childCount / maxSteps);
        }

        for (int step = 0; step < numSteps; step++)
        {
            int childLimit = (step + 1) * numChildrenPerStep;
            if (step == numSteps - 1) { childLimit = graphArea.childCount; }
            // look ahead, if we are at last step, do rest now

            for (int child = step * numChildrenPerStep; child < childLimit; child++)
            {
                if (child >= graphArea.childCount) { yield break; }
                graphArea.GetChild(child).GetComponent<Image>().color = pointColor;
            }

            yield return new WaitForSecondsRealtime(stepTime);
        }
    }

    public int MaxValue(int[] intArray)
    {
        var max = intArray[0];
        for (int i = 1; i < intArray.Length; i++)
        {
            if (intArray[i] > max)
            {
                max = intArray[i];
            }
        }
        return max;
    }

    public float MaxValue(float[] floatArray)
    {
        var max = floatArray[0];
        for (int i = 1; i < floatArray.Length; i++)
        {
            if (floatArray[i] > max)
            {
                max = floatArray[i];
            }
        }
        return max;
    }
}
