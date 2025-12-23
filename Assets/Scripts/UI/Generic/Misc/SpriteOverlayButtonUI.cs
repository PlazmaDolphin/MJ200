using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SpriteOverlayButtonUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler
{
    public Action OnStartedHovering;
    public Action OnStoppedHovering;
    public Action<bool> OnInteractableStateChanged;
    public Action OnClicked;

    GameStateManager stateManager;
    public Button Button { get; set; }

    [Header("References")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private List<GameState> targetGameStates;

    [Space(20f), Header("Colors")]
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color hoveringColor = Color.yellow;
    [SerializeField] private Color clickedColor = Color.black;
    [SerializeField] private Color disabledColor = Color.red;

    [Space(20f), Header("Transition")]
    [SerializeField] private float hoverTransitionSpeed = 0.2f;
    [SerializeField] private float clickTransitionSpeed = 0.1f;
    [SerializeField] private float clickHoldDuration = 0.3f;
    [SerializeField, ReadOnly] private bool isHovering;
    [SerializeField, ReadOnly] private bool isOverlayInteractable;
    private Coroutine colorChangeCoroutine;
    private bool isClicking;

    private enum VisualState
    {
        Disabled,
        Normal,
        Hovering,
        Clicked
    }

    [SerializeField, ReadOnly] private VisualState currentVisualState;

    #region INITIALIZATION
    private void Awake()
    {
        if (spriteRenderer == null)
        {
            Debug.LogError("SpriteRenderer is not assigned.");
            return;
        }

        Button = GetComponent<Button>();

        stateManager = GameStateManager.Instance;

        if (stateManager != null)
        {
            stateManager.OnGameStateChanged += OnGameStateChanged;
        }

        Button.onClick.AddListener(HandleClick);
    }

    private void Start()
    {
        OnGameStateChanged(GameStateManager.Instance.GetGameState());
        // Set the initial state
        SetInteractable(true);
    }


    private void OnDestroy()
    {
        if (GameStateManager.Instance != null)
            GameStateManager.Instance.OnGameStateChanged -= OnGameStateChanged;

        if (Button != null)
            Button.onClick.RemoveListener(HandleClick);
    }

    private void OnGameStateChanged(GameState state)
    {
        // Ensure the button still exists before trying to access it
        if (Button == null) return;

        Button.interactable = targetGameStates.Contains(state);
    }
    #endregion

    #region STATE
    private void SetInteractable(bool value)
    {
        isOverlayInteractable = value;
        CheckIfInteractableChanged();
    }

    private void LateUpdate()
    {
        if (isOverlayInteractable != Button.interactable)
        {
            CheckIfInteractableChanged();
        }

        // We handle hover/select via event callbacks (pointer/select handlers).
        // No polling of currentSelectedGameObject here to avoid sticky selection-hover.
    }

    private void SetVisualState(VisualState newState, float transitionSpeed = 0.2f)
    {
        // while a click effect is actively playing, ignore requests to downgrade the visual state
        if (isClicking && newState != VisualState.Clicked)
            return;

        // Stop any current transition coroutine cleanly
        if (colorChangeCoroutine != null)
        {
            StopCoroutine(colorChangeCoroutine);
            colorChangeCoroutine = null;
        }

        currentVisualState = newState;

        Color target = normalColor;

        switch (newState)
        {
            case VisualState.Disabled: target = disabledColor; break;
            case VisualState.Normal: target = normalColor; break;
            case VisualState.Hovering: target = hoveringColor; break;
            case VisualState.Clicked: target = clickedColor; break;
        }

        colorChangeCoroutine = StartCoroutine(TransitionColor(spriteRenderer.color, target, transitionSpeed));
    }

    private void CheckIfInteractableChanged()
    {
        isOverlayInteractable = Button.interactable;
        OnInteractableStateChanged?.Invoke(isOverlayInteractable);

        if (!isOverlayInteractable)
            SetVisualState(VisualState.Disabled);
        else if (!isHovering)
            SetVisualState(VisualState.Normal);

        if (!isOverlayInteractable && isHovering)
            OnHoverExit();
    }
    #endregion

    #region BUTTON LOGIC
    private void OnHoverEnter()
    {
        if (!isOverlayInteractable) return;

        isHovering = true;
        SetVisualState(VisualState.Hovering, hoverTransitionSpeed);
        OnStartedHovering?.Invoke();
    }

    private void OnHoverExit()
    {
        isHovering = false;
        SetVisualState(isOverlayInteractable ? VisualState.Normal : VisualState.Disabled, hoverTransitionSpeed);
        OnStoppedHovering?.Invoke();
    }

    private void HandleClick()
    {
        if (!isOverlayInteractable) return;

        OnClicked?.Invoke();
        StartCoroutine(ClickEffect());
    }
    #endregion

    #region EVENT HANDLERS (pointer & selection)
    // Pointer events (mouse/touch)
    public void OnPointerEnter(PointerEventData eventData)
    {
        OnHoverEnter();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        OnHoverExit();
    }

    // Selection events (keyboard/controller navigation)
    public void OnSelect(BaseEventData eventData)
    {
        OnHoverEnter();
    }

    public void OnDeselect(BaseEventData eventData)
    {
        OnHoverExit();
    }
    #endregion

    #region COROUTINES
    private IEnumerator ClickEffect()
    {
        isClicking = true;
        SetVisualState(VisualState.Clicked, clickTransitionSpeed);
        yield return new WaitForSecondsRealtime(clickHoldDuration);

        isClicking = false;
        // Drop back down to hover or normal
        SetVisualState(isHovering ? VisualState.Hovering : VisualState.Normal, clickTransitionSpeed);

        // optional: clear selection to avoid keeping the button selected after click
        if (EventSystem.current != null && EventSystem.current.currentSelectedGameObject == gameObject)
            EventSystem.current.SetSelectedGameObject(null);
    }

    private IEnumerator TransitionColor(Color fromColor, Color toColor, float speed)
    {
        float t = 0f;
        // guard against zero speed
        float effectiveSpeed = Mathf.Max(0.0001f, speed);

        while (t < 1f)
        {
            t += Time.unscaledDeltaTime / effectiveSpeed;
            spriteRenderer.color = Color.Lerp(fromColor, toColor, t);
            yield return null;
        }

        spriteRenderer.color = toColor;

        // IMPORTANT: clear the coroutine reference so callers/guards know the transition finished
        colorChangeCoroutine = null;
    }
    #endregion
}
