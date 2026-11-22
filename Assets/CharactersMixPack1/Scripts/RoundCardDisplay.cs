
using UnityEngine;

public class RoundCardDisplay : MonoBehaviour
{
    [Header("Music Manager")]
    public MusicTransitionManager musicManager;

    [Header("Six Round Panels")]
    public GameObject[] roundPanels = new GameObject[6];

    [Header("Round Time Signatures")]
    [Tooltip("4 = 4/4 music, 3 = 3/4 music")]
    public int[] roundMeters = new int[6]
    {
        4, 4, 3, 3, 4, 3
    };

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

        if (Input.GetKeyDown(KeyCode.Alpha1))
            ShowRound(1);

        if (Input.GetKeyDown(KeyCode.Alpha2))
            ShowRound(2);

        if (Input.GetKeyDown(KeyCode.Alpha3))
            ShowRound(3);

        if (Input.GetKeyDown(KeyCode.Alpha4))
            ShowRound(4);

        if (Input.GetKeyDown(KeyCode.Alpha5))
            ShowRound(5);

        if (Input.GetKeyDown(KeyCode.Alpha6))
            ShowRound(6);
    }

    public void ShowRound(int roundNumber)
    {
        // Check valid round number.
        if (roundPanels == null ||
            roundNumber < 1 ||
            roundNumber > roundPanels.Length)
        {
            Debug.LogWarning(
                "Invalid round number: " + roundNumber
            );
            return;
        }

        currentRound = roundNumber;

        // Show only the current round card.
        for (int i = 0; i < roundPanels.Length; i++)
        {
            if (roundPanels[i] != null)
            {
                roundPanels[i].SetActive(
                    i == roundNumber - 1
                );
            }
        }

        // Switch music according to the round meter.
        int meterIndex = roundNumber - 1;

        if (musicManager != null &&
            roundMeters != null &&
            meterIndex < roundMeters.Length)
        {
            int meter = roundMeters[meterIndex];

            if (meter == 4)
            {
                musicManager.PlayFourFour();
            }
            else if (meter == 3)
            {
                musicManager.PlayThreeFour();
            }
            else
            {
                Debug.LogWarning(
                    "Unsupported meter in round " +
                    roundNumber + ": " + meter
                );
            }
        }

        Debug.Log(
            "Showing Round " + currentRound
        );
    }

    public int GetCurrentRound()
    {
        return currentRound;
    }
}
