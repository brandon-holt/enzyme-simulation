using UnityEngine;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    [SerializeField] private Transform menu;

    [SerializeField] private float scaleTime;

    [SerializeField] private GameObject blur;

    [SerializeField] private Button done;

    private Coroutine[] coroutines;

    private void Awake() { coroutines = new Coroutine[menu.childCount]; CheckName(); }

    public void Load(string level) { Global.load.LoadLevel(level); }

    public void SetMode(string mode) { Global.sim.mode = mode; }

    public void SetFileType(string fileType) { Global.fileType = fileType; }

    public void QuitGame() { Application.Quit(); }

    public void Grow(Transform t)
    {
        if (coroutines[t.GetSiblingIndex()] != null) { StopCoroutine(coroutines[t.GetSiblingIndex()]); }
        coroutines[t.GetSiblingIndex()] = StartCoroutine(Global.Scale(t, 2f, scaleTime));
    }

    public void Shrink(Transform t)
    {
        if (coroutines[t.GetSiblingIndex()] != null) { StopCoroutine(coroutines[t.GetSiblingIndex()]); }
        coroutines[t.GetSiblingIndex()] = StartCoroutine(Global.Scale(t, 1f, scaleTime));
    }

    private void CheckName()
    {
        if (PlayerPrefs.GetString("name", "") != "") { ToggleBlur(false); }
        else { ToggleBlur(true); done.interactable = false; }
    }

    public void UpdateName(string n)
    {
        PlayerPrefs.SetString("name", n);
        if (n.Length > 2) { done.interactable = true; }
        else { done.interactable = false; }
    }

    public void ToggleBlur(bool on) { blur.SetActive(on); }
}
