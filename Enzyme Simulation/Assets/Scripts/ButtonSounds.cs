using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonSounds : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler
{
    [SerializeField] private int hoverIndex, clickIndex;
    private AudioSource hover, click;
    private Music music;

    private void Awake()
    {
        music = GameObject.Find("Music").GetComponent<Music>();
    }

    private void Start()
    {
        if (clickIndex == 0) { clickIndex = 1; }
        click = music.transform.Find("sounds").transform.GetChild(clickIndex).GetComponent<AudioSource>();
        if (hoverIndex == -1) { return; }
        hover = music.transform.Find("sounds").transform.GetChild(hoverIndex).GetComponent<AudioSource>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (hover != null) { hover.Play(); }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (click != null) { click.Play(); }
    }
}
