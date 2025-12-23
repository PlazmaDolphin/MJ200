using System;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[DefaultExecutionOrder(-1)]
public class GameStateManager : MonoBehaviourSingleton<GameStateManager>
{
    public Action<bool> OnPausedStateChanged;
    public Action<GameState> OnGameStateChanged;

    [Space(20f), Header("State")]
    [SerializeField] private GameState initialGameState;
    [SerializeField, ReadOnly] private GameState gameState;
    [SerializeField, ReadOnly] private bool isPaused = false;
    public static float NormalTimeScale = 1f;

    // Perform this on awake
    private void Awake()
    {
        SetGameState(initialGameState);
    }

    #region PAUSING
    public void TogglePause()
    {
        //Debug.Log("Tried set pause");

        SetPausedState(!isPaused);
    }

    public void SetPausedState(bool pausedState)
    {
        if (isPaused == pausedState)
        {
            //Debug.LogWarning("Can't pause to the same paused value");
            return;
        }

        if (gameState == null)
        {
            Debug.LogError($"Can't change paused state, game state is null!");
            return;
        }

        if (!gameState.allowPausing)
        {
            //Debug.LogWarning($"Can't pause due to {gameState.name} game state prohibiting it");
            return;
        }

        isPaused = pausedState;

        SetTimeScale(isPaused ? 0f : NormalTimeScale);

        OnPausedStateChanged?.Invoke(isPaused);
        //Debug.Log($"Paused state changed to {isPaused}! Game state: {gameState.name}");
    }

    public bool GetIsPaused() => isPaused;
    #endregion

    #region GAME STATE
    public void SetGameState(GameState state)
    {
        if (state == null)
        {
            return;
        }

        if (gameState == state)
        {
            return;
        }

        SetTimeScale(state.stopTime ? 0f : NormalTimeScale);

        //Debug.Log($"<color=cyan>Game state set to {state.name}</color>");
        gameState = state;
        OnGameStateChanged?.Invoke(gameState);
    }

    private void SetTimeScale(float timeScale)
    {
        Time.timeScale = timeScale;
    }

    public void SetNormalTimeScale(float timeScale)
    {
        NormalTimeScale = timeScale;
        SetTimeScale(timeScale);
    }

    public GameState GetGameState() => gameState;
    //public bool CanPlay() => !isPaused && gameState.isPlayable && LevelLoadingManager.Instance.CurrentLoadingState == LevelLoadingManager.LoadingState.None;
    public static bool CanPlay()
    {
        // Cannot play in non playable states, in case of null references yes
        // has instace? has a not null game state? is the game resumed AND is a playable state?
        return !HasInstance || !Instance.gameState || (!Instance.isPaused && Instance.gameState.isPlayable);
    }
    #endregion
}

#if UNITY_EDITOR
[CustomEditor(typeof(GameStateManager), true)]
[CanEditMultipleObjects]
public class GameStateManagerUIEditor : UnityEditor.Editor
{
    public override void OnInspectorGUI()
    {
        // Get reference to the target script
        GameStateManager manager = (GameStateManager)target;

        // Draw the default inspector first (optional)

        DrawDefaultInspector();

        EditorGUILayout.Space(20f);
        EditorGUILayout.LabelField("Editor Settings", EditorStyles.boldLabel);

        #region BUTTONS
        if (GUILayout.Button("Toggle Pause"))
        {
            manager.TogglePause();
        }
        #endregion
    }
}
#endif