using UnityEngine;
using TMPro;

public class Well : MonoBehaviour
{
    private string date, file;
    private World world;

    [SerializeField] private TextMeshProUGUI dateText, descText;
    [SerializeField] private GameObject box;

    private void Awake()
    {
        world = GameObject.Find("World").GetComponent<World>();
    }

    public void SetInfo(string f)
    {
        file = f;
        string[] fileParts = f.Split('/');
        string[] nameParts = fileParts[fileParts.Length - 1].Split('_');
        string year = nameParts[1];
        string month = nameParts[2];
        string day = nameParts[3];
        string hour = nameParts[4];
        string min = nameParts[5];
        date = month + "/" + day + "/" + year + "\n" + hour + ":" + min;
        dateText.text = date;

        world.LoadWorld(file);
        descText.text = world.description;
    }

    public void ShowBox(bool show)
    {
        box.SetActive(show);
    }

    public void LoadFile()
    {
        world.LoadWorld(file);

        if (Global.fileType == "result")
        {
            Global.load.LoadLevel("ViewResults");
        }
        else if (Global.fileType == "library")
        {
            Global.sim.mode = "play";
            Global.load.LoadLevel("SetEnzymesSubstrates");
        }
    }

    public void DeleteFile()
    {
        world.DeleteWorld(file);

        Destroy(gameObject);
    }
}
