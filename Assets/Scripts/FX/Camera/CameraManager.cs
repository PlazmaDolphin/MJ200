using Unity.Cinemachine;
using UnityEngine;

public class CameraManager : MonoBehaviourSingleton<CameraManager>
{
    // Uses classes with constructors.
    [Header("Camera FXs")]
    [SerializeField] private CameraShakeFX shakeFX;
    public CameraShakeFX ShakeFX => shakeFX;
    [SerializeField] private CameraFreezeFX freezeFX;
    public CameraFreezeFX FreezeFX => freezeFX;

    [Header("Cinemachine")]
    [SerializeField] private CinemachineCamera cinemachineCamera;
    public CinemachineCamera CinemachineCamera => cinemachineCamera;

    private void Awake()
    {
        shakeFX.Initialize(this);
        freezeFX.Initialize(this);
    }
}
