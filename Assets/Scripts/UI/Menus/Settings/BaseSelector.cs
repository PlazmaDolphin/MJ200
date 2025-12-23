using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasRenderer))]
public class BaseSelector : Selectable, ISubmitHandler
{
    [Space(20f), Header("Arrows")]
    [SerializeField] protected BaseButtonUI leftArrow;
    [SerializeField] protected BaseButtonUI rightArrow;

    [Space(20f), Header("Input")]
    [SerializeField] protected float inputBuffer = 0.2f;
    protected float currentInputTimer;

    [Space(20f), Header("Colors")]
    [SerializeField] protected Color normalColor = Color.white;
    [SerializeField] protected Color selectedColor = Color.green;

    [Space(20f), Header("Values")]
    [SerializeField] protected IntSetting settingData;
    [SerializeField] protected int maxOptions;

    [ReadOnly, SerializeField] protected int currentIndex = 0;
    [ReadOnly, SerializeField] protected bool isAnimating;

    public int CurrentIndex => currentIndex;

#if UNITY_EDITOR
    protected override void OnValidate()
    {
        if (settingData != null)
        {
            gameObject.name = $"{settingData.name} Selector";
        }
    }
#endif

    protected override void Start()
    {
        base.Start();

        if (!Application.isPlaying) return;

        currentIndex = settingData.Value;

        leftArrow.Button.onClick.AddListener(() => ChangeOption(-1));
        rightArrow.Button.onClick.AddListener(() => ChangeOption(1));

        if (settingData != null)
        {
            settingData.OnValueChanged += (_) => UpdateVisuals();
        }
    }

    protected virtual void InitializeMethods()
    {
        SetArrowVisibility(false);
        UpdateVisuals();
    }

    protected override void OnDestroy()
    {
        if (settingData != null)
        {
            settingData.OnValueChanged -= (_) => UpdateVisuals();
        }

        base.OnDestroy();
    }

    protected virtual void Update()
    {
        if (!Application.isPlaying) return;
        if (!IsActive() || !IsInteractable() || EventSystem.current.currentSelectedGameObject != gameObject)
            return;

        // Reduce timer
        if (currentInputTimer > 0f)
        {
            currentInputTimer -= Time.unscaledDeltaTime;
            return;
        }

        if (isAnimating)
            return;

        //Vector2 move = moveInput.action.ReadValue<Vector2>();
        float move = Input.GetAxis("Horizontal");

        if (move > 0.5f)
        {
            ChangeOption(1);
            currentInputTimer = inputBuffer;
        }
        else if (move < -0.5f)
        {
            ChangeOption(-1);
            currentInputTimer = inputBuffer;
        }
    }

    public virtual void ChangeOption(int direction)
    {
        if (isAnimating)
            return;

        int newIndex = Mathf.Clamp(currentIndex + direction, 0, maxOptions);
        if (newIndex == currentIndex)
            return;

        currentIndex = newIndex;
        UpdateVisuals();
    }

    protected virtual void UpdateVisuals()
    {
        // Update visuals
    }

    public override void OnSelect(BaseEventData eventData)
    {
        base.OnSelect(eventData);
        SetArrowVisibility(true);
    }

    public override void OnDeselect(BaseEventData eventData)
    {
        base.OnDeselect(eventData);
        SetArrowVisibility(false);
    }

    public virtual void OnSubmit(BaseEventData eventData)
    {
        if (isAnimating)
        {
            Debug.Log("Couldnt apply changes because animating");
            return;
        }

        Debug.Log("Applied changes");
        settingData.UpdateValue();
    }

    protected virtual void SetArrowVisibility(bool visible)
    {
        List<BaseButtonUI> buttons = new() { leftArrow, rightArrow };

        foreach (var button in buttons)
        {
            button.Background.enabled = visible;
            button.Button.interactable = visible;
        }
    }
}
