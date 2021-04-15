using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SetKinetics : MonoBehaviour
{
    public Simulate simulate;

    [SerializeField] private Transform heatmap, substrates, enzymes, scalebar;

    [SerializeField] private GameObject squarePrefab, subPrefab, enzPrefab;

    public PaintControls paintControls;

    public Color[] colorRange = new Color[2];

    private RectTransform hm, sb;

    private Vector2 maxSize = new Vector2(1400f, 700f);
    private float originalSize = 100f, barSizeThreshold = 20f, barSize = 50f;
    public bool[] barMode = new bool[2];
    [SerializeField] private GameObject[] mapLabels;

    public float maxValue;

    private HeatmapSquare[] hmSquares;

    [SerializeField] private Toggle paintModeToggle;
    
    private void Awake()
    {
        simulate = Global.sim;
    }

    private void Start()
    {
        SetGridConstraints();

        simulate.InitializeParameters();

        StartCoroutine(GenerateHeatMap());
    }

    private void Update()
    {
        MonitorColors();
    }

    private void MonitorColors()
    {
        float newMax = GetMaxValue();

        if (newMax != maxValue)
        {
            maxValue = newMax;
            UpdateAllColors();
        }
    }

    public void UpdateAllColors()
    {
        if (hmSquares == null) { return; }

        SetScaleBarText(); // every time a value is changed

        foreach (HeatmapSquare h in hmSquares)
        {
            h.UpdateColor();
        }
    }

    public void SetAllValues(float value)
    {
        if (hmSquares == null) { return; }

        foreach (HeatmapSquare h in hmSquares)
        {
            h.SetKon(value.ToString());
            h.SetKcat(value.ToString());
        }

        UpdateAllColors();
    }

    private void SetGridConstraints()
    {
        int subCount = 0;
        for (int i = 0; i < simulate.ny1.Length; i++)
        {
            subCount += 1;
            if (simulate.ny2[i] > 0) { subCount += 1; }
        }
        GridLayoutGroup hm = heatmap.GetComponent<GridLayoutGroup>();
        GridLayoutGroup sb = substrates.GetComponent<GridLayoutGroup>();
        GridLayoutGroup en = enzymes.GetComponent<GridLayoutGroup>();

        hm.constraintCount = subCount;
        sb.constraintCount = subCount;
        en.constraintCount = simulate.ne.Length;

        Vector2 cellSize = originalSize * Vector2.one;
        cellSize.x = Mathf.Min(100f, maxSize.x / subCount);
        cellSize.y = Mathf.Min(100f, maxSize.y / simulate.ne.Length);

        hm.cellSize = cellSize;
        en.cellSize = new Vector2(cellSize.y, cellSize.y);
        if (cellSize.y < barSizeThreshold) { en.cellSize = new Vector2(barSize, cellSize.y); barMode[0] = true; }
        sb.cellSize = new Vector2(cellSize.x, cellSize.x);
        if (cellSize.x < barSizeThreshold) { sb.cellSize = new Vector2(cellSize.x, barSize); barMode[1] = true; }
        for (int i = 0; i < barMode.Length; i++) { mapLabels[i].SetActive(barMode[i]); }
    }

    private void PositionScaleBarSetColor()
    {
        hm = heatmap.GetComponent<RectTransform>();
        sb = scalebar.GetComponent<RectTransform>();

        Vector2 pos = Vector2.zero;
        pos.x = hm.anchoredPosition.x + hm.sizeDelta.x + 20f;
        pos.y = hm.anchoredPosition.y;
        sb.anchoredPosition = pos;

        scalebar.GetComponent<ScaleBar>().SetColors(colorRange[0], colorRange[1]);
    }

    public void SetScaleBarText()
    {
        string label = "Catalytic Efficiency (k<sub>cat</sub>/K<sub>M</sub>)";
        string max = maxValue.ToString("F1");

        scalebar.GetComponent<ScaleBar>().SetText("0", max, label);
    }

    private IEnumerator GenerateHeatMap()
    {
        paintModeToggle.interactable = false;

        yield return null;
        PositionScaleBarSetColor();

        int colCount = simulate.ny2.Length;
        foreach(int i in simulate.ny2) { if (i > 0) { colCount += 1; } }
        heatmap.GetComponent<GridLayoutGroup>().constraintCount = colCount;

        int counter = 0;
        for (int ei = 0; ei < simulate.ne.Length; ei++)
        {
            Enzyme enz = Instantiate(enzPrefab, enzymes).GetComponent<Enzyme>();
            enz.SetColor(simulate.enzyme_colors[ei]);
            enz.SetName(simulate.enzyme_names[ei], Color.clear);
            if (barMode[0]) { enz.SetBarMode(); }

            for (int si = 0; si < simulate.ny1.Length; si++)
            {
                if (ei == 0)
                {
                    Substrate sub = Instantiate(subPrefab, substrates).GetComponent<Substrate>();
                    if (simulate.ny2[si] > 0)
                    {
                        Color[] otherColors = new Color[2];
                        for (int i = 0; i < otherColors.Length; i++)
                        { otherColors[i] = new Color(simulate.substrate_colors[si, i].r, simulate.substrate_colors[si, i].g, simulate.substrate_colors[si, i].b, .2f); }
                        sub.SetType(true);
                        sub.SetName(simulate.substrate_names[si] + " A", Color.clear);
                        sub.SetColor(simulate.substrate_colors[si, 0], 0);
                        sub.SetColor(otherColors[1], 1);
                        Substrate sub2 = Instantiate(subPrefab, substrates).GetComponent<Substrate>();
                        sub2.SetType(true);
                        sub2.SetName(simulate.substrate_names[si] + " B", Color.clear);
                        sub2.SetColor(simulate.substrate_colors[si, 1], 1);
                        sub2.SetColor(otherColors[0], 0);
                        if (barMode[1]) { sub2.SetBarMode(1); }
                    }
                    else
                    {
                        sub.SetType(false);
                        sub.SetName(simulate.substrate_names[si], Color.clear);
                        sub.SetColor(simulate.substrate_colors[si, 0], 0);
                    }
                    if (barMode[1]) { sub.SetBarMode(0); }
                }

                NewSquare(si, ei, 0);
                if (simulate.ny2[si] > 0) { NewSquare(si, ei, 1); }

                counter += 1;
                if (counter % 10 == 0) { yield return null; }
            }
        }

        SetScaleBarText();

        hmSquares = heatmap.GetComponentsInChildren<HeatmapSquare>();

        paintModeToggle.interactable = true;
    }

    private void NewSquare(int si, int ei, int side)
    {
        HeatmapSquare hmsquare = Instantiate(squarePrefab, heatmap).GetComponent<HeatmapSquare>();
        hmsquare.SetKon(simulate.Kon[si, ei, side]);
        hmsquare.SetKoff(simulate.Koff[si, ei, side]);
        hmsquare.SetKcat(simulate.Kon[si, ei, side]);
        hmsquare.indicies = new int[3] { si, ei, side };
    }

    public void Back()
    {
        paintControls.SetMode(false);

        Global.load.LoadLevel("SetEnzymesSubstrates");
    }

    public void Continue()
    {
        paintControls.SetMode(false);

        if (simulate.mode == "sim") { Global.load.LoadLevel("Simulate"); }
        else if (simulate.mode == "play") { Global.load.LoadLevel("Play"); }
    }

    public float GetMaxValue()
    {
        float maxVal = 0f;
        foreach (HeatmapSquare h in heatmap.GetComponentsInChildren<HeatmapSquare>())
        {
            h.GetKinetics(out float kon, out float koff, out float kcat);
            float thisVal = kcat * kon / (koff + kcat);
            maxVal = Mathf.Max(maxVal, thisVal);
        }
        return maxVal;
    }
}
