
using System.Collections;
using UnityEngine;

public class CustomerReaction : MonoBehaviour
{
    [Header("Character Models")]
    public GameObject moleRat;
    public GameObject skeletonMage;
    public Animator moleRatAnimator;

    [Header("Success Effects")]
    public ParticleSystem successFireworks;
    public ParticleSystem yellowFireworks;

    [Header("Settings")]
    public float failDuration = 2f;

    [Header("Keyboard Testing")]
    public bool enableKeyboardTest = true;

    private Coroutine failCoroutine;

    void Start()
    {
        ShowMoleRat();
    }

    void Update()
    {
        if (!enableKeyboardTest)
            return;

        // Press 1: Correct burger
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            OnBurgerSuccess();
        }

        // Press 2: Wrong burger
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            OnBurgerFail();
        }
    }

    public void ShowMoleRat()
    {
        if (moleRat != null)
            moleRat.SetActive(true);

        if (skeletonMage != null)
            skeletonMage.SetActive(false);
    }

    public void OnBurgerSuccess()
    {
        Debug.Log("Burger Correct!");

        // Play Mole Rat happy animation
        if (moleRatAnimator != null &&
            moleRat != null &&
            moleRat.activeInHierarchy)
        {
            moleRatAnimator.SetTrigger("Success");
        }

        // Blue fireworks
        if (successFireworks != null)
        {
            successFireworks.Play();
        }

        // Yellow fireworks
        if (yellowFireworks != null)
        {
            yellowFireworks.Play();
        }
    }

    public void OnBurgerFail()
    {
        Debug.Log("Burger Wrong!");

        if (failCoroutine != null)
            StopCoroutine(failCoroutine);

        failCoroutine = StartCoroutine(FailRoutine());
    }

    private IEnumerator FailRoutine()
    {
        if (moleRat != null)
            moleRat.SetActive(false);

        if (skeletonMage != null)
            skeletonMage.SetActive(true);

        yield return new WaitForSeconds(failDuration);

        ShowMoleRat();
        failCoroutine = null;
    }
}
