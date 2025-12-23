using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


#if UNITY_EDITOR
using UnityEditor;
#endif

public class UIScreenManager : MonoBehaviourSingleton<UIScreenManager>
{
    public Action<UIScreen> OnScreenOpened;
    public Action<UIScreen> OnScreenClosed;
    public Action OnAllScreensClosed;

    public Action OnNavigated;
    public Action OnResetNavigate;

    [Header("Input")]
    //[SerializeField] private InputActionReference backInput;
    [SerializeField] private float backInputCooldown = 0.2f; // Small delay
    private float lastScreenOpenedTime = -1f;

    //[SerializeField] private InputActionReference navigateReference;
    //[SerializeField] private InputActionReference resetNavigateReference;
    public List<UIScreen> ScreenList { get; private set; } = new();

    #region INPUT
    //private void OnEnable()
    //{
    //    backInput.action.Enable();
    //    backInput.action.performed += HandleEscapeInput;
    //}

    //private void HandleEscapeInput(InputAction.CallbackContext obj)
    private void HandleEscapeInput()
    {
        // Debounce input right after opening a screen
        if (Time.unscaledTime - lastScreenOpenedTime < backInputCooldown)
        {
            //Debug.Log("Back input ignored due to cooldown.");
            return;
        }

        if (!HasScreensOpen())
        {
            //Debug.Log("No screen opened on ui screen manager");
            return;
        }

        GetTopScreen().OnBackInput();
    }

    public void PushScreen(UIScreen newScreen)
    {
        if (ScreenList.Contains(newScreen))
        {
            return;
        }

        if (ScreenList.Count > 0)
        {
            newScreen.SetScreenBehind(GetTopScreen());
        }

        ScreenList.Add(newScreen);
        lastScreenOpenedTime = Time.unscaledTime; // Start cooldown now

        OnScreenOpened?.Invoke(newScreen);
    }

    //private void OnDisable()
    //{
    //    backInput.action.performed += HandleEscapeInput;
    //    backInput.action.Disable();
    //}
    #endregion

    public void RemoveScreen(UIScreen newScreen)
    {
        if (ScreenList.Count == 0) return;
        if (!ScreenList.Contains(newScreen))
            return;

        ScreenList.Remove(newScreen);
        //newScreen.SetWithTransition(false); // hide

        OnScreenClosed?.Invoke(newScreen);

        if (ScreenList.Count == 0)
            OnAllScreensClosed?.Invoke();
    }

    public UIScreen GetTopScreen() => ScreenList.Count > 0 ? ScreenList[ScreenList.Count - 1] : null;
    public bool HasScreensOpen() => ScreenList.Count > 0;
}

#if UNITY_EDITOR
[CustomEditor(typeof(UIScreenManager))]
public class UIScreenManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // Draw the default inspector
        DrawDefaultInspector();

        // Add a separator
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("UI Debug Info", EditorStyles.boldLabel);

        // Show the currently selected UI element in the EventSystem
        GameObject currentSelected = EventSystem.current?.currentSelectedGameObject;

        if (currentSelected != null)
        {
            EditorGUILayout.ObjectField("Selected UI Element", currentSelected, typeof(GameObject), true);
        }
        else
        {
            EditorGUILayout.LabelField("Selected UI Element", "None");
        }

        // Refresh the inspector every frame in play mode
        if (Application.isPlaying)
        {
            Repaint();
        }
    }
}
#endif