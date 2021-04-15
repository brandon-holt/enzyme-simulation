using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class Enzyme : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private GameObject hoverPrefab;
    private Transform canvas;
    private HoverPanel hp;
    private bool barMode;

    public Color color;
    public string enzymeName;
    private int number;

    private void Awake() { RandomizeEnzyme(); canvas = GetComponentInParent<Canvas>().transform; }

    public void RandomizeEnzyme()
    {
        SetColor(new Color(Random.value, Random.value, Random.value));
        SetName("Enzyme" + Random.Range(100, 1000).ToString());
        SetAmount(Random.Range(0, 100));
    }

    public void SetColor(Color c)
    {
        color = c;
        GetComponent<Image>().color = c;
    }

    public void SetName(string subName)
    {
        enzymeName = subName;
        nameText.text = subName;
    }

    public void SetName(string subName, Color color)
    {
        nameText.color = color;
        SetName(subName);
    }

    public void SetAmount(int amount) { number = amount; }

    public void GetInfo(out Color c, out string enzName, out int amount)
    {
        c = color;
        enzName = enzymeName;
        amount = number;
    }

    public void SetBarMode()
    {
        barMode = true;
        GetComponent<Image>().sprite = null;
        nameText.text = "";
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!barMode) { return; }
        hp = Instantiate(hoverPrefab, canvas).GetComponent<HoverPanel>();
        hp.MakeEnzyme(this);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!barMode) { return; }
        if (hp.gameObject != null) { Destroy(hp.gameObject); }
    }
}
