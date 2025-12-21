using UnityEngine;

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
        //SceneManager.LoadScene(this.name);
        SceneLoader.Instance.LoadScene(this);
    }

    public void ReloadCurrentScene()
    {
        SceneLoader.Instance.ReloadCurrentScene();
    }
}