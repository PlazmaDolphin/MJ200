using UnityEngine;

[CreateAssetMenu(fileName = "Game Controller", menuName = "Misc/Game Controller")]
public class GameController : ScriptableObject
{
    #region APPLICATION
    public void OpenURL(string url)
    {
        Debug.Log($"Player loaded url to {url}");

        if (!Application.isEditor)
        {
            Application.OpenURL(url);
        }
    }

    public void ExitGame()
    {
        Debug.Log($"Exited game");

        if (!Application.isEditor)
        {
            Application.Quit();
        }
    }

    #endregion

    #region PLAYER PREFS
    public void SavePlayerPrefs()
    {
        Debug.Log($"Saved Player Prefs");
        PlayerPrefs.Save();
    }

    public void ClearPlayerPrefs()
    {
        Debug.Log($"Cleared Player Prefs");
        PlayerPrefs.DeleteAll();
    }
    #endregion
}
