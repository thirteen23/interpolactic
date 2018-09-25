using UnityEngine;
using UnityEngine.UI;

public class TimeScaleSlider : MonoBehaviour {

    [SerializeField]
    Slider slider;

    void Start()
    {
        slider.minValue = 0;
        slider.maxValue = 1;

        slider.value = Time.timeScale;

        slider.onValueChanged.AddListener(val => Time.timeScale = val);
    }
}
