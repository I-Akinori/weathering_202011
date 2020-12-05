using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class SimulationSpeedSlider : MonoBehaviour
{
    static public float SliderValue = 0.0f;

    [SerializeField] Slider slider;

    void Start()
    {
        Slider slider = this.GetComponent<Slider>();
        slider.onValueChanged.AddListener(OnSlide);
    }

    void OnSlide(float value)
    {
        SliderValue = value;
    }
}