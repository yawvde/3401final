
using UnityEngine;

public class EvaUIBridge : MonoBehaviour
{
    [Header("Eva UI References")]
    public ScoreLifeUI scoreLifeUI;
    public ResultFeedbackUI resultFeedbackUI;
    public TimerUI timerUI;
    public EndGameUI endGameUI;

    private bool roundResolved = false;
    private bool gameEnded = false;

    public bool IsGameOver
    {
        get
        {
            return scoreLifeUI != null &&
                   scoreLifeUI.IsGameOver;
        }
    }

    // 每轮开始：恢复接收判定，显示倒计时
    public void BeginRound(float duration)
    {
        if (gameEnded) return;

        roundResolved = false;

        if (timerUI != null)
        {
            timerUI.ShowTimer();
            timerUI.SetTime(duration);
        }
    }

    // 显示 RoundManager 提供的真实剩余时间
    public void UpdateRemainingTime(float seconds)
    {
        if (roundResolved || gameEnded) return;

        if (timerUI != null)
            timerUI.SetTime(seconds);
    }

    // 倒计时结束，开始运输与检查
    public void BeginChecking()
    {
        if (roundResolved || gameEnded) return;

        if (timerUI != null)
            timerUI.HideTimer();

        if (resultFeedbackUI != null)
            resultFeedbackUI.ShowChecking();
    }

    // 自动识别系统返回正确或错误
    public void ResolveRound(bool isCorrect)
    {
        if (roundResolved || gameEnded) return;

        roundResolved = true;

        if (isCorrect)
        {
            if (scoreLifeUI != null)
                scoreLifeUI.AddSuccess();

            if (resultFeedbackUI != null)
                resultFeedbackUI.ShowSuccess();
        }
        else
        {
            if (scoreLifeUI != null)
                scoreLifeUI.LoseLife();

            if (resultFeedbackUI != null)
                resultFeedbackUI.ShowFailure();
        }
    }

    // 最终结算：生命归零或完成全部六轮
    public void FinishGame()
    {
        if (gameEnded) return;

        gameEnded = true;

        if (timerUI != null)
            timerUI.HideTimer();

        if (endGameUI == null) return;

        if (IsGameOver)
        {
            endGameUI.ShowGameOver();
        }
        else
        {
            int successes = scoreLifeUI != null
                ? scoreLifeUI.SuccessfulRounds : 0;

            int totalRounds = scoreLifeUI != null
                ? Mathf.Max(1, scoreLifeUI.totalRounds)
                : 6;

            int earnedStars = Mathf.FloorToInt(
                (float)successes / totalRounds * 3f
            );

            endGameUI.ShowComplete(earnedStars);
        }
    }
}
