using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System.IO;
using System.Collections.Generic;

public class Global : MonoBehaviour
{
    public static Load load;
    public static Simulate sim;
    public static Music music;
    public static Options options;

    public static string fileType, lastSavedFile, latestLeaderboards, leaderboardDownloadMessage;
    public static int versionCode = 1;
    public static bool uploadingScore, uploadingFile;
    public static bool downloadingScore, downloadingFile;

    public static class LastClassificationResults
    {
        public static float[] aurocValues;
        public static int repeats, numGroups;
        public static List<float[,]> meanSubValues = new List<float[,]>(); // List = round -> int[group, substrate]
    }

    public static int[] RandPerm(int length)
    {
        int[] randperm = new int[length];
        for (int i = 0; i < length; i++) { randperm[i] = i; }

        // shuffles the ordered indicies like a deck of cards
        for (int i = 0; i < length; i++)
        {
            int rnd = Random.Range(0, length);
            int temp = randperm[rnd];
            randperm[rnd] = randperm[i];
            randperm[i] = temp;
        }

        return randperm;
    }

    public static float ColorDist(Color a, Color b)
    {
        float r2 = Mathf.Pow(a.r - b.r, 2);
        float g2 = Mathf.Pow(a.g - b.g, 2);
        float b2 = Mathf.Pow(a.b - b.b, 2);
        return Mathf.Sqrt(r2 + g2 + b2);
    }

    public static IEnumerator Scale(Transform item, float target, float time)
    {
        float currVel = 0f;

        while (Mathf.Abs(item.localScale.x - target) > 0.01f)
        {
            float scale = Mathf.SmoothDamp(item.localScale.x, target, ref currVel, time);

            item.localScale = new Vector3(scale, scale, scale);

            yield return new WaitForSeconds(0.01f);
        }

        item.localScale = new Vector3(target, target, target);
    }

    public static int[] IntArray(int size)
    {
        int[] result = new int[size];

        for (int i = 0; i < size; i++) { result[i] = i; }

        return result;
    }
    public static int[] IntArray(int start, int finish)
    {
        List<int> intList = new List<int>();

        for (int i = start; i < finish; i++) { intList.Add(i); }

        return intList.ToArray();
    }

    public static double Sum(double[] array)
    {
        double result = 0d;
        for (int i = 0; i < array.Length; i++) { result += array[i]; }
        return result;
    }
    public static float Sum(float[] array)
    {
        float result = 0f;
        for (int i = 0; i < array.Length; i++) { result += array[i]; }
        return result;
    }

    // Networking functions

    public static IEnumerator DownloadFromLeaderboard(string leaderboard, int top)
    {
        downloadingScore = true;

        leaderboardDownloadMessage = "downloading data from leaderboards...";

        string URL = "https://synvoyity.com/enzyme_server/Leaderboard.php?act=get"
            + "&leaderboard=" + leaderboard + "&top=" + top.ToString();

        UnityWebRequest w = UnityWebRequest.Get(URL);
        w.SetRequestHeader("Cache-Control", "max-age=0, no-cache, no-store"); w.SetRequestHeader("Pragma", "no-cache");
        yield return w.SendWebRequest();

        leaderboardDownloadMessage = "";

        if (w.isNetworkError) { leaderboardDownloadMessage = "Network error."; }
        else { leaderboardDownloadMessage = ""; Debug.Log("Leaderboard downloaded successfully"); }

        latestLeaderboards = w.downloadHandler.text;

        downloadingScore = false;
    }

    public static IEnumerator UploadToLeaderboard(string leaderboard, float[] scores, string file)
    {
        uploadingScore = true;

        string URL = "https://synvoyity.com/enzyme_server/Leaderboard.php?name="
            + PlayerPrefs.GetString("name").ToString() + "&leaderboard=" + leaderboard
            + "&comp=" + scores[0].ToString("F0") + "&auc=" + scores[1].ToString("F0")
            + "&score=" + scores[2].ToString("F0") + "&file=" + file + "&version=" + versionCode;

        UnityWebRequest w = UnityWebRequest.Get(URL);
        w.SetRequestHeader("Cache-Control", "max-age=0, no-cache, no-store"); w.SetRequestHeader("Pragma", "no-cache");
        yield return w.SendWebRequest();

        if (w.isNetworkError) { Debug.Log("Network error."); }
        else { Debug.Log("Score uploaded successfully"); }

        uploadingScore = false;
    }

    public static IEnumerator UploadSaveFile(string fileName) // file name to save as on server
    {
        uploadingFile = true;

        //convert the local save file to be ready for upload
        byte[] levelData = File.ReadAllBytes(Application.persistentDataPath + "/" + fileName);

        WWWForm form = new WWWForm();
        form.AddField("action", "level upload");
        form.AddField("file", "file");
        form.AddBinaryData("file", levelData, fileName, null);
        UnityWebRequest w = UnityWebRequest.Post("https://synvoyity.com/enzyme_server/Library.php", form);

        yield return w.SendWebRequest();
        if (w.error != null) { Debug.Log(w.error); }
        else if (w.uploadProgress == 1f && w.isDone) { Debug.Log("file upload complete"); }
        else { Debug.Log("upload wasn't finished"); }

        uploadingFile = false;
    }


    public static IEnumerator DownloadSaveFile(string fileName)
    {
        downloadingFile = true;

        UnityWebRequest w2 = new UnityWebRequest("https://synvoyity.com/enzyme_server/libraries/" + fileName, UnityWebRequest.kHttpVerbGET);
        w2.SetRequestHeader("Cache-Control", "max-age=0, no-cache, no-store"); w2.SetRequestHeader("Pragma", "no-cache");
        w2.downloadHandler = new DownloadHandlerBuffer();
        yield return w2.SendWebRequest();

        if (w2.error != null) { Debug.Log(w2.error); }
        else
        {
            //then if the retrieval was successful, validate its content to ensure the level file integrity is intact
            if (w2.downloadHandler.text != null && w2.downloadHandler.text != "")
            {
                Debug.Log("Save file downloaded.");
                byte[] data = w2.downloadHandler.data;

                // save to local machine
                File.WriteAllBytes(Application.persistentDataPath + "/" + fileName, data);

                w2.downloadHandler.Dispose();
            }
            else { Debug.Log("Save File " + fileName + " is Empty"); }
        }

        // load the file

        downloadingFile = false;

    }
}



