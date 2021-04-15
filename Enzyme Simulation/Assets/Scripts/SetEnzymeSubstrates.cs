using UnityEngine;
using TMPro;

public class SetEnzymeSubstrates : MonoBehaviour
{
    [SerializeField] private Transform enzymesContent, substratesContent;
    private RectTransform enzRect, subRect;
    [SerializeField] private ScrollRow enzRow, subRow;
    [SerializeField] private TextMeshProUGUI title;
    [SerializeField] private GameObject clearEnzymesButton;

    private void Awake()
    {
        AdjustBasedOnMode(); // depending on mode, adjust things

        enzRect = enzymesContent.GetComponent<RectTransform>();
        subRect = substratesContent.GetComponent<RectTransform>();
    }

    private void AdjustBasedOnMode()
    {
        enzRow.maxQuantity = Global.sim.limit_enzs;
        subRow.maxQuantity = Global.sim.limit_subs;

        if (Global.sim.mode == "play")
        {
            title.text = "Customize Universal Substrate Library";
            clearEnzymesButton.SetActive(false);
            if (enzRow.gameObject != null) { Destroy(enzRow.gameObject); }
            UpdateSubstratesRow();
        }
        else if (Global.sim.mode == "sim")
        {
            title.text = "Customize Enzymes and Substrates";
            clearEnzymesButton.SetActive(true);
            UpdateEnzymesRow();
            UpdateSubstratesRow();
        }
    }

    private void Update()
    {
        AdjustPivot(enzRect);
        AdjustPivot(subRect);
    }

    private void AdjustPivot(RectTransform rt)
    {
        if (rt == null) { return; }
        if (rt.sizeDelta.x > 1600f) { rt.pivot = new Vector2(1f, .5f); }
        else { rt.pivot = .5f * Vector2.one; }
    }

    public void ResetList(Transform list)
    {
        for (int i = 1; i < list.childCount - 1; i++)
        {
            Destroy(list.GetChild(i).gameObject);
        }
    }

    private void UpdateEnzymesRow()
    {
        if (Global.sim.ne.Length > 0)
        {
            for (int i = 0; i < Global.sim.ne.Length - 1; i++) { enzRow.AddOne(); }
            Enzyme[] enzymes = enzymesContent.GetComponentsInChildren<Enzyme>();
            for (int i = 0; i < Global.sim.ne.Length; i++)
            {
                enzymes[i].SetName(Global.sim.enzyme_names[i]);
                enzymes[i].SetColor(Global.sim.enzyme_colors[i]);
                enzymes[i].SetAmount(Global.sim.ne[i]);
            }
        }
    }

    private void UpdateSubstratesRow()
    {
        if (Global.sim.ny1.Length > 0)
        {
            for (int i = 0; i < Global.sim.ny1.Length - 1; i++) { subRow.AddOne(); }
            Substrate[] substrates = substratesContent.GetComponentsInChildren<Substrate>();
            for (int i = 0; i < Global.sim.ny1.Length; i++)
            {
                if (Global.sim.ny1[i] > 0) // then YES
                {
                    substrates[i].SetType(false);
                    substrates[i].SetAmount(Global.sim.ny1[i]);
                }
                else // then YES^2
                {
                    substrates[i].SetType(true);
                    substrates[i].SetAmount(Global.sim.ny2[i]);
                    substrates[i].SetColor(Global.sim.substrate_colors[i, 1], 1);
                }
                substrates[i].SetColor(Global.sim.substrate_colors[i, 0], 0);
                substrates[i].SetName(Global.sim.substrate_names[i]);
            }
        }
    }

    public void SetInfo(string level)
    {
        if (Global.sim.mode == "sim") { SetEnzymeInfo(); }
        else if (Global.sim.mode == "play"
            && Global.load.lastScene != "SavedResults"
            && Global.load.lastScene != "Leaderboards")
        { StandardEnzymeInfo(); }

        SetSubstrateInfo();

        Load(level);
    }

    private void StandardEnzymeInfo()
    {
        if (Global.sim.enzyme_colors.Length != Global.sim.limit_enzs)
        {
            Color[] enzyme_colors = new Color[Global.sim.limit_enzs];
            for (int i = 0; i < enzyme_colors.Length; i++)
            {
                enzyme_colors[i] = new Color(Random.value, Random.value, Random.value);
            }
            Global.sim.enzyme_colors = enzyme_colors;
        }
        if (Global.sim.enzyme_names.Length != Global.sim.limit_enzs)
        {
            string[] enzyme_names = new string[Global.sim.limit_enzs];
            for (int i = 0; i < enzyme_names.Length; i++)
            {
                enzyme_names[i] = Global.sim.GetStandardEnzymeName(i);
            }
            Global.sim.enzyme_names = enzyme_names;
        }
        if (Global.sim.ne.Length != Global.sim.limit_enzs)
        {
            int[] ne = new int[Global.sim.limit_enzs]; // temp
            for (int i = 0; i < ne.Length; i++) { ne[i] = 100; }
            Global.sim.ne = ne;
        }
    }

    private void SetEnzymeInfo()
    {
        Enzyme[] enzymes = enzymesContent.GetComponentsInChildren<Enzyme>();
        int[] ne = new int[enzymes.Length];
        string[] enzyme_names = new string[enzymes.Length];
        Color[] enzyme_colors = new Color[enzymes.Length];
        for (int i = 0; i < enzymes.Length; i++)
        {
            enzymes[i].GetInfo(out Color color, out string enzName, out int amount);
            ne[i] = amount;
            enzyme_names[i] = enzName;
            enzyme_colors[i] = color;
        }
        Global.sim.ne = ne;
        Global.sim.enzyme_names = enzyme_names;
        Global.sim.enzyme_colors = enzyme_colors;
    }

    private void SetSubstrateInfo()
    {
        Substrate[] substrates = substratesContent.GetComponentsInChildren<Substrate>();
        int[] ny1 = new int[substrates.Length];
        int[] ny2 = new int[substrates.Length];
        string[] substrate_names = new string[substrates.Length];
        Color[,] substrate_colors = new Color[substrates.Length, 2];
        for (int i = 0; i < substrates.Length; i++)
        {
            substrates[i].GetInfo(out Color[] colorAB, out string subName, out bool isYES2, out int amount);
            if (isYES2)
            {
                ny1[i] = 0;
                ny2[i] = amount;
            }
            else
            {
                ny1[i] = amount;
                ny2[i] = 0;
            }
            substrate_names[i] = subName;
            substrate_colors[i, 0] = colorAB[0];
            substrate_colors[i, 1] = colorAB[1];
        }
        Global.sim.ny1 = ny1;
        Global.sim.ny2 = ny2;
        Global.sim.substrate_names = substrate_names;
        Global.sim.substrate_colors = substrate_colors;
    }

    public void Load(string level) { Global.load.LoadLevel(level); }
}
