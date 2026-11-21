using UnityEngine;

public class NoteAudioPlayer : MonoBehaviour
{
    [Header("Audio Source")]
    public AudioSource audioSource;

    [Header("Complete Music Clips")]
    public AudioClip[] musicClips;

    [Header("Testing")]
    public int testClipIndex = 0;

    private void Awake()
    {
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }
    }

    // Play one complete music clip.
    // Index starts at 0.
    public void PlayMusic(int index)
    {
        if (audioSource == null)
        {
            UnityEngine.Debug.LogError(
                "AudioSource is missing!"
            );
            return;
        }

        if (musicClips == null ||
            index < 0 ||
            index >= musicClips.Length)
        {
            UnityEngine.Debug.LogWarning(
                "Invalid music index: " + index
            );
            return;
        }

        AudioClip selectedClip = musicClips[index];

        if (selectedClip == null)
        {
            UnityEngine.Debug.LogWarning(
                "Music clip is missing at index: " + index
            );
            return;
        }

        audioSource.Stop();
        audioSource.clip = selectedClip;
        audioSource.loop = false;
        audioSource.Play();

        UnityEngine.Debug.Log(
            "Now playing: " + selectedClip.name
        );
    }

    // For testing with a Unity UI Button.
    public void PlayTestMusic()
    {
        PlayMusic(testClipIndex);
    }

    // Stop current music.
    public void StopMusic()
    {
        if (audioSource != null)
        {
            audioSource.Stop();
        }
    }
}