using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OptionsSliderUI : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private FloatSetting floatSettingsData;
    [SerializeField] private TextMeshProUGUI percentageText;
    [SerializeField] private string percentageFormat;

    private void OnEnable()
    {
        slider.value = floatSettingsData.Value / slider.maxValue;
        slider.onValueChanged.AddListener(OnValueChanged);
    }

    private void OnDisable()
    {
        slider.onValueChanged.RemoveListener(OnValueChanged);
    }

    private void OnValueChanged(float amount)
    {
        floatSettingsData.SetTargetValue(amount / slider.maxValue);
        floatSettingsData.UpdateValue();
        percentageText.text = string.Format(percentageFormat, amount);
    }
}
