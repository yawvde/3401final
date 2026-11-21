
using UnityEngine;
using System.Collections;

public class ResultFeedbackUI : MonoBehaviour
{
    [Header("Result Texts")]
    public GameObject successText;
    public GameObject failureText;
    public GameObject checkingText;

    [Header("Timer")]
    public TimerUI timerUI;

    [Header("Settings")]
    public float displayDuration = 2f;

    private Coroutine hideCoroutine;

    private void Awake()
    {
        HideAll();
    }

    // 进入鉴别阶段
    public void ShowChecking()
    {
        StopCurrentHide();

        HideAll();

        if (timerUI != null)
            timerUI.StopAndHide();

        if (checkingText != null)
            checkingText.SetActive(true);
    }

    // 鉴别成功
    public void ShowSuccess()
    {
        ShowResult(true);
    }

    // 鉴别失败
    public void ShowFailure()
    {
        ShowResult(false);
    }

    private void ShowResult(bool isSuccess)
    {
        StopCurrentHide();
        HideAll();

        if (isSuccess)
        {
            if (successText != null)
                successText.SetActive(true);
        }
        else
        {
            if (failureText != null)
                failureText.SetActive(true);
        }

        hideCoroutine = StartCoroutine(HideAfterDelay());
    }

    private IEnumerator HideAfterDelay()
    {
        yield return new WaitForSecondsRealtime(displayDuration);

        HideAll();
        hideCoroutine = null;
    }

    private void StopCurrentHide()
    {
        if (hideCoroutine != null)
        {
            StopCoroutine(hideCoroutine);
            hideCoroutine = null;
        }
    }

    private void HideAll()
    {
        if (successText != null)
            successText.SetActive(false);

        if (failureText != null)
            failureText.SetActive(false);

        if (checkingText != null)
            checkingText.SetActive(false);
    }

#if UNITY_EDITOR
    [ContextMenu("TEST - Checking")]
    private void TestChecking()
    {
        ShowChecking();
    }

    [ContextMenu("TEST - Success")]
    private void TestSuccess()
    {
        ShowSuccess();
    }

    [ContextMenu("TEST - Failure")]
    private void TestFailure()
    {
        ShowFailure();
    }
#endif
}
