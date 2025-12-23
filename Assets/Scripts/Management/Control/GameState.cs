using UnityEngine;

[CreateAssetMenu(fileName = "GameState", menuName = "Misc/Game State", order = 1)]
public class GameState : ScriptableObject
{
    public bool isPlayable = true;
    public bool allowPausing = true;
    public bool stopTime = false;

    //public enum GameState
    //{
    //    Initializing, // Initializing assets or whatever
    //    Tutorial, // Seeing how to play if necessary
    //    Playing, // Actual game running
    //    Results, // Game Over
    //}

    public void SetManagerGameState()
    {
        GameStateManager.Instance.SetGameState(this);
    }

    public void SetManagerPausedState(bool paused)
    {
        GameStateManager.Instance.SetPausedState(paused);
    }

    public void SetNormalTime(float timeScale)
    {
        GameStateManager.Instance.SetNormalTimeScale(timeScale);
    }
}