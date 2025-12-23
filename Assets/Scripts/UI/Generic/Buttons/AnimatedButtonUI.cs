using System.Collections;
using UnityEngine;

public class AnimatedButtonUI : BaseButtonUI
{
    private Vector2 normalScale;

    [Header("Clicked Animation")]
    [SerializeField] private float punchScale = 0.2f;
    [SerializeField] private float clickDuration = 0.3f;
    [SerializeField] private AnimationCurve punchCurve;

    [Header("Selected Animation")]
    [SerializeField] private float selectedScaleAmount = 1.1f;
    [SerializeField] private float selectedDuration = 0.25f;
    [SerializeField] private AnimationCurve scaleCurve;

    private Coroutine runningAnim;
    private RectTransform rectTransform;

    protected void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        normalScale = rectTransform.localScale;
        rectTransform.localScale = normalScale;
    }

    protected override void AnimateClick()
    {
        if (runningAnim != null)
            StopCoroutine(runningAnim);

        Vector3 baseScale = IsSelected
            ? normalScale * selectedScaleAmount
            : normalScale;

        runningAnim = StartCoroutine(PunchScaleRoutine(baseScale));
    }

    protected override void AnimateSelect()
    {
        if (runningAnim != null)
            StopCoroutine(runningAnim);

        runningAnim = StartCoroutine(ScaleToRoutine(normalScale * selectedScaleAmount, selectedDuration));
    }

    protected override void AnimateDeselect()
    {
        if (runningAnim != null)
            StopCoroutine(runningAnim);

        runningAnim = StartCoroutine(ScaleToRoutine(normalScale, selectedDuration));
    }

    private IEnumerator PunchScaleRoutine(Vector3 baseScale)
    {
        rectTransform.localScale = baseScale;

        float time = 0f;
        float half = clickDuration * 0.5f;

        Vector3 punchTarget = baseScale + Vector3.one * punchScale;

        while (time < clickDuration)
        {
            time += Time.unscaledDeltaTime;
            float t = time / clickDuration;

            // Simple punch (you can swap for custom curve)
            float strength = punchCurve.Evaluate(t);

            // Move between baseScale and punchTarget using curve
            rectTransform.localScale = Vector3.LerpUnclamped(baseScale, punchTarget, strength);

            yield return null;
        }

        rectTransform.localScale = baseScale;

        if (IsSelected)
            Select();
        else
            Deselect();

        FinishClick();
        runningAnim = null;
    }

    private IEnumerator ScaleToRoutine(Vector3 target, float duration)
    {
        Vector3 start = rectTransform.localScale;
        float time = 0f;

        while (time < duration)
        {
            time += Time.unscaledDeltaTime;
            float t = time / duration;

            float eased = scaleCurve != null ? scaleCurve.Evaluate(t) : t;

            rectTransform.localScale = Vector3.LerpUnclamped(start, target, eased);
            yield return null;
        }

        rectTransform.localScale = target;
        runningAnim = null;
    }

    protected override void OnDestroy()
    {
        if (runningAnim != null)
            StopCoroutine(runningAnim);
        base.OnDestroy();
    }

    private void OnDisable()
    {
        if (runningAnim != null)
            StopCoroutine(runningAnim);
    }
}
