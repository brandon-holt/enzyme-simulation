using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class ParameterManager : MonoBehaviour
{
    public float deltaStep = .01f;

    [SerializeField] private Slider slider;
    [SerializeField] private TMP_InputField input;
    [SerializeField] private Button up, down, simButton;
    [SerializeField] private TextMeshProUGUI tstepText, timeText, pjumpText, progText, currentTimeText;
    [SerializeField] private RectTransform progress, bar;

    private void Awake()
    {
        SetPJump(Global.sim.pJump * 100f);

        ChangeTStep(0);

        ChangeTMax(Global.sim.tMax.ToString("F0"));

        simButton.onClick.AddListener(StartSimulation);
    }

    public void SetPJump(float value)
    {
        pjumpText.text = value.ToString("F0") + "%";
        Global.sim.pJump = value / 100f;
    }

    public void ChangeTStep(int sign)
    {
        Global.sim.timeStep = Mathf.Max(Global.sim.timeStep + sign * deltaStep, deltaStep);
        tstepText.text = Global.sim.timeStep.ToString("F2") + " s timestep";
    }

    public void ChangeTMax(string input)
    {
        if (float.TryParse(input, out float result))
        {
            Global.sim.tMax = Mathf.Abs(result);
            timeText.text = Global.sim.tMax.ToString("F0") + " s";
        }
    }

    private void BlockParameters(bool block)
    {
        slider.interactable = !block;
        input.interactable = !block;
        up.interactable = !block;
        down.interactable = !block;
    }

    private void StartSimulation()
    {
        BlockParameters(true);

        simButton.GetComponentInChildren<TextMeshProUGUI>().text = "CANCEL";
        simButton.onClick.RemoveAllListeners();
        simButton.onClick.AddListener(StopSimulation);

        Global.sim.lessData = false;
        Global.sim.Activity();

        StartCoroutine(UpdateProgress());

    }

    private IEnumerator UpdateProgress()
    {
        float maxWidth = progress.sizeDelta.x;

        while (!Global.sim.finish)
        {
            bar.sizeDelta = new Vector2(Global.sim.max_progress * maxWidth, bar.sizeDelta.y);

            currentTimeText.text = Global.sim.currentTime.ToString("F1") + " s";

            progText.text = (100f * Global.sim.max_progress).ToString("F1") + "%";

            yield return null;
        }

        if (Global.sim.max_progress > .99f || Global.sim.currentTime > Global.sim.tMax)
        {
            progText.text = "Complete!";

            simButton.GetComponentInChildren<TextMeshProUGUI>().text = "VIEW RESULTS";
            simButton.onClick.RemoveAllListeners();
            simButton.onClick.AddListener(ViewResults);
        }

    }

    private void StopSimulation()
    {
        Global.sim.finish = true;

        BlockParameters(false);

        simButton.GetComponentInChildren<TextMeshProUGUI>().text = "Global.sim";
        simButton.onClick.RemoveAllListeners();
        simButton.onClick.AddListener(StartSimulation);
    }

    private void ViewResults()
    {
        Global.sim.finish = true;

        Global.load.LoadLevel("ViewResults");
    }

    public void Back()
    {
        Global.sim.finish = true;

        Global.load.LoadLevel("SetKinetics");
    }

}
