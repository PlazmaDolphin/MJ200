using System;
using UnityEngine;

public abstract class SettingsData<T> : ScriptableObject
{
    [SerializeField] protected string key;
    [SerializeField] protected T defaultValue;
    [SerializeField] protected T currentValue;
    [ReadOnly, SerializeField] protected T targetValue;

    public event Action<T> OnValueChanged;

    public string Key => key;
    public T Value => currentValue;

    private void OnValidate()
    {
        string targetName = this.name;

        // Split by space, capitalize each word except the first
        string[] words = targetName.Split(' ', System.StringSplitOptions.RemoveEmptyEntries);

        if (words.Length == 0)
        {
            key = "";
            return;
        }

        // Start with the first word, first letter lowercased
        string result = char.ToLower(words[0][0]) + words[0].Substring(1);

        // Capitalize the first letter of each subsequent word
        for (int i = 1; i < words.Length; i++)
        {
            if (words[i].Length > 0)
            {
                result += char.ToUpper(words[i][0]) + words[i].Substring(1);
            }
        }

        key = result;
    }

    public void ResetToDefault()
    {
        if (!Equals(currentValue, targetValue))
        {
            return;
        }

        SetTargetValue(defaultValue);
        UpdateValue();
    }

    public void SetTargetValue(T newValue)
    {
        targetValue = newValue;
    }

    public void UpdateValue()
    {
        if (!Equals(currentValue, targetValue))
        {
            currentValue = targetValue;
            SaveValue();
            OnValueChanged?.Invoke(targetValue);
        }
    }

    protected abstract T GetSavedValue();
    protected abstract void SaveValue();
}
