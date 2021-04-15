using UnityEngine;
using UnityEngine.UI;

public class ColorSlider : MonoBehaviour
{
    [SerializeField] private Slider[] sliders = new Slider[3];

    [HideInInspector] public Color color;

    private void Awake() { UpdateValues(); }

    public void UpdateValues() { color = new Color(sliders[0].value, sliders[1].value, sliders[2].value); }
}
