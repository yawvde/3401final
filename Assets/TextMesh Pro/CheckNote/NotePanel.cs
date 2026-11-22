using UnityEngine;
using UnityEngine.Events;

public class NotePanel : MonoBehaviour
{
    [Header("Match Event")]
    public UnityEvent onAllNotesMatched;

    [Header("Current Round (0 = Round 1)")]
    [SerializeField] private int currentRound = 0;

    private int currentIndex = 0;

    // Six fixed correct answers.
    private readonly NoteType[][] roundAnswers =
    {
        // Round 1: Omelet Omelet Patty Patty Omelet
        new NoteType[]
        {
            NoteType.Note5, NoteType.Note5,
            NoteType.Note7, NoteType.Note7,
            NoteType.Note5
        },

        // Round 2: Patty Patty Omelet Tomato Patty
        new NoteType[]
        {
            NoteType.Note7, NoteType.Note7,
            NoteType.Note5, NoteType.Note8,
            NoteType.Note7
        },

        // Round 3
        new NoteType[]
        {
            NoteType.Note7, NoteType.Note2,
            NoteType.Note7, NoteType.Note8,
            NoteType.Note2, NoteType.Note4
        },

        // Round 4
        new NoteType[]
        {
            NoteType.Note1, NoteType.Note2,
            NoteType.Note7
        },

        // Round 5
        new NoteType[]
        {
            NoteType.Note8, NoteType.Note5,
            NoteType.Note2, NoteType.Note3,
            NoteType.Note7, NoteType.Note7,
            NoteType.Note7, NoteType.Note7,
            NoteType.Note7, NoteType.Note5,
            NoteType.Note4, NoteType.Note7
        },

        // Round 6
        new NoteType[]
        {
            NoteType.Note4, NoteType.Note4,
            NoteType.Note8, NoteType.Note7,
            NoteType.Note5, NoteType.Note6,
            NoteType.Note7
        }
    };

    public void SetRound(int roundIndex)
    {
        if (roundIndex < 0 ||
            roundIndex >= roundAnswers.Length)
        {
            UnityEngine.Debug.LogError(
                "Invalid round: " + roundIndex
            );
            return;
        }

        currentRound = roundIndex;
        currentIndex = 0;

        UnityEngine.Debug.Log(
            "NotePanel: Round " + (currentRound + 1) +
            " | Required Notes: " + TotalNotes
        );
    }

    public NoteType GetTargetNote(int index)
    {
        if (index < 0 || index >= TotalNotes)
            return NoteType.Note1;

        return roundAnswers[currentRound][index];
    }

    // Compatibility with the existing system.
    public bool CheckNote(NoteType note)
    {
        if (currentIndex >= TotalNotes)
            return false;

        if (GetTargetNote(currentIndex) != note)
            return false;

        currentIndex++;

        if (currentIndex >= TotalNotes)
            onAllNotesMatched?.Invoke();

        return true;
    }

    public void ResetNotes()
    {
        currentIndex = 0;
    }

    public int CurrentRound => currentRound;

    public int TotalNotes =>
        roundAnswers[currentRound].Length;

    public int CurrentIndex => currentIndex;

    public bool IsCompleted =>
        currentIndex >= TotalNotes;
}