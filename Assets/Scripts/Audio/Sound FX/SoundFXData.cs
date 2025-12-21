using UnityEngine;
using UnityEngine.Audio;

[CreateAssetMenu(fileName = "Sound", menuName = "Misc/Sound Effect")]
public class SoundFXData : ScriptableObject
{
    [Header("References")]
    public AudioMixerGroup mixerGroup;
    public AudioClip[] clips;

    [Header("Playing")]
    [Range(0f, 1f)] public float volume = 1f;
    public float minPitch = 0.9f;
    public float maxPitch = 1.1f;

    /// <summary>
    /// Plays this sound effect at the given position.
    /// </summary>
    public void Play(Transform source = null)
    {
        if (clips == null || clips.Length == 0 || mixerGroup == null)
        {
            Debug.LogWarning($"SoundFXData '{name}' is not set up correctly.", this);
            return;
        }

        if (!SoundFXManager.HasInstance) return;

        SoundFXManager.Instance.PlaySoundFX(this, source);
    }
}
