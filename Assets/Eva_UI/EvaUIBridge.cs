
using UnityEngine;

public class EvaUIBridge : MonoBehaviour
{
    [Header("Eva UI References")]
    public ScoreLifeUI scoreLifeUI;
    public ResultFeedbackUI resultFeedbackUI;
    public TimerUI timerUI;
    public EndGameUI endGameUI;

    [Header("Game Settings")]
    [Tooltip("Total number of rounds after removing old Round 3")]
    public int totalGameRounds = 5;

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

    // Called when a new round begins.
    public void BeginRound(float duration)
    {
        if (gameEnded)
            return;

        roundResolved = false;

        if (timerUI != null)
        {
            timerUI.ShowTimer();
            timerUI.SetTime(duration);
        }
    }

    // Update remaining round time.
    public void UpdateRemainingTime(float seconds)
    {
        if (roundResolved || gameEnded)
            return;

        if (timerUI != null)
        {
            timerUI.SetTime(seconds);
        }
    }

    // Called when the conveyor starts checking.
    public void BeginChecking()
    {
        if (roundResolved || gameEnded)
            return;

        if (timerUI != null)
        {
            timerUI.HideTimer();
        }

        if (resultFeedbackUI != null)
        {
            resultFeedbackUI.ShowChecking();
        }
    }

    // Called by the note recognition system.
    public void ResolveRound(bool isCorrect)
    {
        if (roundResolved || gameEnded)
            return;

        roundResolved = true;

        if (isCorrect)
        {
            if (scoreLifeUI != null)
            {
                scoreLifeUI.AddSuccess();
            }

            if (resultFeedbackUI != null)
            {
                resultFeedbackUI.ShowSuccess();
            }
        }
        else
        {
            if (scoreLifeUI != null)
            {
                scoreLifeUI.LoseLife();
            }

            if (resultFeedbackUI != null)
            {
                resultFeedbackUI.ShowFailure();
            }
        }

        if (IsGameOver)
        {
            Debug.Log(
                "GAME OVER: Three failed rounds."
            );
        }
    }

    // Called when the game ends.
    public void FinishGame()
    {
        if (gameEnded)
            return;

        gameEnded = true;

        if (timerUI != null)
        {
            timerUI.HideTimer();
        }

        if (endGameUI == null)
        {
            Debug.LogWarning(
                "EvaUIBridge: EndGameUI is missing!"
            );
            return;
        }

        // Three failures: Game Over.
        if (IsGameOver)
        {
            endGameUI.ShowGameOver();
            return;
        }

        // Final score based on five rounds.
        int successes = scoreLifeUI != null
            ? scoreLifeUI.SuccessfulRounds
            : 0;

        int totalRounds = Mathf.Max(1, totalGameRounds);

        successes = Mathf.Clamp(
            successes,
            0,
            totalRounds
        );

        int earnedStars = Mathf.FloorToInt(
            (float)successes / totalRounds * 3f
        );

        earnedStars = Mathf.Clamp(
            earnedStars,
            0,
            3
        );

        Debug.Log(
            "GAME COMPLETE! Successes: " +
            successes + "/" + totalRounds +
            ", Stars: " + earnedStars
        );

        endGameUI.ShowComplete(earnedStars);
    }
}
