using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class DrawingRangeSlider : MonoBehaviour
{
    static public float DrawingRange = 1.0f;

    [SerializeField] Slider slider;

    void Start()
    {
        Slider slider = this.GetComponent<Slider>();
        slider.onValueChanged.AddListener(OnSlide);
    }

    void OnSlide(float value)
    {
        DrawingRange = value;
    }
}
