using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using MEC;
using System.Collections.Generic;

public class CustomizeSubstrate : MonoBehaviour
{
    [SerializeField] private GameObject substratePrefab;
    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private ColorSlider[] colorSliders;
    [SerializeField] private TextMeshProUGUI typeText, amountText;

    private EventSystem eventSystem;

    private bool isYES2;
    private int amount;

    private Substrate substrate;

    private void Awake()
    {
        substrate = Instantiate(substratePrefab, transform).GetComponent<Substrate>();

        substrate.transform.SetAsFirstSibling();

        eventSystem = EventSystem.current.GetComponent<EventSystem>();
    }

    private void Start()
    {
        substrate.GetInfo(out Color[] colorAB, out string subName, out isYES2, out amount);

        UpdateTypeButton();

        amountText.text = amount.ToString();
    }

    public void UpdateName()
    {
        substrate.SetName(inputField.text);
    }

    public void OpenPanel(int index)
    {
        if (!isYES2) { index = 0; }

        Timing.RunCoroutine(_ListenForColor(colorSliders[index].gameObject, index).CancelWith(gameObject));
    }

    private IEnumerator<float> _ListenForColor(GameObject slider, int index)
    {
        slider.SetActive(true);

        eventSystem.SetSelectedGameObject(slider);

        while (slider.activeSelf)
        {
            substrate.SetColor(colorSliders[index].color, index);

            GameObject current = eventSystem.currentSelectedGameObject;
            if (current == null) { slider.SetActive(false); }
            else if (current.name == "close") { slider.SetActive(false); }
            else if (current.name == "primary" || current.name == "secondary")
            {
                if (current.transform.GetChild(0).gameObject != slider) { slider.SetActive(false); }
            }

            yield return Timing.WaitForOneFrame;
        }
    }

    public void SwitchType()
    {
        isYES2 = !isYES2;
        UpdateTypeButton();
        substrate.SetType(isYES2);
        colorSliders[1].gameObject.SetActive(false);
    }

    private void UpdateTypeButton()
    {
        if (isYES2) { typeText.text = "YES<sup>2"; }
        else { typeText.text = "YES"; }
    }

    public void UpdateAmount(float amt)
    {
        substrate.SetAmount(Mathf.RoundToInt(amt));
        amountText.text = amt.ToString();
    }

    public void DeleteSubstrate()
    {
        if (transform.parent.childCount == 2) { return; }
        Destroy(gameObject);
    }
}
