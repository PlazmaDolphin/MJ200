using UnityEngine;
using UnityEngine.Audio;

[CreateAssetMenu(menuName = "Settings/Audio Volume Setting")]
public class AudioVolumeSetting : FloatSetting
{
    public AudioMixer audioMixer;
    public string exposedParameter = "masterVol";

    protected override void SaveValue()
    {
        base.SaveValue();
        ApplyToMixer(Value);
    }

    private void OnEnable()
    {
        // Sync saved value when loaded
        currentValue = GetSavedValue();
        ApplyToMixer(currentValue);

        OnValueChanged += ApplyToMixer;
    }

    private void OnDisable()
    {
        OnValueChanged -= ApplyToMixer;
    }

    private void ApplyToMixer(float value)
    {
        if (audioMixer == null || string.IsNullOrEmpty(exposedParameter))
            return;

        // Convert 0–1 slider to decibel scale (-80 dB to 0 dB)
        float dB = Mathf.Log10(Mathf.Clamp(value, 0.0001f, 1f)) * 20f;
        audioMixer.SetFloat(exposedParameter, dB);
        //Debug.Log($"db {dB}; Value {Value}");
    }
}