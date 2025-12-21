using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Editor
{
    public class EditorSceneRestarter : MonoBehaviour
    {
#if UNITY_EDITOR
        #region RESTARTING SCENES
        [MenuItem("Debugging Tools/Restart Current Scene #R")]
        private static void RestartCurrentScene()
        {
            RestartScene(SceneManager.GetActiveScene().buildIndex);
        }

        [MenuItem("Debugging Tools/Load Scene 0 #R")]
        private static void LoadSceneZero()
        {
            RestartScene(0);
        }

        private static void RestartScene(int sceneIndex)
        {
            if (!EditorApplication.isPlaying) return;

            SceneManager.LoadScene(sceneIndex);
        }
        #endregion

        #region AUTO REFRESH
        //This is called when you click on the 'Tools/Auto Refresh' and toggles its value
        [MenuItem("Debugging Tools/Auto Refresh")]
        static void AutoRefreshToggle()
        {
            var status = EditorPrefs.GetInt("kAutoRefresh");
            if (status == 1)
                EditorPrefs.SetInt("kAutoRefresh", 0);
            else
                EditorPrefs.SetInt("kAutoRefresh", 1);
        }


        //This is called before 'Tools/Auto Refresh' is shown to check the current value
        //of kAutoRefresh and update the checkmark
        [MenuItem("Debugging Tools/Auto Refresh", true)]
        static bool AutoRefreshToggleValidation()
        {
            var status = EditorPrefs.GetInt("kAutoRefresh");
            if (status == 1)
                Menu.SetChecked("Debugging Tools/Auto Refresh", true);
            else
                Menu.SetChecked("Debugging Tools/Auto Refresh", false);
            return true;
        }
        #endregion
#endif
    }
}