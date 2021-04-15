using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Leaderboards : MonoBehaviour
{
    [SerializeField] private GameObject lbPrefab, borderPrefab;
    [SerializeField] private Transform content, scroll;
    [SerializeField] private int top;
    [SerializeField] private Color[] topColors, backColors;
    [SerializeField] private TextMeshProUGUI message;

    private void Start()
    {
        StartCoroutine(Download());
    }

    private IEnumerator Download()
    {
        StartCoroutine(Global.DownloadFromLeaderboard("scores", top));

        message.text = Global.leaderboardDownloadMessage;

        while (Global.downloadingScore) { yield return null; }

        message.text = Global.leaderboardDownloadMessage;

        string[] rows = Global.latestLeaderboards.Split('\n');

        int rank = 1;

        foreach (string r in rows)
        {
            if (string.IsNullOrEmpty(r)) { continue; }

            string[] cols = r.Split('/');

            LeaderboardEntry lbe = Instantiate(lbPrefab, content).GetComponent<LeaderboardEntry>();

            string name = cols[0];
            string comp = cols[1];
            string auc = cols[2];
            string score = cols[3];
            string dt = cols[4].Substring(0, 10);
            string file = cols[5];
            if (!int.TryParse(cols[6], out int version)) { version = 1; }
            
            lbe.SetValues(file, version, rank.ToString(), name, comp, auc, score, dt);

            // colors and borders for top ranks
            if (rank <= topColors.Length) { lbe.SetTextColor(topColors[rank - 1]); }

            if (rank <= backColors.Length)
            {
                Border border = Instantiate(borderPrefab, scroll).GetComponent<Border>();
                border.target = lbe.GetComponent<RectTransform>();
                border.transform.SetAsFirstSibling();
                border.GetComponent<Image>().color = backColors[rank - 1];
            }

            rank += 1;

            yield return null;
        }
    }
}
