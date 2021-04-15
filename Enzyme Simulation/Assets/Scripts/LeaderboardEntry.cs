using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.IO;

public class LeaderboardEntry : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI rankText, userText, crText, aucText, scoreText, dateText;
    private TextMeshProUGUI[] texts;

    [SerializeField] private Button getButton;

    [SerializeField] private Color[] colors;
    [SerializeField] private float[] compLimits, aucLimits, scoreLimits;

    private string file;

    private int version;

    private void Awake()
    {
        getButton.onClick.AddListener(DownloadLibrary);

        texts = new TextMeshProUGUI[6] { rankText, userText, crText, aucText, scoreText, dateText };
    }

    public void SetValues(string fileName, int versionCode, string rank, string username,
        string cr, string auc, string score, string date)
    {
        file = fileName;
        version = versionCode;
        rankText.text = rank;
        userText.text = username;
        crText.text = cr;
        aucText.text = auc;
        scoreText.text = score;
        dateText.text = date;

        // set colors based on value
        SetValueColor(crText, cr, compLimits, 2);
        SetValueColor(aucText, auc, aucLimits, 0);
        SetValueColor(scoreText, score, scoreLimits, 0);
    }

    private void SetValueColor(TextMeshProUGUI t, string val, float[] lims, int rev)
    {
        if (float.TryParse(val, out float result))
        {
            for (int i = 0; i < lims.Length; i++)
            {
                int ind = Mathf.Abs(rev - i);
                if (result < lims[i]) { t.color = colors[ind]; return; }
            }
        }
    }

    public void SetTextColor(Color color)
    {
        foreach (TextMeshProUGUI t in texts) { t.color = color; }
    }

    private void DownloadLibrary()
    {
        if (Global.downloadingFile) { return; }

        StartCoroutine(Downloading());
    }

    private IEnumerator Downloading()
    {
        // first check if we already have this file

        StartCoroutine(Global.DownloadSaveFile(file));

        getButton.interactable = false;

        getButton.GetComponentInChildren<TextMeshProUGUI>().text = ". . .";

        while (Global.downloadingFile) { yield return null; }

        Global.sim.mode = "play";

        World world = GameObject.Find("World").GetComponent<World>();

        world.LoadWorld(Application.persistentDataPath + "/" + file);

        Global.load.LoadLevel("SetEnzymesSubstrates");
    }
}
