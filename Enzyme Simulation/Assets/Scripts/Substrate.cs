using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class Substrate : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Image[] images;
    [SerializeField] private Sprite primaryYES, primaryYES2;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private GameObject hoverPrefab;
    private Transform canvas;
    private HoverPanel hp;
    private bool barMode;

    public Color[] colors = new Color[2]; // [primary, secondary]
    public string substrateName;
    public bool YES2;
    private int number;

    private void Awake() { RandomizeSubstrate(); canvas = GetComponentInParent<Canvas>().transform; }

    public void RandomizeSubstrate()
    {
        SetColor(new Color(Random.value, Random.value, Random.value), 0);
        SetColor(new Color(Random.value, Random.value, Random.value), 1);
        SetName("Substrate" + Random.Range(100, 1000).ToString());
        if (Random.value > .5f) { SetType(true); }
        else { SetType(false); }
        if (Global.sim.mode == "play") { SetAmount(500); }
        else { SetAmount(Random.Range(100, 300)); }
    }

    public void SetColor(Color color, int index)
    {
        colors[index] = color;
        images[index].color = color;
    }

    public void SetName(string subName)
    {
        substrateName = subName;
        nameText.text = subName;
    }

    public void SetName(string subName, Color color)
    {
        nameText.color = color;
        SetName(subName);
    }

    public void SetType(bool isYES2)
    {
        YES2 = isYES2;
        if (isYES2) { images[0].sprite = primaryYES2; }
        else { images[0].sprite = primaryYES; }
        images[1].gameObject.SetActive(isYES2);
    }

    public void SetAmount(int amount) { number = amount; }

    public void GetInfo(out Color[] colorAB, out string subName, out bool isYES2, out int amount)
    {
        colorAB = colors;
        subName = substrateName;
        isYES2 = YES2;
        amount = number;
    }

    public void SetBarMode(int index)
    {
        barMode = true;
        for (int i = 0; i < images.Length; i++)
        {
            images[i].sprite = null;
            images[i].color = new Color(images[i].color.r, images[i].color.g, images[i].color.b, 1f);
            if (i != index) { images[i].color = Color.clear; }
        }
        nameText.text = "";
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!barMode) { return; }
        hp = Instantiate(hoverPrefab, canvas).GetComponent<HoverPanel>();
        hp.MakeSubstrate(this);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!barMode) { return; }
        if (hp.gameObject != null) { Destroy(hp.gameObject); }
    }

}
