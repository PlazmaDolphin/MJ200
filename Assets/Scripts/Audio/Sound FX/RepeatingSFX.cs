using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "Repeating Sound", menuName = "Sound Effects/Repeating Sound Effect")]
public class RepeatingSFX : SoundFXData
{
    [Space(20f), Header("RepeatingSFX")]
    public float timeInBetween;
    public bool isPlaying;

    public override void Play(Transform source = null)
    {
        SoundFXManager instance = SoundFXManager.Instance;

        if (instance == null)
        {
            return;
        }

        isPlaying = true;
        instance.StartCoroutine(PlayingCorountine(source));
    }

    private IEnumerator PlayingCorountine(Transform source)
    {
        while (isPlaying)
        {
            var played = PlayLogic(source);
            yield return new WaitForSeconds(played.clip.length + timeInBetween);
        }
    }

    public void SetIsPlaying(bool isPlaying)
    {
        this.isPlaying = isPlaying;
    }
}
