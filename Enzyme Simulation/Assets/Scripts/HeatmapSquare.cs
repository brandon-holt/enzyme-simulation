using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class HeatmapSquare : MonoBehaviour
{
    private float Kon, Koff, Kcat;
    [HideInInspector] public float kcatkm;
    [HideInInspector] public int[] indicies = new int[3];

    private SetKinetics setKinetics;
    [SerializeField] private GameObject constantsPanel;
    [SerializeField] private TextMeshProUGUI[] pholders;

    private void Awake()
    {
        setKinetics = GameObject.Find("SetKinetics").GetComponent<SetKinetics>();
        GetComponent<Button>().onClick.AddListener(OpenConstantsPanel);
    }

    private void Start() { UpdateColor(); }

    public void UpdateColor()
    {
        kcatkm = Kcat * Kon / (Koff + Kcat);

        if (kcatkm > setKinetics.maxValue)
        {
            setKinetics.maxValue = kcatkm;
            setKinetics.UpdateAllColors();
        }
        float maxVal = setKinetics.maxValue;

        float frac = kcatkm / maxVal;
        if (float.IsNaN(frac)) { frac = 0f; }
        float r = setKinetics.colorRange[0].r + (frac * (setKinetics.colorRange[1].r - setKinetics.colorRange[0].r));
        float g = setKinetics.colorRange[0].g + (frac * (setKinetics.colorRange[1].g - setKinetics.colorRange[0].g));
        float b = setKinetics.colorRange[0].b + (frac * (setKinetics.colorRange[1].b - setKinetics.colorRange[0].b));
        GetComponent<Image>().color = new Color(r, g, b);
    }

    public void SetKon(float val) { Kon = val; }
    public void SetKoff(float val) { Koff = val; }
    public void SetKcat(float val) { Kcat = val; }
    public void SetKon(string val)
    {
        if (float.TryParse(val, out float result))
        {
            if (Mathf.Abs(result) == Kon) { return; }
            Kon = Mathf.Abs(result);
            setKinetics.simulate.Kon[indicies[0], indicies[1], indicies[2]] = Kon;
        }
    }
    public void SetKoff(string val)
    {
        if (float.TryParse(val, out float result))
        {
            if (Mathf.Abs(result) == Koff) { return; }
            Koff = Mathf.Abs(result);
            setKinetics.simulate.Koff[indicies[0], indicies[1], indicies[2]] = Koff;
        }
    }
    public void SetKcat(string val)
    {
        if (float.TryParse(val, out float result))
        {
            if (Mathf.Abs(result) == Kcat) { return; }
            Kcat = Mathf.Abs(result);
            setKinetics.simulate.Kcat[indicies[0], indicies[1], indicies[2]] = Kcat;
        }
    }

    public void GetKinetics(out float kon, out float koff, out float kcat)
    {
        kon = Kon;
        koff = Koff;
        kcat = Kcat;
    }

    private void CheckValueChanged(string val, out bool changed)
    {
        changed = false;

        if (float.TryParse(val, out float result))
        { if (Mathf.Abs(result) == Kon) { return; } }

        if (float.TryParse(val, out result))
        { if (Mathf.Abs(result) == Kcat) { return; } }

        changed = true;
    }

    public void OpenConstantsPanel()
    {
        if (setKinetics.paintControls.paintMode) { return; }

        SetPlaceholder(Kon.ToString("F1"), 0);
        SetPlaceholder(Koff.ToString("F1"), 1);
        SetPlaceholder(Kcat.ToString("F1"), 2);

        StartCoroutine(WaitForClose());
    }

    public void PaintValue()
    {
        if (!Input.GetMouseButton(0) || !setKinetics.paintControls.paintMode) { return; }

        CheckValueChanged(setKinetics.paintControls.valueText.text, out bool changed);
        if (!changed) { return; }

        SetKon(setKinetics.paintControls.valueText.text);
        SetKcat(setKinetics.paintControls.valueText.text);

        setKinetics.paintControls.ShowRunningTotal();
        setKinetics.paintControls.runningTotalPainted += 1;

        UpdateColor();
    }

    private void SetPlaceholder(string val, int index) { pholders[index].text = val; }

    private IEnumerator WaitForClose()
    {
        constantsPanel.SetActive(true);

        EventSystem eventSystem = EventSystem.current.GetComponent<EventSystem>();

        while (true)
        {
            GameObject g = eventSystem.currentSelectedGameObject;
            if (g == null) { break; }
            else if (g != gameObject && g.name != "input") { break; }

            yield return null;
        }

        constantsPanel.SetActive(false);
    }
}
