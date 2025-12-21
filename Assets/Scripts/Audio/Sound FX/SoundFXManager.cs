using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class SoundFXManager : MonoBehaviourSingleton<SoundFXManager>
{
    [Header("Playing SFX")]
    [SerializeField] AudioSource baseAudioSource;
    [SerializeField, ReadOnly] private List<AudioSource> audioSourcesReadyList = new();
    [SerializeField, ReadOnly] private List<AudioSource> audioSourcesActiveList = new();
    private const int audioSourcesAmount = 10;

    private List<AudioSource> allAudioSources = new List<AudioSource>();
    private bool isPaused = false;

    #region INITIALIZATION
    private void Awake()
    {
        if (!MarkInstanceAsDontDestroyOnLoad(this))
        {
            return;
        }

        for (int i = 0; i < audioSourcesAmount; ++i)
        {
            var spawnedAudioSource = Instantiate(baseAudioSource, transform.position, Quaternion.identity, transform);
            audioSourcesReadyList.Add(spawnedAudioSource);
            allAudioSources.Add(spawnedAudioSource);
        }

        //GameStateManager.Instance.OnPausedStateChanged += UpdateAudiosAll;
    }

    private void OnValidate()
    {
        if (Application.isPlaying)
        {
            return;
        }

        if (transform.childCount > 0)
        {
            return;
        }

        if (transform.GetComponentInChildren<AudioSource>() == null)
        {
            return;
        }

        GameObject newObject = new("Base Audio Source");
        newObject.transform.SetParent(transform, false);
        newObject.isStatic = true;
        baseAudioSource = newObject.AddComponent<AudioSource>();
    }
    #endregion

    #region PLAYING SFX
    public void PlaySoundFX(SoundFXData soundFXData, Transform source)
    {
        source = source != null ? source : transform;
        AudioSource audioSource = GetAudioSource(source.position);
        audioSource.gameObject.name = $"{source.name} Audio Source";
        audioSource.volume = soundFXData.volume;
        audioSource.outputAudioMixerGroup = soundFXData.mixerGroup;

        // Pick random clip & pitch
        var clip = soundFXData.clips[Random.Range(0, soundFXData.clips.Length)];
        audioSource.clip = clip;

        float pitch = Random.Range(soundFXData.minPitch, soundFXData.maxPitch);
        audioSource.pitch = pitch;

        audioSource.Play();
        audioSourcesActiveList.Add(audioSource);

        float length = audioSource.clip.length;
        StartCoroutine(WaitToEnd(audioSource, length + 0.01f));
    }

    private AudioSource GetAudioSource(Vector2 origin)
    {
        if (audioSourcesReadyList.Count > 0)
        {
            var pooledAudioSource = audioSourcesReadyList[0];
            pooledAudioSource.transform.position = origin;
            pooledAudioSource.gameObject.SetActive(true);
            audioSourcesReadyList.RemoveAt(0);
            return pooledAudioSource;
        }

        return Instantiate(baseAudioSource, origin, Quaternion.identity, transform);
    }

    private IEnumerator WaitToEnd(AudioSource audioSource, float length)
    {
        yield return new WaitForSecondsRealtime(length);

        if (audioSourcesActiveList.Contains(audioSource))
            audioSourcesActiveList.Remove(audioSource);

        audioSource.gameObject.SetActive(false);
        audioSourcesReadyList.Add(audioSource);
    }
    #endregion

    #region PAUSING
    public void RegisterSource(AudioSource source)
    {
        if (!allAudioSources.Contains(source))
        {
            allAudioSources.Add(source);
        }
    }

    public void UpdateAudiosAll(bool pauseAll)
    {
        if (isPaused == pauseAll) return;

        foreach (var audioSource in allAudioSources)
        {
            // ex: if we want to pause (true) and the audio is playing (true) we will pause, otherwise skip
            if (audioSource.isPlaying != pauseAll)
            {
                continue;
            }

            if (pauseAll)
            {
                audioSource.Pause();
            }
            else
            {
                audioSource.UnPause();
            }
        }

        isPaused = pauseAll;
    }
    #endregion
}
