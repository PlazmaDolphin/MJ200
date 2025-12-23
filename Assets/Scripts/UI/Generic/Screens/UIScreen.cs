using System;
using UnityEngine;
using UnityEngine.Events;

#if UNITY_EDITOR
using UnityEditor;
#endif

[RequireComponent(typeof(CanvasGroup))]
[DefaultExecutionOrder(-1)]
public class UIScreen : MonoBehaviour
{
    public Action<bool> OnVisibilityChanged;

    protected CanvasGroup canvasGroup;

    [Header("Base UI Screen")]
    [Header("Visibility")]
    [SerializeField] private bool controlAlpha = true;
    [SerializeField] private DefaultActivationType activeByDefault = DefaultActivationType.NoOverriding;
    protected enum DefaultActivationType
    {
        NoOverriding,
        Active,
        Deactive
    }

    [field: SerializeField, ReadOnly] public bool IsVisible { get; set; }
    [field: SerializeField, ReadOnly] public bool IsTransitioning { get; set; }

    [Space(20f), Header("Screen Overlaying")]
    [SerializeField] protected BaseButtonUI buttonToPressOnBack;
    [SerializeField] protected bool isInteractable = true;
    [field: SerializeField, ReadOnly] public UIScreen ScreenBehind { get; set; }

    [Space(20f), Header("Screen Overlaying")]
    [SerializeField] protected AudioClip targetSong;
    [SerializeField] protected bool setMusic = false;

    [Space(20f), Header("Unity Events")]
    [SerializeField] private UnityEvent OnFadedIn;
    [SerializeField] private UnityEvent OnFadeOut;

    protected virtual void Awake()
    {
        // Get the CanvasGroup component
        canvasGroup = GetComponent<CanvasGroup>();
    }

    protected virtual void Start()
    {
        switch (activeByDefault)
        {
            case DefaultActivationType.NoOverriding:
                break;
            case DefaultActivationType.Active:
                SetImmediately(true);
                break;
            case DefaultActivationType.Deactive:
                SetImmediately(false);
                break;
        }
    }

    public virtual void SetWithTransition(bool visible)
    {
        // By default just set immediatelys, no transition

        SetImmediately(visible);
    }

    // Called at the end of a transition
    public void SetImmediately(bool visible)
    {
        if (canvasGroup != null)
        {
            if (controlAlpha)
            {
                //Debug.Log($"Manually set {gameObject.name}'s UI Screen to {visible}");
                canvasGroup.alpha = visible ? 1f : 0f;
            }

            canvasGroup.blocksRaycasts = isInteractable && visible;
            canvasGroup.interactable = isInteractable && visible;
        }

        SetMusics();

        IsTransitioning = false;
        IsVisible = visible;

        if (visible)
        {
            UIScreenManager.Instance?.PushScreen(this);
            OnFadedIn?.Invoke();
        }
        else
        {
            UIScreenManager.Instance?.RemoveScreen(this);
            OnFadeOut?.Invoke();
        }

        OnVisibilityChanged?.Invoke(visible);
    }

    private void SetMusics()
    {
        if (!setMusic)
        {
            return;
        }

        if (!MusicManager.HasInstance)
        {
            return;
        }

        if (targetSong == null)
        {
            MusicManager.Instance.StopMusic();
        }
        else
        {
            MusicManager.Instance.PlayMusic(targetSong);
        }
    }

    // Called when Escape is pressed while this screen is on top
    public virtual void OnBackInput()
    {
        if (!IsVisible) return;
        if (IsTransitioning) return;
        if (buttonToPressOnBack == null) return;

        buttonToPressOnBack.Button.onClick.Invoke();
        //Debug.Log($"{buttonToPressOnBack.gameObject.name} from ${gameObject.name} was clicked");
    }

    public void SetScreenBehind(UIScreen uIScreen)
    {
        ScreenBehind = uIScreen;
    }
}

#if UNITY_EDITOR
[CanEditMultipleObjects()]
[CustomEditor(typeof(UIScreen), true)]
public class UIScreenEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // Draw the default inspector first
        DrawDefaultInspector();

        // Get reference to the target UIScreen
        UIScreen screen = (UIScreen)target;

        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("Editor Controls", EditorStyles.boldLabel);

        // Button to show the screen
        if (GUILayout.Button("Show Screen (Transition True)"))
        {
            if (Application.isPlaying)
            {
                screen.SetWithTransition(true);
            }
            else
            {
                Debug.LogWarning("SetWithTransition only works in Play mode.");
            }
        }

        // Button to hide the screen
        if (GUILayout.Button("Hide Screen (Transition False)"))
        {
            if (Application.isPlaying)
            {
                screen.SetWithTransition(false);
            }
            else
            {
                Debug.LogWarning("SetWithTransition only works in Play mode.");
            }
        }
    }
}
#endif