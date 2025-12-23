using UnityEngine;

#if UNITY_EDITOR
using UnityEditor.SceneManagement;
#endif

public class CanvasCameraSetter : MonoBehaviour
{
    private void OnValidate()
    {
#if UNITY_EDITOR
        if (IsInPrefabMode())
            return;

        // Wait for canvas update so Unity doesn’t complain
        UnityEditor.EditorApplication.delayCall += () =>
        {
            if (this != null) // Ensure object still exists, as unity destroys objects when enterimg/exiting prefab mode
                UpdateCanvas();
        };
#endif
    }

    // Safe prefab mode check
    private bool IsInPrefabMode()
    {
        // When we build the game this will be defaulted to false as intended
#if UNITY_EDITOR
        return PrefabStageUtility.GetCurrentPrefabStage() != null;
#else
        return false;
#endif
    }

    private void Start()
    {
        if (UpdateCanvas() == null)
        {
            Debug.LogWarning(
                $"Tried to set <b>{gameObject.name}'s</b> canvas world camera, but no <b>'canvas'</b> component found!",
                this
            );
        }
    }

    private Canvas UpdateCanvas()
    {
        if (!TryGetComponent(out Canvas canvas))
            return null;

        if (canvas.worldCamera == null)
        {
            Camera camera = Camera.main;
            if (camera != null)
                canvas.worldCamera = camera;
        }

        return canvas;
    }
}
