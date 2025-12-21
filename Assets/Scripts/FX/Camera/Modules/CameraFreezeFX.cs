using System;
using System.Collections;
using UnityEngine;

[Serializable]
public class CameraFreezeFX
{
    private Coroutine freezeCorountine;

    private CameraManager manager;
    [SerializeField] private IntSetting enableFreezeFrames;

    public void Initialize(CameraManager manager)
    {
        this.manager = manager;

        #region NULL CHECKS
        if (enableFreezeFrames == null)
        {
            Debug.LogError("No enableFreezeFrames!");
            return;
        }
        #endregion
    }

    public void Freeze(CameraFXData cameraFreezeData)
    {
        if (cameraFreezeData == null)
        {
            return;
        }

        if (enableFreezeFrames && enableFreezeFrames.Value == 0)
        {
            return;
        }

        if (cameraFreezeData.freezeDuration <= 0f)
        {
            return;
        }

        if (freezeCorountine != null)
        {
            manager.StopCoroutine(freezeCorountine);
        }

        freezeCorountine = manager.StartCoroutine(FreezeLogic(cameraFreezeData));
    }

    private IEnumerator FreezeLogic(CameraFXData cameraFreezeData)
    {
        yield return new WaitForSecondsRealtime(cameraFreezeData.freezeStartDelay);

        // use fixed 1 value instead of current time scale
        // as if we call on succession the game can become too slow, or permanently stuck at 0
        float originalTimeScale = 1f;
        float newTimeScale = originalTimeScale * cameraFreezeData.freezeTimeScale;

        Time.timeScale = newTimeScale;

        yield return new WaitForSecondsRealtime(cameraFreezeData.freezeReturnDuration);

        float timer = 0f;

        while (timer < cameraFreezeData.freezeReturnDuration)
        {
            float t = timer / cameraFreezeData.freezeReturnDuration;
            float curveValue = cameraFreezeData.freezeReturnCurve.Evaluate(t);

            Time.timeScale = Mathf.Lerp(newTimeScale, originalTimeScale, curveValue);

            timer += Time.unscaledDeltaTime;
            yield return null;
        }

        // End
        Time.timeScale = originalTimeScale;

        freezeCorountine = null;
    }
}