using UnityEngine;

public class MusicManager : MonoBehaviourSingleton<MusicManager>
{
    [SerializeField] private AudioSource gameMusic;
    private PreviousValueTracker<AudioClip> musicTracker = new();

    private void Awake()
    {
        if (gameMusic == null)
            gameMusic = GetComponent<AudioSource>();

        // So we keep track of the initial music clip
        musicTracker.Set(gameMusic.clip);
    }

    public void PlayMusic(AudioClip audioClip)
    {
        if (audioClip == null || !musicTracker.Set(audioClip))
            return;

        UpdateMusic();
    }

    public void ReturnToPrevious()
    {
        if (musicTracker.ReturnToPrevious())
        {
            UpdateMusic();
        }
    }

    private void UpdateMusic()
    {
        Debug.Log($"Playing: {musicTracker.Current}");
        gameMusic.Stop();
        gameMusic.clip = musicTracker.Current;
        gameMusic.Play();
    }

    public void StopMusic()
    {
        if (musicTracker.Current == null)
            return;

        gameMusic.Stop();
        gameMusic.clip = null;
        musicTracker.Clear();
    }
}
