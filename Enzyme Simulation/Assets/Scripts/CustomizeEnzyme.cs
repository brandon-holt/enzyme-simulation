using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using MEC;
using System.Collections.Generic;

public class CustomizeEnzyme : MonoBehaviour
{
    [SerializeField] private GameObject enzymePrefab;
    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private ColorSlider colorSlider;
    [SerializeField] private TextMeshProUGUI amountText;

    private EventSystem eventSystem;
    private int amount;

    private Enzyme enzyme;

    private void Awake()
    {
        enzyme = Instantiate(enzymePrefab, transform).GetComponent<Enzyme>();

        enzyme.transform.SetAsFirstSibling();

        eventSystem = EventSystem.current.GetComponent<EventSystem>();
    }

    private void Start()
    {
        enzyme.GetInfo(out Color color, out string subName, out amount);

        amountText.text = amount.ToString();
    }

    public void UpdateName()
    {
        enzyme.SetName(inputField.text);
    }

    public void OpenPanel()
    {
        Timing.RunCoroutine(_ListenForColor(colorSlider.gameObject).CancelWith(gameObject));
    }

    private IEnumerator<float> _ListenForColor(GameObject slider)
    {
        slider.SetActive(true);

        while (slider.activeSelf)
        {
            enzyme.SetColor(colorSlider.color);

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

    public void UpdateAmount(float amt)
    {
        enzyme.SetAmount(Mathf.RoundToInt(amt));
        amountText.text = amt.ToString();
    }

    public void DeleteEnzyme()
    {
        if (transform.parent.childCount == 2) { return; }
        Destroy(gameObject);
    }
}
