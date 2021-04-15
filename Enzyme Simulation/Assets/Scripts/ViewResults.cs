using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using System.IO;
using System;
using System.Text;
using TMPro;

public class ViewResults : MonoBehaviour
{
    [SerializeField] private ScrollRow graphsRow;
    [SerializeField] private GameObject export;
    [SerializeField] private TextMeshProUGUI backText;

    private void Start()
    {
        if (Application.platform == RuntimePlatform.WebGLPlayer && export != null) { Destroy(export); }

        if (Global.load.lastScene == "SavedResults") { backText.text = "Results"; }
        else if (Global.load.lastScene == "Simulate") { backText.text = "Simulate"; }
        else if (Global.load.lastScene == "Play") { backText.text = "Test Library"; }

        GenerateGraphs();
    }

    public void Back()
    {
        Global.load.LoadLevel(Global.load.lastScene);
    }

    private void GenerateGraphs()
    {
        
        for (int i = 0; i < Global.sim.ny1.Length - 1; i++) { graphsRow.AddOne(); }
        int maxProduct = 0;
        for (int i = 0; i < Global.sim.ny1.Length; i++)
        { maxProduct = Mathf.Max(Global.sim.py1[i].Last(), Global.sim.py2[i].Last(), maxProduct); }

        LineGraph[] lineGraphs = graphsRow.GetComponentsInChildren<LineGraph>();
        for (int i = 0; i < Global.sim.ny1.Length; i++)
        {
            int[] products;
            Substrate sub = lineGraphs[i].GetComponentInChildren<Substrate>();
            sub.SetColor(Global.sim.substrate_colors[i, 0], 0);
            if (Global.sim.ny1[i] > 0) // then YES
            {
                products = Global.sim.py1[i].ToArray();
                sub.SetType(false);
            }
            else // then YES^2
            {
                products = Global.sim.py2[i].ToArray();
                sub.SetType(true);
                sub.SetColor(Global.sim.substrate_colors[i, 1], 1);
            }
            lineGraphs[i].GenerateGraph(products, new string[3] { Global.sim.substrate_names[i], "Time (seconds)", "Products" }, maxProduct);
            Color lineColor = Global.sim.substrate_colors[i, 0];
            if (Global.ColorDist(lineColor, Color.white) < .5f) { lineColor = Color.black; }
            lineGraphs[i].CustomizeGraph(lineColor, Global.sim.substrate_colors[i, 1]);
            lineGraphs[i].RevealGraph();
        }
    }

    public void LoadLevel(string level)
    {
        Global.load.LoadLevel(level);
    }

    public void DeactivateButton(Button b)
    {
        b.interactable = false;
    }

    public void SetDescription(string description) { Global.sim.description = description; }

    public void Export()
    {
        string content = "Time (seconds)";

        for (int i = 0; i < Global.sim.ny1.Length; i++)
        {
            if (Global.sim.ny1[i] > 0) // YES
            {
                content += "," + Global.sim.substrate_names[i] + " Substrates,"
                    + Global.sim.substrate_names[i] + " Products";
            }
            else // YES2
            {
                content += "," + Global.sim.substrate_names[i] + " Substrates,"
                    + Global.sim.substrate_names[i] + " Intermediates,"
                    + Global.sim.substrate_names[i] + " Products";
            }
        }

        content += "\n";

        for (int ti = 0; ti < Global.sim.t.Count; ti++)
        {

            content += Global.sim.t[ti].ToString();

            for (int si = 0; si < Global.sim.ny1.Length; si++)
            {
                if (Global.sim.ny1[si] > 0) // YES
                {
                    content += "," + Global.sim.sy1[si][ti].ToString()
                        + "," + Global.sim.py1[si][ti].ToString();
                }
                else // YES2
                {
                    content += "," + Global.sim.sy2[si][ti].ToString()
                        + "," + Global.sim.iy2[si][ti].ToString()
                        + "," + Global.sim.py2[si][ti].ToString();
                }
            }

            content += "\n";
        }

        content += "\n\n\n";

        content += "Enzyme,Amount\n";
        for (int i = 0; i < Global.sim.enzyme_names.Length; i++)
        {
            content += Global.sim.enzyme_names[i];
            content += "," + Global.sim.ne[i].ToString() + "\n";
        }

        content += "\n";

        content += "Substrate,Type,Amount\n";
        for (int i = 0; i < Global.sim.substrate_names.Length; i++)
        {
            content += Global.sim.substrate_names[i];
            if (Global.sim.ny1[i] > 0)
            {
                content += "," + "YES";
                content += "," + Global.sim.ny1[i].ToString() + "\n";
            }
            else
            {
                content += "," + "YES2";
                content += "," + Global.sim.ny2[i].ToString() + "\n";
            }
           
        }


        ContentToCSV(content);
    }

    private void ContentToCSV(string content)
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine(content);
        string filePath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) +
            "/simulation_" + DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss") + ".csv";
        StreamWriter outStream = File.AppendText(filePath);
        outStream.WriteLine(sb);
        outStream.Close();
    }
}
