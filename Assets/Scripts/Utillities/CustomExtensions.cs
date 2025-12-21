using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

#if UNITY_EDITOR
using UnityEditor;
#endif

public static class CustomExtensions
{
    #region MISC
    // Convert Color to Hexadecimal
    public static string ColorToHex(Color color)
    {
        // Convert each color component to a 0-255 integer and format to 2 hexadecimal characters
        int r = Mathf.RoundToInt(color.r * 255);
        int g = Mathf.RoundToInt(color.g * 255);
        int b = Mathf.RoundToInt(color.b * 255);
        int a = Mathf.RoundToInt(color.a * 255);

        // Format as a hexadecimal string
        if (a < 255) // Include alpha if it's not fully opaque
        {
            return $"#{r:X2}{g:X2}{b:X2}{a:X2}";
        }
        else // Exclude alpha if it's fully opaque
        {
            return $"#{r:X2}{g:X2}{b:X2}";
        }
    }

    // Helper method to check if a GameObject is in a specific LayerMask.
    public static bool IsInLayerMask(GameObject obj, LayerMask layerMask)
    {
        return (layerMask.value & (1 << obj.layer)) > 0;
    }

    public static bool ShowImageInCaseOfNull(Image image, Sprite sprite)
    {
        if (sprite != null)
        {
            image.sprite = sprite;
            return true;
        }
        else
        {
            image.enabled = false;
            return false;
        }
    }
    #endregion

    #region LISTS
    public static void RandomizeList<T>(List<T> list)
    {
        int n = list.Count;
        for (int i = n - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);
            (list[i], list[randomIndex]) = (list[randomIndex], list[i]); // Swap elements
        }
    }

    public static void DestroyItensList<T>(List<T> original) where T : Component
    {
        // Create a copy to avoid modifying the collection during iteration
        List<T> copy = new(original);

        foreach (var item in copy)
        {
            if (item != null)
            {
                GameObject.Destroy(item.gameObject);
            }
        }

        original.Clear();
    }
    #endregion

    #region STRINGS
    public static string AddSpacesToCamelCase(string text)
    {
        return Regex.Replace(text, "(?<!^)([A-Z])", " $1");
    }

    public static string RemoveWordFromString(string text, string wordToRemove)
    {
        string targetText = text;

        // Check if the input contains "move"
        if (targetText.ToLower().Contains(wordToRemove.ToLower()))
        {
            // Remove "move" part
            targetText = targetText.Replace(wordToRemove, "");
        }

        return targetText;
    }
    #endregion

#if UNITY_EDITOR
    #region EDITOR
    /// <summary>
    /// Draws a centered sprite preview in the inspector.
    /// </summary>
    /// <param name="sprite">The sprite to preview.</param>
    /// <param name="maxSize">Maximum preview size in pixels (width/height).</param>
    public static void DrawSpritePreview(Sprite sprite, string spriteName = "Sprite", float maxSize = 100f)
    {
        if (sprite == null)
        {
            EditorGUILayout.HelpBox($"No {spriteName.ToLower()} assigned, can't display preview.", MessageType.Warning);
            return;
        }

        GUILayout.Label($"{spriteName} Preview:", EditorStyles.miniLabel);

        float spriteWidth = sprite.rect.width;
        float spriteHeight = sprite.rect.height;
        float aspect = spriteWidth / spriteHeight;

        float width = maxSize;
        float height = maxSize;

        if (aspect > 1f)
            height = width / aspect;
        else
            width = height * aspect;

        Rect previewRect = GUILayoutUtility.GetRect(width, height, GUILayout.ExpandWidth(false));
        float xOffset = (EditorGUIUtility.currentViewWidth - width) / 2f;
        previewRect.x = xOffset;

        Rect texCoords = new Rect(
            sprite.rect.x / sprite.texture.width,
            sprite.rect.y / sprite.texture.height,
            sprite.rect.width / sprite.texture.width,
            sprite.rect.height / sprite.texture.height
        );

        GUI.DrawTextureWithTexCoords(previewRect, sprite.texture, texCoords, true);
    }
    #endregion
#endif
}
