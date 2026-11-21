
using UnityEngine;
using TMPro;

public class EndGameUI : MonoBehaviour
{
    [Header("UI References")]
    public GameObject endPanel;
    public TMP_Text endTitle;
    public TMP_Text endDescription;

    [Header("Colors")]
    public Color gameOverColor = Color.red;
    public Color completeColor = Color.yellow;

    private void Awake()
    {
        HideEndScreen();
    }

    // 三条生命全部耗尽时调用
    public void ShowGameOver()
    {
        ShowEndScreen(
            "GAME OVER",
            "The performance has ended!",
            gameOverColor
        );
    }

    // 六轮完成时调用
    public void ShowComplete(int earnedStars)
    {
        earnedStars = Mathf.Clamp(earnedStars, 0, 3);

        ShowEndScreen(
            "PERFORMANCE COMPLETE!",
            "Final Rating: " + earnedStars + " / 3 Stars",
            completeColor
        );
    }

    private void ShowEndScreen(
        string title,
        string description,
        Color titleColor)
    {
        if (endTitle != null)
        {
            endTitle.text = title;
            endTitle.color = titleColor;
        }

        if (endDescription != null)
        {
            endDescription.text = description;
        }

        if (endPanel != null)
        {
            endPanel.SetActive(true);
        }
    }

    public void HideEndScreen()
    {
        if (endPanel != null)
        {
            endPanel.SetActive(false);
        }
    }

#if UNITY_EDITOR
    [ContextMenu("TEST - Game Over")]
    private void TestGameOver()
    {
        ShowGameOver();
    }

    [ContextMenu("TEST - Complete 3 Stars")]
    private void TestComplete()
    {
        ShowComplete(3);
    }

    [ContextMenu("TEST - Hide End Screen")]
    private void TestHide()
    {
        HideEndScreen();
    }
#endif
}
