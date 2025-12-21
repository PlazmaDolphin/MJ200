using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
#endif

/// <summary>
/// 
/// <br>Shows the property in inspector but greys it out and disable editing of it. </br>
/// <br>Can be used to indicate that the variable should not be edited in the inspector but it can be viewed for debugging.</br>
/// <br>Member variables need to be public OR have [SerializeField]</br>
/// <br> </br>
/// <br>How to use: Add [ReadOnly] to member variables, similar to [SerializeField] / [HideInInspector]</br>
/// </summary>
public class ReadOnlyAttribute : PropertyAttribute { }

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
public class ReadOnlyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        GUI.enabled = false;
        EditorGUI.PropertyField(position, property, label, true);
        GUI.enabled = true;
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUI.GetPropertyHeight(property, label, true);
    }
}

#endif