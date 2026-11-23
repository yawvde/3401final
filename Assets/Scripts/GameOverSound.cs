
using UnityEngine;

public class GameOverSound : MonoBehaviour
{
    [Header("Game Over Audio")]
    public AudioSource audioSource;
    public AudioClip gameOverClip;

    private bool hasPlayed = false;

    public void PlayGameOverSound()
    {
        if (hasPlayed)
            return;

        if (audioSource == null || gameOverClip == null)
        {
            Debug.LogWarning("Game Over audio is not assigned!");
            return;
        }

        audioSource.PlayOneShot(gameOverClip);
        hasPlayed = true;
    }

    public void ResetGameOverSound()
    {
        hasPlayed = false;
    }
}
