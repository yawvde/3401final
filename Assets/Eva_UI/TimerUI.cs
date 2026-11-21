
using UnityEngine;
using TMPro;

public class TimerUI : MonoBehaviour
{
    [Header("UI")]
    public TMP_Text timerText;

    [Header("Test Settings")]
    public bool standaloneTestMode = true;
    public float testDuration = 30f;

    [Header("Colors")]
    public Color normalColor = Color.white;
    public Color warningColor = Color.yellow;
    public Color dangerColor = Color.red;

    private float remainingTime;
    private bool isRunning;

    private void Start()
    {
        if (standaloneTestMode)
        {
            StartTestTimer();
        }
    }

    private void Update()
    {
        // 正式整合时关掉，避免重复计时
        if (!standaloneTestMode || !isRunning)
            return;

        remainingTime -= Time.deltaTime;
        remainingTime = Mathf.Max(0f, remainingTime);

        SetTime(remainingTime);

        if (remainingTime <= 0f)
        {
            isRunning = false;
            UnityEngine.Debug.Log("Timer finished!");
        }
    }

    // 测试场景使用
    public void StartTestTimer()
    {
        remainingTime = Mathf.Max(0f, testDuration);
        isRunning = remainingTime > 0f;

        ShowTimer();
        SetTime(remainingTime);
    }

    // 正式整合时由 RoundManager 调用
    public void SetTime(float seconds)
    {
        if (timerText == null)
            return;

        seconds = Mathf.Max(0f, seconds);

        int totalSeconds = Mathf.CeilToInt(seconds);
        int minutes = totalSeconds / 60;
        int secs = totalSeconds % 60;

        timerText.text = string.Format(
            "{0:00}:{1:00}", minutes, secs);

        if (seconds <= 2f)
            timerText.color = dangerColor;
        else if (seconds <= 5f)
            timerText.color = warningColor;
        else
            timerText.color = normalColor;
    }

    public void ShowTimer()
    {
        if (timerText != null)
            timerText.gameObject.SetActive(true);
    }

    public void HideTimer()
    {
        if (timerText != null)
            timerText.gameObject.SetActive(false);
    }

    // 用来测试“鉴别阶段隐藏倒计时”
    public void StopAndHide()
    {
        isRunning = false;
        HideTimer();
    }

#if UNITY_EDITOR
    [ContextMenu("TEST - Restart Timer")]
    private void TestRestart()
    {
        StartTestTimer();
    }

    [ContextMenu("TEST - Checking Phase")]
    private void TestChecking()
    {
        StopAndHide();
    }
#endif
}
