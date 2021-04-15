using System.Collections.Generic;
using UnityEngine;

public class PlayResults : MonoBehaviour
{
    [SerializeField] private ScrollRow graphsRow;
    [SerializeField] private string[] groupNames;
    [SerializeField] private Color[] groupColors;

    private void Start()
    {
        GenerateGraphs();
    }

    private void GenerateGraphs()
    {
        for (int i = 0; i < 2 * Global.LastClassificationResults.repeats - 1; i++)
        { graphsRow.AddOne(); }

        BarGraph[] barGraphs = graphsRow.GetComponentsInChildren<BarGraph>();

        int index = 0;

        for (int round = 0; round < Global.LastClassificationResults.repeats; round++)
        {
            for (int group = 0; group < Global.LastClassificationResults.numGroups; group++)
            {
                List<float> values = new List<float>();

                for (int sub = 0; sub < Global.LastClassificationResults.meanSubValues[round].GetLength(1); sub++)
                { values.Add(Global.LastClassificationResults.meanSubValues[round][group, sub]); }

                string title = "Round " + (round+1).ToString() + ": " + groupNames[group]
                    + " (AUROC=" + (100*Global.LastClassificationResults.aurocValues[round]).ToString("F0") + "%)";
                barGraphs[index].GenerateGraph(values.ToArray(), new string[3] { title, "Substrates", "Products" });

                barGraphs[index].CustomizeGraph(groupColors[group], Color.white);

                index += 1;
            }
        }
    }

    public void Load(string level) { Global.load.LoadLevel(level); }
}
