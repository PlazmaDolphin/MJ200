using UnityEngine;
using Debug = UnityEngine.Debug;

public class GameController : MonoBehaviour
{
    // Used to spawn things via other scripts
    private static GameController _spawned;

    public static GameController SpawnedInstance
    {
        get
        {
            if (_spawned == null)
            {
                GameObject newObject = new GameObject("Game Controller (Spawned)");
                var controller = newObject.AddComponent<GameController>();

                _spawned = Instantiate(controller);
                DontDestroyOnLoad(_spawned.gameObject);
            }

            return _spawned;
        }
    }

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
