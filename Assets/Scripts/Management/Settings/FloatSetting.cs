using UnityEngine;

[CreateAssetMenu(menuName = "Settings/Float Setting")]
public class FloatSetting : SettingsData<float>
{
    protected override float GetSavedValue()
    {
        return PlayerPrefs.GetFloat(key, defaultValue);
    }

    protected override void SaveValue()
    {
        PlayerPrefs.SetFloat(key, Value);
    }
}
