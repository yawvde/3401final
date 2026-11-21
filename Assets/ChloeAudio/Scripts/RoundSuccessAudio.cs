using UnityEngine;

public class RoundSuccessAudio : MonoBehaviour
{
    [Header("Audio Player")]
    public NoteAudioPlayer audioPlayer;

    [Header("Round Manager")]
    public RoundManager roundManager;

    private bool playedThisRound = false;

    public bool IsPlaying
    {
        get
        {
            return playedThisRound &&
                   audioPlayer != null &&
                   audioPlayer.audioSource != null &&
                   audioPlayer.audioSource.isPlaying;
        }
    }

    public void ResetForRound()
    {
        playedThisRound = false;
    }

    public void PlayCurrentRound()
    {
        if (audioPlayer == null || roundManager == null)
        {
            UnityEngine.Debug.LogError(
                "RoundSuccessAudio: Missing references!"
            );
            return;
        }

        int index = roundManager.currentRound;

        if (audioPlayer.musicClips == null ||
            index < 0 ||
            index >= audioPlayer.musicClips.Length ||
            audioPlayer.musicClips[index] == null)
        {
            UnityEngine.Debug.LogWarning(
                "Missing audio for round " + (index + 1)
            );
            return;
        }

        playedThisRound = true;
        audioPlayer.PlayMusic(index);

        UnityEngine.Debug.Log(
            "PLAYING ROUND MUSIC: " + (index + 1)
        );
    }
}