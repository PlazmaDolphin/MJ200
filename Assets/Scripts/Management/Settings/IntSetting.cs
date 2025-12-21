using UnityEngine;

[CreateAssetMenu(menuName = "Settings/Int Setting")]
public class IntSetting : SettingsData<int>
{
    // ints can be reused for bools, as 0 means false and 1 means true
    protected override int GetSavedValue()
    {
        return PlayerPrefs.GetInt(Key, defaultValue);
    }

    protected override void SaveValue()
    {
        PlayerPrefs.SetInt(Key, Value);
    }

    public bool GetValueAsBool()
    {
        return Value == 1;
    }

    public void SeValueAsInvertedBool()
    {
        SetValueFromBool(!GetValueAsBool());
    }

    public void SetValueFromBool(bool value)
    {
        SetTargetValue(value ? 1 : 0);
    }
}
