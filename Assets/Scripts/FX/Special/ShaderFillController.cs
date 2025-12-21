using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;


#if UNITY_EDITOR
using UnityEditor;
#endif

public class ShaderFillController : MonoBehaviour
{
    [Serializable]
    public struct Transition
    {
        public float duration;
        public float startDelay;
        public float endDelay;
        public AnimationCurve animationCurve;

        public static Transition Default => new()
        {
            duration = 1f,
            startDelay = 0f,
            endDelay = 0f,
            animationCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f) // nice smooth default
        };
    }


    [Header("Filling")]
    [SerializeField, Range(0, 1)] private float initialAmount = 0f;
    [SerializeField, ReadOnly] private Material material;
    private Image image;

    [Space(20f), Header("Unity Events")]
    [SerializeField] private UnityEvent<float> OnStartedTransition;
    [SerializeField] private UnityEvent<float> OnEndedTransition;

    private Coroutine transitionCoroutine;
    private readonly int fillAmount = Shader.PropertyToID("_FillAmount");

    public int FillAmount => fillAmount;
    public bool IsTransitioning => transitionCoroutine != null;

    private void Awake()
    {
        if (!TryGetComponent(out image))
        {
            Debug.LogError("Component not found!");
            return;
        }

        // Clone so we have a new instance, that way changing any values only changes this one's
        material = new Material(image.material) { name = $"COPY {image.material.name}" };
        image.material = material;

        SetFillAmount(initialAmount);
    }

    public Coroutine FadeTo(float targetAmount, Transition transition)
    {
        float start = material.GetFloat(fillAmount);

        if (transitionCoroutine != null)
        {
            //Debug.Log("Stopped Corountine");
            StopCoroutine(transitionCoroutine);
        }

        //Debug.Log("Started corountine");
        transitionCoroutine = StartCoroutine(TransitionCoroutine(start, targetAmount, transition));
        return transitionCoroutine;
    }

    public IEnumerator TransitionCoroutine(float start, float targetAmount, Transition transition)
    {
        OnStartedTransition?.Invoke(targetAmount);
        float elapsedTime = 0f;

        yield return new WaitForSecondsRealtime(transition.startDelay);

        while (elapsedTime < transition.duration)
        {
            elapsedTime += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(elapsedTime / transition.duration);

            // Apply animation curve
            float curvedT = transition.animationCurve.Evaluate(t);

            float lerpedAmount = Mathf.LerpUnclamped(start, targetAmount, curvedT);
            SetFillAmount(lerpedAmount);
            Debug.Log($"{gameObject.name} Lerping: {lerpedAmount}");

            yield return null;
        }

        //Debug.Log("Finished");

        yield return new WaitForSecondsRealtime(transition.endDelay);

        SetFillAmount(targetAmount);
        OnEndedTransition?.Invoke(targetAmount);
        transitionCoroutine = null;
    }

    public void SetFillAmount(float targetAmount)
    {
        Debug.Log($"Setted fill to {targetAmount}");
        image.materialForRendering.SetFloat(fillAmount, targetAmount);
    }

    private void OnDestroy()
    {
        if (transitionCoroutine != null)
        {
            StopCoroutine(transitionCoroutine);
        }
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(ShaderFillController), true)]
public class ShaderFillControllerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // Draw default inspector first
        DrawDefaultInspector();

        GUILayout.Space(10);

        // Header
        EditorGUILayout.LabelField("Editor Transitions", EditorStyles.boldLabel);

        // Buttons (only interactable in play mode)
        EditorGUI.BeginDisabledGroup(!Application.isPlaying);
        ShaderFillController controller = (ShaderFillController)target;

        if (GUILayout.Button("Set to 0"))
        {
            controller.SetFillAmount(0);
        }

        if (GUILayout.Button("Set to 1"))
        {
            controller.SetFillAmount(1);
        }

        if (GUILayout.Button("> Start Fill In"))
        {
            controller.FadeTo(1f, ShaderFillController.Transition.Default);
        }

        if (GUILayout.Button("< Start Fill Out"))
        {
            controller.FadeTo(0f, ShaderFillController.Transition.Default);
        }

        EditorGUI.EndDisabledGroup();
    }
}
#endif
