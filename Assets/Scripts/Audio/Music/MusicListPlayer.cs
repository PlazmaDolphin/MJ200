using System.Collections.Generic;
using UnityEngine;

public class MusicListPlayer : MonoBehaviour
{
    public static Song lastPlayedSong;

    [System.Serializable]
    public class Song
    {
        public AudioClip clip;
        public float targetVolume = 1.0f;
    }

    public AudioSource audioSource;
    public List<Song> songsList = new List<Song>();
    private int currentSongIndex = 0;
    public bool randomizeOrderOnNextTime;
    private float songStartTime;

    void Start()
    {
        int targetIndex = 0; // Start at first music by default

        if (lastPlayedSong != null && randomizeOrderOnNextTime)
        {
            List<Song> list = new(songsList);
            list.Remove(lastPlayedSong);

            targetIndex = Random.Range(0, list.Count - 1);
        }

        if (songsList.Count > 0 && audioSource != null)
        {
            PlaySong(songsList[targetIndex]);
        }
    }

    void Update()
    {
        if (audioSource.isPlaying) return;

        if (Time.time - songStartTime >= audioSource.clip.length)
        {
            PlayNextSong();
        }
    }

    public void PlayNextSong()
    {
        if (songsList.Count == 0) return;

        currentSongIndex = (currentSongIndex + 1) % songsList.Count;
        lastPlayedSong = songsList[currentSongIndex];
        PlaySong(lastPlayedSong);
    }

    public void PlaySong(Song song)
    {
        if (song.clip != null)
        {
            audioSource.clip = song.clip;
            audioSource.volume = song.targetVolume;
            audioSource.Play();
            songStartTime = Time.time;
        }
    }

}
