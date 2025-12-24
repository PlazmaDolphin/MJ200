using UnityEngine;

[CreateAssetMenu(fileName = "Shake Data", menuName = "Misc/Camera FX")]
public class CameraFXData : ScriptableObject
{
    [Header("Shake")]
    public float shakeStartDelay = 0f;
    public Vector2 shakeStrength = new(0.1f, 0.1f);
    public float shakeDuration = 0.1f;
    public float shakeImpulseInterval = 0.05f;

    //[Space(20f), Header("Controller Rumble")]
    //[Range(0f, 1f)] public float controllerLowRumble = 0f; // Used more for heavy explosions
    //[Range(0f, 1f)] public float controllerHighRumble = 0.5f; // Used for light zaps
    //public float controllerRumbleDuration = 0.3f;

    [Space(20f), Header("Freeze")]
    public float freezeStartDelay = 0f;
    public float freezeDuration;
    public float freezeTimeScale = 0f;
    public AnimationCurve freezeReturnCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    public float freezeReturnDuration = 0.3f;

    public void Play(Transform sender = null)
    {
        CameraManager.Instance.ShakeFX.Shake(this, sender);
        CameraManager.Instance.FreezeFX.Freeze(this);
        //InputManager.Instance.Rumble(this);
    }
}