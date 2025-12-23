using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OptionsSliderUI : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private Image fill;
    [SerializeField] private FloatSetting floatSettingsData;
    [SerializeField] private TextMeshProUGUI percentageText;
    [SerializeField] private string percentageFormat;

    private void OnEnable()
    {
        slider.onValueChanged.AddListener(OnValueChanged); ;
    }

    private void OnDisable()
    {
        slider.onValueChanged.RemoveListener(OnValueChanged);
    }

    private void Start()
    {
        slider.value = floatSettingsData.Value;
        OnValueChanged(floatSettingsData.Value);
    }

    private void OnValueChanged(float amount)
    {
        float normalizedValue = slider.maxValue > 0 ? amount / slider.maxValue : 0;

        floatSettingsData.SetTargetValue(amount);
        floatSettingsData.UpdateValue();

        fill.fillAmount = normalizedValue;
        percentageText.text = string.Format(percentageFormat, amount);
    }
}
