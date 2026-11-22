
using System.Collections;
using UnityEngine;

public class MusicTransitionManager : MonoBehaviour
{
    [Header("Music Sources")]
    public AudioSource music44;
    public AudioSource music34;

    [Header("Transition")]
    public float fadeDuration = 1.5f;
    public float targetVolume = 0.7f;

    private int currentMeter = 0;
    private Coroutine transitionRoutine;

    void Start()
    {
        if (music44 == null || music34 == null)
        {
            Debug.LogError("Music AudioSources are missing!");
            return;
        }

        music44.loop = true;
        music34.loop = true;

        music44.playOnAwake = false;
        music34.playOnAwake = false;

        music44.volume = 0f;
        music34.volume = 0f;
    }

    public void PlayFourFour()
    {
        SwitchMeter(4);
    }

    public void PlayThreeFour()
    {
        SwitchMeter(3);
    }

    public void SwitchMeter(int meter)
    {
        if (meter != 3 && meter != 4)
        {
            Debug.LogWarning("Unsupported meter: " + meter);
            return;
        }

        if (music44 == null || music34 == null)
            return;

        if (currentMeter == meter)
            return;

        AudioSource next = meter == 4 ? music44 : music34;
        AudioSource previous = meter == 4 ? music34 : music44;

        currentMeter = meter;

        if (transitionRoutine != null)
            StopCoroutine(transitionRoutine);

        if (next.clip == null)
        {
            Debug.LogError("Missing audio clip for " + meter + "/4");
            currentMeter = 0;
            return;
        }

        if (!next.isPlaying)
        {
            next.volume = 0f;
            next.Play();
        }

        transitionRoutine = StartCoroutine(
            Crossfade(previous, next)
        );

        Debug.Log("Switching to " + meter + "/4 music");
    }

    IEnumerator Crossfade(AudioSource previous, AudioSource next)
    {
        float elapsed = 0f;
        float oldStart = previous.volume;
        float newStart = next.volume;
        float duration = Mathf.Max(0.01f, fadeDuration);

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;

            float t = Mathf.Clamp01(elapsed / duration);

            previous.volume = Mathf.Lerp(oldStart, 0f, t);
            next.volume = Mathf.Lerp(newStart, targetVolume, t);

            yield return null;
        }

        previous.volume = 0f;
        next.volume = targetVolume;

        previous.Stop();
        transitionRoutine = null;
    }
}
