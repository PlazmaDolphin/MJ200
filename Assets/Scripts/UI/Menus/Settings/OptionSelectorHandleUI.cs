using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OptionSelectorHandleUI : BaseSelector
{
    [Space(20f), Header("Text")]
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private int textOffset = 1;
    [SerializeField] private int textMultiplier = 1;
    private string textFormat;

    [Space(20f), Header("Dots")]
    [SerializeField] private RectTransform dotContainer;
    [SerializeField] private RectTransform dotPrefab;

    [Space(20f), Header("Handle")]
    [SerializeField] private RectTransform handle;
    [ReadOnly, SerializeField] private Image handleImage;
    [SerializeField] private float transitionDuration = 0.2f;
    private Dictionary<Image, RectTransform> dots = new();

#if UNITY_EDITOR
    protected override void OnValidate()
    {
        if (settingData != null)
        {
            gameObject.name = $"{settingData.name} Handle Selector";
        }
    }
#endif

    protected override void Start()
    {
        if (!Application.isPlaying) return; // Prevent Edit mode execution

        base.Start();

        if (dotPrefab == null || handle == null || dotContainer == null)
        {
            Debug.LogError("Missing references on OptionSelectorHandleUI!", this);
            return;
        }

        textFormat = text.text;
        InitializeMethods();
    }


    protected override void InitializeMethods()
    {
        base.InitializeMethods();
        SpawnDots();
        StartCoroutine(DelayedMoveHandle());
    }

    private IEnumerator DelayedMoveHandle()
    {
        yield return null; // Wait one frame for layout to update

        MoveHandleImmediate(currentIndex);
    }

    private void SpawnDots()
    {
        for (int i = 0; i < maxOptions; i++)
        {
            RectTransform dot = Instantiate(dotPrefab, dotContainer);
            Image img = dot.GetComponent<Image>();

            dot.gameObject.SetActive(true);
            dots.Add(img, dot);
        }
    }

    protected override void UpdateVisuals()
    {
        if (text != null)
        {
            int displayValue = (settingData.Value + textOffset) * textMultiplier;
            text.text = string.Format(textFormat, displayValue);
        }

        var keys = new List<Image>(dots.Keys);

        for (int i = 0; i < keys.Count; i++)
        {
            var dot = keys[i];
            dot.color = (i == settingData.Value) ? selectedColor : normalColor;
        }

        // Color change
        if (handleImage != null)
        {
            handleImage.color = (currentIndex == settingData.Value) ? selectedColor : normalColor;
        }
    }

    public override void ChangeOption(int direction)
    {
        base.ChangeOption(direction);
        AnimateHandleTo(currentIndex);
    }

    private void AnimateHandleTo(int index)
    {
        isAnimating = true;

        var keys = new List<Image>(dots.Keys);
        Vector3 worldPos = keys[index].rectTransform.position;
        Vector3 localTarget = handle.parent.InverseTransformPoint(worldPos);


        // Move handle
        handle.DOLocalMove(localTarget, transitionDuration)
            .SetEase(Ease.OutCubic).SetUpdate(true).OnComplete(() =>
            {
                isAnimating = false;
                settingData.SetTargetValue(index);
            });
    }

    private void MoveHandleImmediate(int index)
    {
        if (index < 0 || index >= dots.Count)
            return;

        var keys = new List<Image>(dots.Keys);
        Vector3 worldPos = keys[index].rectTransform.position;
        handle.localPosition = handle.parent.InverseTransformPoint(worldPos);
    }
}
