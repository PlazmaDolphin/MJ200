using UnityEngine;

public abstract class SettingBase : ScriptableObject
{
    public abstract void Load(bool ignorePlayerPrefs);
    public abstract void UpdateValue();
    public abstract void ResetToDefault();
}
