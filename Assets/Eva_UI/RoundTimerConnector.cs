
using UnityEngine;

public class RoundTimerConnector : MonoBehaviour
{
    [Header("References")]
    public RoundManager roundManager;
    public TimerUI timerUI;
    public ResultFeedbackUI resultFeedbackUI;

    private bool wasCounting = false;
    private bool checkingStarted = false;

    private void Update()
    {
        if (roundManager == null || timerUI == null)
            return;

        bool isPreparing = roundManager.CanRegisterFood;

        if (isPreparing)
        {
            // A new round has started.
            if (!wasCounting)
            {
                checkingStarted = false;
                timerUI.ShowTimer();
            }

            timerUI.SetTime(roundManager.remainingTime);
        }
        else
        {
            // Countdown has ended.
            timerUI.HideTimer();

            // Transport started.
            if (wasCounting && !checkingStarted)
            {
                checkingStarted = true;

                if (resultFeedbackUI != null)
                {
                    resultFeedbackUI.ShowChecking();
                }
            }
        }

        wasCounting = isPreparing;
    }
}
