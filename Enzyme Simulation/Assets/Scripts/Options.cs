using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Options : MonoBehaviour
{
    private static Options instance;
    public GameObject group, quit;
    [SerializeField] private TextMeshProUGUI muteText, fullText;
    [SerializeField] private Image icon;
    [SerializeField] private Sprite hamburger, ex;

    private void Awake()
    {
        if (instance == null) { DontDestroyOnLoad(this); instance = this; Global.options = this; }
        else if (gameObject != null) { Destroy(gameObject); }

        group.SetActive(false);

        if (Application.platform == RuntimePlatform.WebGLPlayer)
        {
            fullText.GetComponent<Button>().interactable = false;
            quit.GetComponent<Button>().interactable = false;
        }

        SetFullScreenText();
    }

    public void ToggleGroup()
    {
        group.SetActive(!group.activeSelf);
        if (group.activeSelf) { icon.sprite = ex; }
        else { icon.sprite = hamburger; }
    }

    public void ToggleMute()
    {
        if (muteText.text == "mute") { muteText.text = "unmute"; }
        else { muteText.text = "mute"; }
        AudioListener.volume = 1f - AudioListener.volume;
    }

    public void ToggleFull()
    {
        Resolution[] resolutions = Screen.resolutions;
        int w = resolutions[resolutions.Length - 1].width;
        int h = resolutions[resolutions.Length - 1].height;
        Screen.SetResolution(w, h, !Screen.fullScreen);

        SetFullScreenText();
    }

    private void SetFullScreenText()
    {
        if (Screen.fullScreen) { fullText.text = "windowed"; }
        else { fullText.text = "fullscreen"; }
    }

    public void Quit() { Application.Quit(); }

}
