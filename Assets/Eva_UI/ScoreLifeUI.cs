
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ScoreLifeUI : MonoBehaviour
{
    [Header("Star Images")]
    public UnityEngine.UI.Image[] stars;

    [Header("Progress Bar")]
    public UnityEngine.UI.Image progressFill;

    [Header("Heart Images")]
    public TMP_Text[] hearts;

    [Header("Game Settings")]
    public int totalRounds = 6;
    public int maxLives = 3;

    [Header("Colors")]
    public Color activeStarColor = Color.yellow;
    public Color inactiveStarColor = Color.gray;

    public Color activeHeartColor = Color.red;
    public Color inactiveHeartColor = Color.gray;

    private int successfulRounds;
    private int currentLives;

    public int CurrentLives => currentLives;
    public int SuccessfulRounds => successfulRounds;
    public bool IsGameOver => currentLives <= 0;

    private void Start()
    {
        ResetUI();
    }

    public void AddSuccess()
    {
        if (IsGameOver || successfulRounds >= totalRounds)
            return;

        successfulRounds++;
        UpdateUI();

        UnityEngine.Debug.Log("SUCCESS! Total success: " + successfulRounds);
    }

    public void LoseLife()
    {
        if (IsGameOver)
            return;

        currentLives--;
        UpdateUI();

        UnityEngine.Debug.Log("FAIL! Lives left: " + currentLives);
        if (IsGameOver)
        {
            UnityEngine.Debug.Log("GAME OVER!");
        }
    }

    public void ResetUI()
    {
        successfulRounds = 0;
        currentLives = maxLives;
        UpdateUI();
    }

    private void UpdateUI()
    {
        float progress = (float)successfulRounds /
                         Mathf.Max(1, totalRounds);

        if (progressFill != null)
        {
            progressFill.fillAmount = progress;
        }

        int earnedStars = Mathf.FloorToInt(progress * 3f);

        if (stars != null)
        {
            for (int i = 0; i < stars.Length; i++)
            {
                if (stars[i] != null)
                {
                    stars[i].color = i < earnedStars
                        ? activeStarColor
                        : inactiveStarColor;
                }
            }
        }

        if (hearts != null)
        {
            for (int i = 0; i < hearts.Length; i++)
            {
                if (hearts[i] != null)
                {
                    hearts[i].color = i < currentLives
                        ? activeHeartColor
                        : inactiveHeartColor;
                }
            }
        }
    }

#if UNITY_EDITOR
    [ContextMenu("TEST - Add Success")]
    private void TestAddSuccess()
    {
        AddSuccess();
    }

    [ContextMenu("TEST - Lose Life")]
    private void TestLoseLife()
    {
        LoseLife();
    }

    [ContextMenu("TEST - Reset UI")]
    private void TestResetUI()
    {
        ResetUI();
    }
#endif

}
