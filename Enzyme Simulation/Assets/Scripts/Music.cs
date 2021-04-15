using System.Collections;
using UnityEngine;

public class Music : MonoBehaviour
{
    private AudioSource audioSource;
    private int trackNumber;
    private AudioClip[] songs;
    private static Music instance;

    private void Awake()
    {
        if (instance == null) { DontDestroyOnLoad(this); instance = this; Global.music = this; }
        else if (gameObject != null) { Destroy(gameObject); }
        audioSource = GetComponent<AudioSource>();
        songs = Resources.LoadAll<AudioClip>("Music");
    }

    private void Start()
    {
        StartCoroutine(MusicManager());
    }

    private IEnumerator MusicManager()
    {
        while (true)
        {
            if (!audioSource.isPlaying)
            {
                audioSource.clip = songs[trackNumber];
                audioSource.Play();
                trackNumber += 1;
                if (trackNumber > songs.Length - 1) { trackNumber %= songs.Length; }
            }

            yield return new WaitForSeconds(3f);
        }
    }
}
