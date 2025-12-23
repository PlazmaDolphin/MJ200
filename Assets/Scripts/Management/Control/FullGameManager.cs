using UnityEngine;

public class FullGameManager : MonoBehaviourSingleton<FullGameManager>
{
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P) || Input.GetKeyDown(KeyCode.Escape))
        {
            GameStateManager.Instance.TogglePause();
        }
    }

    //[SerializeField] IntSetting fullScreenSetting;
    ////[SerializeField] IntSetting muteAudioSetting; // Do this instead on sound manager

    //private void Awake()
    //{
    //    if (!MarkInstanceAsDontDestroyOnLoad(this))
    //    {
    //        return;
    //    }
    //}

    //private void OnEnable()
    //{
    //    #region INPUT
    //    InputManager manager = InputManager.Instance;

    //    if (!manager)
    //    {
    //        return;
    //    }

    //    manager.pauseInput.OnPerformed += Input_Pause;
    //    manager.fullScreenInput.OnPerformed += Input_ToggleFullScreen;
    //    manager.reloadLevelInput.OnPerformed += Input_Reaload;
    //    manager.closeGameInput.OnPerformed += Input_ExitGame;

    //    if (!Application.isEditor)
    //    {
    //        Application.focusChanged += Application_FocusChanged;
    //        Application.wantsToQuit += Application_wantsToQuit;
    //    }
    //    #endregion

    //    #region SETTINGS DATAS
    //    if (fullScreenSetting)
    //    {
    //        fullScreenSetting.OnValueChanged += FullScreenSetting_OnValueChanged;
    //        FullScreenSetting_OnValueChanged(fullScreenSetting.Value);
    //    }
    //    #endregion
    //}

    //private void OnDisable()
    //{
    //    if (!Application.isEditor)
    //    {
    //        Application.focusChanged -= Application_FocusChanged;
    //        Application.wantsToQuit -= Application_wantsToQuit;
    //    }
    //}

    //#region APPLICATION
    //private void Application_FocusChanged(bool _)
    //{
    //    if (!GameStateManager.HasInstance)
    //    {
    //        return;
    //    }

    //    GameStateManager.Instance.SetPausedState(true);
    //}

    //private bool Application_wantsToQuit()
    //{
    //    if (GameStateManager.HasInstance && GameStateManager.Instance.GetIsPaused())
    //    {
    //        GameStateManager.Instance.SetPausedState(true);

    //        // Return false to prevent quitting
    //        return false;
    //    }

    //    // If the game is paused, proceed with quitting
    //    return true;
    //}
    //#endregion

    //#region INPUT AND SETTINGS
    //private void Input_Pause()
    //{
    //    var manager = GameStateManager.Instance;

    //    if (manager)
    //    {
    //        manager.SetPausedState(true);
    //    }
    //}

    //private void Input_ExitGame()
    //{
    //    Debug.Log($"Exited {Application.productName}");

    //    if (!Application.isEditor)
    //    {
    //        Application.Quit();
    //    }
    //}

    //private void Input_Reaload()
    //{
    //    LevelLoadingManager.Instance.ReloadLevel();
    //}

    //private void Input_ToggleFullScreen()
    //{
    //    fullScreenSetting.SetValueFromBool(!Screen.fullScreen);
    //    fullScreenSetting.UpdateValue();
    //}

    //private void FullScreenSetting_OnValueChanged(int value)
    //{
    //    //Debug.Log($"Setted fullscreen to {value == 1}");
    //    Screen.fullScreen = value == 1;
    //}
    //#endregion
}
