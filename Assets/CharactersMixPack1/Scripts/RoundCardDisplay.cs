
using UnityEngine;

public class RoundCardDisplay : MonoBehaviour
{
    [Header("Six Round Panels")]
    public GameObject[] roundPanels = new GameObject[6];

    [Header("Testing")]
    public bool enableKeyboardTest = false;

    private int currentRound = 1;

    private void Start()
    {
        ShowRound(1);
    }

    private void Update()
    {
        if (!enableKeyboardTest)
            return;

        if (Input.GetKeyDown(KeyCode.Alpha1)) ShowRound(1);
        if (Input.GetKeyDown(KeyCode.Alpha2)) ShowRound(2);
        if (Input.GetKeyDown(KeyCode.Alpha3)) ShowRound(3);
        if (Input.GetKeyDown(KeyCode.Alpha4)) ShowRound(4);
        if (Input.GetKeyDown(KeyCode.Alpha5)) ShowRound(5);
        if (Input.GetKeyDown(KeyCode.Alpha6)) ShowRound(6);
    }

    public void ShowRound(int roundNumber)
    {
        if (roundNumber < 1 ||
            roundNumber > roundPanels.Length)
        {
            UnityEngine.Debug.LogWarning(
                "Invalid round number: " + roundNumber
            );
            return;
        }

        currentRound = roundNumber;

        for (int i = 0; i < roundPanels.Length; i++)
        {
            if (roundPanels[i] != null)
            {
                roundPanels[i].SetActive(
                    i == roundNumber - 1
                );
            }
        }

        UnityEngine.Debug.Log(
            "Showing round cards: " + currentRound
        );
    }
}
