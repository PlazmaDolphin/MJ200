using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform), typeof(Button))]
public abstract class BaseButtonUI : MonoBehaviour
{
    public Action OnStartedAnimation;
    public Action OnEndedAnimation;

    public Animator Animator { get; private set; }
    public RectTransform RectTransform { get; private set; }
    public Button Button { get; private set; }

    [Header("References")]
    [SerializeField] protected TextMeshProUGUI text;
    [SerializeField] protected Image background;
    [SerializeField] protected Image icon;
    public TextMeshProUGUI Text => text;
    public Image Background => background;
    public Image Icon => icon;

    [Space(20f), Header("Sound Effects")]
    [SerializeField] protected SoundFXData clickedSFX;
    [SerializeField] protected SoundFXData selectedSFX;

    [Space(20f), Header("Unity Events")]
    [SerializeField] protected bool onlyCallEventOnce = false;
    [FormerlySerializedAs("OnAnimated")][SerializeField] private UnityEvent onClicked;
    protected bool calledEventOnce;

    // State
    public bool IsAnimating { get; private set; }
    public bool IsSelected { get; private set; }

    protected virtual void Awake()
    {
        Button = GetComponent<Button>();
        RectTransform = GetComponent<RectTransform>();
        Animator = GetComponent<Animator>();

        Button.onClick.AddListener(ClickButton);
    }

    protected virtual void OnDestroy()
    {
        Button.onClick.RemoveListener(ClickButton);
    }

    protected virtual void OnValidate()
    {
        if (text == null) text = GetComponent<TextMeshProUGUI>();
        if (background == null) background = GetComponent<Image>();
    }

    #region CLICK
    public void ClickButton()
    {
        if (IsAnimating)
        {
            return;
        }

        if (clickedSFX != null)
        {
            clickedSFX.Play(transform);
        }

        IsAnimating = true;
        OnStartedAnimation?.Invoke();
        AnimateClick();
    }

    protected abstract void AnimateClick();

    protected void FinishClick()
    {
        if (this == null || gameObject == null) return; // avoid destroyed refs
        if (!gameObject.activeInHierarchy) return;

        IsAnimating = false;

        if (!onlyCallEventOnce || !calledEventOnce)
        {
            onClicked?.Invoke();
            calledEventOnce = true;
        }

        OnEndedAnimation?.Invoke();
        Deselect();
    }
    #endregion

    #region SELECTING
    public void Select()
    {
        if (!Button.interactable)
        {
            return;
        }

        if (IsSelected)
        {
            return;
        }

        IsSelected = true;
        if (selectedSFX != null) selectedSFX.Play(transform);

        AnimateSelect();
    }

    protected abstract void AnimateSelect();

    public void Deselect()
    {
        if (!IsSelected) return;

        IsSelected = false;
        AnimateDeselect();
    }

    protected abstract void AnimateDeselect();
    #endregion
}
