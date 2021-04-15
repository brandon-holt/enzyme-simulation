using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class PaintControls : MonoBehaviour
{
    public bool paintMode;
    public TextMeshProUGUI valueText;

    [SerializeField] private Slider slider;
    [SerializeField] private SetKinetics setKinetics;
    [SerializeField] private Image[] sliderImages;

    public Texture2D cursor;
    private CursorMode cursorMode = CursorMode.Auto;
    public Vector2 hotSpot = Vector2.zero;

    [SerializeField] private GameObject runningTotal;
    [SerializeField] private TextMeshProUGUI runningTotalText;
    public int runningTotalPainted;
    private bool runningTotalShowing;

    private void Start() { SetValue(slider.value); }

    public void SetMode(bool paintModeOn)
    {
        paintMode = paintModeOn;

        if (paintModeOn) { Cursor.SetCursor(cursor, hotSpot, cursorMode); }
        else { Cursor.SetCursor(null, Vector2.zero, cursorMode); }
    }

    public void SetValue(float value)
    {
        valueText.text = value.ToString("F1");
        float fraction = value / slider.maxValue;
        Color color = Color.Lerp(setKinetics.colorRange[0], setKinetics.colorRange[1], fraction);
        sliderImages[0].color = color;
        sliderImages[1].color = .6f * color;
        sliderImages[2].color = color;
        sliderImages[3].color = color;
    }

    public void ShowRunningTotal() { if (!runningTotalShowing) { StartCoroutine(RunningTotal()); } }

    private IEnumerator RunningTotal()
    {
        runningTotalShowing = true;

        SetRunningTotalWindowPosition();

        runningTotal.SetActive(true);

        runningTotalPainted = 0;

        while (Input.GetMouseButton(0))
        {
            SetRunningTotalWindowPosition();

            runningTotalText.text = runningTotalPainted.ToString();

            yield return null;
        }

        runningTotal.SetActive(false);

        runningTotalShowing = false;
    }

    private void SetRunningTotalWindowPosition()
    {
        runningTotal.transform.position
            = new Vector2(Input.mousePosition.x + 60f, Input.mousePosition.y + 150f);
    }
}

