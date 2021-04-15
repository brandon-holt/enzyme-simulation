using UnityEngine;
using TMPro;

public class NumberOutOfTotal : MonoBehaviour
{
    [SerializeField] private Transform content;
    private TextMeshProUGUI text;
    private int max;

    private void Awake()
    {
        string type = transform.parent.parent.name;
        text = GetComponent<TextMeshProUGUI>();
        if (type == "enzymes") { max = Global.sim.limit_enzs; }
        if (type == "substrates") { max = Global.sim.limit_subs; }
    }

    private void Update()
    {
        text.text = "(" + (content.childCount - 1).ToString() + "/" + max + ")";
    }
}
