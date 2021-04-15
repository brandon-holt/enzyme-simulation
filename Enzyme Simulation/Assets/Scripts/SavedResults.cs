using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Collections;
using TMPro;

public class SavedResults : MonoBehaviour
{

    [SerializeField] private ScrollRow plateRow;
    [SerializeField] private GameObject wellPrefab;
    [SerializeField] private TextMeshProUGUI title;
    [SerializeField] private Color[] plateColors;
    [SerializeField] private Color[] textColors;
    private int colorIndex;

    private void Start()
    {
        AdjustAppearance();

        StartCoroutine(LoadPlate(Global.fileType));
    }

    private IEnumerator LoadPlate(string type)
    {
        UpdatePlates();

        var info = new DirectoryInfo(Application.persistentDataPath);
        FileInfo[] fileInfo = info.GetFiles("*.enzyme");
        for (int i = 0; i < fileInfo.Length; i++)
        {
            string thisType = fileInfo[i].Name.Split('_')[0];

            if (thisType != type) { continue; }

            if ((i+1) % 96 == 1 && (i+1) > 96) { plateRow.AddOne(); UpdatePlates(); } // when we fill a plate

            int plateCount = plateRow.transform.Find("content").childCount;
            Transform plate = plateRow.transform.Find("content").GetChild(plateCount - 1);
            Well well = Instantiate(wellPrefab, plate.Find("wells")).GetComponent<Well>();

            well.SetInfo(fileInfo[i].FullName);

            yield return null;
        }
    }

    private void UpdatePlates()
    {
        foreach (Plate p in plateRow.GetComponentsInChildren<Plate>())
        { p.SetColors(plateColors[colorIndex], textColors[colorIndex]); }
    }

    private void AdjustAppearance()
    {
        if (Global.fileType == "result")
        {
            title.text = "Saved Results";
            title.color = Color.white;
            colorIndex = 0;
        }
        else if (Global.fileType == "library")
        {
            title.text = "Saved Libraries";
            title.color = Color.black;
            colorIndex = 1;
        }
    }

    public void Load(string level) { Global.load.LoadLevel(level); }

}
