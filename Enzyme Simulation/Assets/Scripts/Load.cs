using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Load : MonoBehaviour
{
    [SerializeField] private GameObject content;

    private static Load instance;

    public string lastScene;

    void Awake()
    {
        if (instance == null)
        {
            DontDestroyOnLoad(this); instance = this;
            Global.load = this;
        }
        else if (gameObject != null)
        {
            Destroy(gameObject);
        }
    }

    public void LoadLevel(string level) { StartCoroutine(Loading(level)); }

    private IEnumerator Loading(string level)
    {
        if (Global.options.group.activeSelf) { Global.options.ToggleGroup(); }

        lastScene = SceneManager.GetActiveScene().name;

        AsyncOperation operation = SceneManager.LoadSceneAsync(level);

        content.SetActive(true);

        while (!operation.isDone)
        {
            yield return null;
        }

        content.SetActive(false);
    }
}
