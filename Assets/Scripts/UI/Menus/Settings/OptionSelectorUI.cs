using DG.Tweening;
using System;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(CanvasRenderer))]
public class OptionSelectorUI : BaseSelector
{
    public event Action<int> OnTransitionEnd;

    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI textCurrent;
    [SerializeField] private TextMeshProUGUI textNext;

    [Header("Animation")]
    public float transitionDuration = 0.25f;
    public float moveDistance = 100f;

    [Header("Options")]
    [SerializeField] private string[] options;

#if UNITY_EDITOR
    protected override void OnValidate()
    {
        if (settingData != null)
        {
            gameObject.name = $"{settingData.name} Text Selector";
        }
    }
#endif

    protected override void Start()
    {
        if (!Application.isPlaying) return; // Prevent Edit mode execution

        base.Start();
        InitializeMethods();
    }

    protected override void InitializeMethods()
    {
        base.InitializeMethods();
        UpdateVisual();
    }

    public override void ChangeOption(int direction)
    {
        if (isAnimating || options == null || options.Length == 0)
            return;

        int prevIndex = currentIndex;
        currentIndex = (currentIndex + direction + options.Length) % options.Length;
        AnimateTextSlide(prevIndex, currentIndex, direction);
    }

    private void AnimateTextSlide(int fromIndex, int toIndex, int direction)
    {
        isAnimating = true;

        Vector2 origin = textCurrent.rectTransform.anchoredPosition;
        float offset = moveDistance * direction;
        int target = settingData.Value;

        textCurrent.text = options[fromIndex];
        textNext.text = options[toIndex];

        Color fromColor = fromIndex == target ? selectedColor : normalColor;
        Color toColor = toIndex == target ? selectedColor : normalColor;

        fromColor.a = 1f;
        toColor.a = 0f; // because we fade it in

        textCurrent.color = fromColor;
        textNext.color = toColor;


        textCurrent.rectTransform.anchoredPosition = origin;
        textNext.rectTransform.anchoredPosition = origin + new Vector2(offset, 0);
        textNext.alpha = 0;

        Sequence sequence = DOTween.Sequence().SetUpdate(true);

        sequence.Join(textCurrent.rectTransform.DOAnchorPosX(origin.x - offset, transitionDuration).SetEase(Ease.InOutCubic)).SetUpdate(true);
        sequence.Join(textCurrent.DOFade(0f, transitionDuration)).SetUpdate(true);

        sequence.Join(textNext.rectTransform.DOAnchorPosX(origin.x, transitionDuration).SetEase(Ease.InOutCubic)).SetUpdate(true);
        sequence.Join(textNext.DOFade(1f, transitionDuration)).SetUpdate(true);

        sequence.OnComplete(EndTransition).SetUpdate(true);
    }

    private void EndTransition()
    {
        UpdateVisual();
        isAnimating = false;

        OnTransitionEnd?.Invoke(currentIndex);
        settingData.SetTargetValue(currentIndex);
    }

    private void UpdateVisual()
    {
        textCurrent.text = FormatOptionText(options[currentIndex], currentIndex == settingData.Value);
        textNext.text = string.Empty;
        textCurrent.color = currentIndex == settingData.Value ? selectedColor : normalColor;
        textCurrent.alpha = 1f;
    }

    private string FormatOptionText(string raw, bool isSelected)
    {
        return isSelected ? $"<b><u>{raw}</u></b>" : raw;
    }

    public void SetOption(int index)
    {
        if (index >= 0 && index < options.Length)
        {
            currentIndex = index;
            UpdateVisual();
        }
    }

    public string GetSelectedOptionRawKey()
    {
        return options[currentIndex] ?? "INVALID";
    }
}
