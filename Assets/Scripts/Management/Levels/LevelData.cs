using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu(fileName = "Level", menuName = "Level/Level Data")]
public class LevelData : ScriptableObject
{
    [Header("Level References")]
    public string levelID;
    public SceneField sceneField;
    public bool isPlayableLevel = true;

    private void OnValidate()
    {
        levelID = this.name;
    }

    public void LoadScene()
    {
        SceneManager.LoadScene(sceneField.SceneName);
        //SceneLoader.Instance.LoadScene(this);
    }

    public void ReloadCurrentScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        //SceneLoader.Instance.ReloadCurrentScene();
    }
}