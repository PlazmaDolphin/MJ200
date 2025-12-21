using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;
using Random = UnityEngine.Random;

[Serializable]
public class CameraShakeFX
{
    private CameraManager manager;

    [SerializeField] private FloatSetting shakeStrenght;
    [SerializeField] private CinemachineImpulseSource baseImpulseSource;

    private List<CinemachineImpulseSource> impulseSourcesReadyList = new();
    private const int INITIAL_SHAKES_AMOUNT = 3;

    // Constructor
    public void Initialize(CameraManager manager)
    {
        this.manager = manager;

        baseImpulseSource = manager.GetComponentInChildren<CinemachineImpulseSource>();

        #region NULL CHECKS
        if (shakeStrenght == null)
        {
            Debug.LogError("No shakeStrenght!");
            return;
        }

        if (baseImpulseSource == null)
        {
            Debug.LogError("No CinemachineImpulseSource found in CameraManager children!");
            return;
        }
        #endregion

        for (int i = 0; i < INITIAL_SHAKES_AMOUNT; ++i)
        {
            var spawnedImpulseSource = Spawn(manager.transform);
            impulseSourcesReadyList.Add(spawnedImpulseSource);
        }
    }

    #region CAMERA SHAKE
    public void Shake(CameraFXData cameraShakeData, Transform senderTransform = null)
    {
        if (cameraShakeData == null)
        {
            Debug.LogError("Camera shake data is null!");
            return;
        }

        // Says how many shaking we can have at once
        //if (maxShakesActive > 0 && shakesActive >= maxShakesActive)
        //{
        //    return;
        //}

        if (shakeStrenght && shakeStrenght.Value == 0)
        {
            return;
        }

        // Instantiate impulse source
        CinemachineImpulseSource newImpulseSource = GetAvailableImpulseSource(senderTransform);
        newImpulseSource.gameObject.name = $"{baseImpulseSource.gameObject.name} ({senderTransform.name})";

        // Coroutine to apply repeated impulses during the shake duration
        manager.StartCoroutine(ApplyRepeatedImpulses(newImpulseSource, cameraShakeData));
    }

    private IEnumerator ApplyRepeatedImpulses(CinemachineImpulseSource impulseSource, CameraFXData cameraShakeData)
    {
        yield return new WaitForSecondsRealtime(cameraShakeData.shakeStartDelay);

        float elapsedTime = 0f;

        while (elapsedTime < cameraShakeData.shakeDuration)
        {
            int randomHorizontalValue = Random.value < 0.5f ? -1 : 1;
            int randomVerticalValue = Random.value < 0.5f ? -1 : 1;

            float value = shakeStrenght ? shakeStrenght.Value : 1;
            Vector2 targetStrenght = cameraShakeData.shakeStrength * value * new Vector2(randomHorizontalValue, randomVerticalValue);

            impulseSource.DefaultVelocity = targetStrenght;

            // Generate the impulse at regular intervals
            impulseSource.GenerateImpulse();

            // Wait for the next interval
            yield return new WaitForSeconds(cameraShakeData.shakeImpulseInterval);

            // Increment the elapsed time
            elapsedTime += cameraShakeData.shakeImpulseInterval;
        }

        // Clean up the GameObject after finishing
        yield return new WaitForSecondsRealtime(0.1f);
        impulseSource.gameObject.SetActive(false);
        impulseSourcesReadyList.Add(impulseSource);
    }

    // Pooling impulse sources
    private CinemachineImpulseSource GetAvailableImpulseSource(Transform senderTransform)
    {
        Transform target = senderTransform == null ? manager.transform : senderTransform;

        if (impulseSourcesReadyList.Count > 0)
        {
            var pooledImpulseSource = impulseSourcesReadyList[0];

            // If recycling audio source re-calculated position and activate it
            pooledImpulseSource.transform.position = target.position;
            pooledImpulseSource.gameObject.SetActive(true);

            // Remove it from audio sources list so other's can't use it
            impulseSourcesReadyList.Remove(pooledImpulseSource);

            return pooledImpulseSource;
        }
        CinemachineImpulseSource spawnedImpulseSource = Spawn(target);
        return spawnedImpulseSource;
    }

    private CinemachineImpulseSource Spawn(Transform target)
    {
        return CinemachineImpulseSource.Instantiate(baseImpulseSource, target.position, Quaternion.identity, manager.transform);
    }
    #endregion
}