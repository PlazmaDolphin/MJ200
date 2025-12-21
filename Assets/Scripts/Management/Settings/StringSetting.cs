using UnityEngine;

[CreateAssetMenu(menuName = "Settings/String Setting")]
public class StringSetting : SettingsData<string>
{
    // ints can be reused for bools, as 0 means false and 1 means true
    protected override string GetSavedValue()
    {
        return PlayerPrefs.GetString(Key, defaultValue);
    }

    protected override void SaveValue()
    {
        PlayerPrefs.SetString(Key, Value);
    }
}