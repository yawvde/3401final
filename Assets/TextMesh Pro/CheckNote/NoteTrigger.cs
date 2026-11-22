using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

public class NoteTrigger : MonoBehaviour
{
    public static NoteTrigger Instance;

    [Header("Note Settings")]
    [SerializeField] private NotePanel notePanel;

    [Header("Customer Reaction")]
    [SerializeField] private CustomerReaction customerReaction;

    [Header("Result Events")]
    public UnityEvent onSuccess;
    public UnityEvent onFail;

    private HashSet<RhythmIngredientNote> detectedFoods =
        new HashSet<RhythmIngredientNote>();

    private List<NoteType> collectedNotes =
        new List<NoteType>();

    private bool hasChecked = false;
    private bool isFinished = false;

    public bool HasChecked
    {
        get { return hasChecked; }
        set { hasChecked = value; }
    }

    private void Awake()
    {
        Instance = this;

        Collider zoneCollider = GetComponent<Collider>();

        if (zoneCollider != null)
        {
            zoneCollider.isTrigger = true;
        }
        else
        {
            UnityEngine.Debug.LogError(
                "NoteTrigger: Collider is missing!"
            );
        }

        if (notePanel == null)
        {
            UnityEngine.Debug.LogError(
                "NoteTrigger: NotePanel is missing!"
            );
        }
    }

    // Called at the beginning of every round.
    public void PrepareRound(int roundIndex)
    {
        ResetTrigger();

        if (notePanel != null)
        {
            notePanel.SetRound(roundIndex);

            UnityEngine.Debug.Log(
                "NOTE DETECTOR READY - ROUND " +
                (roundIndex + 1) +
                " | Target Count: " +
                notePanel.TotalNotes
            );
        }
    }

    // Record notes in the order they enter the window.
    private void OnTriggerEnter(Collider other)
    {
        if (isFinished || hasChecked)
            return;

        RhythmIngredientNote foodNote =
            other.GetComponentInParent<RhythmIngredientNote>();

        // Also support ingredient components on child objects.
        if (foodNote == null && other.attachedRigidbody != null)
        {
            foodNote =
                other.attachedRigidbody.GetComponentInChildren
                <RhythmIngredientNote>(true);
        }

        if (foodNote == null)
            return;

        // Prevent one food from being counted multiple times.
        if (!detectedFoods.Add(foodNote))
            return;

        collectedNotes.Add(foodNote.noteType);

        UnityEngine.Debug.Log(
            "DETECTED: " + foodNote.noteType +
            " | Position: " + collectedNotes.Count
        );

        // Do not check here.
        // Wait until RoundManager finishes transportation.
    }

    // Called once after transportation has finished.
    public void CheckResult()
    {
        if (hasChecked)
            return;

        hasChecked = true;
        isFinished = true;

        bool isSuccess = true;

        if (notePanel == null)
        {
            UnityEngine.Debug.LogError(
                "NoteTrigger: NotePanel is missing!"
            );

            isSuccess = false;
        }
        else if (collectedNotes.Count != notePanel.TotalNotes)
        {
            isSuccess = false;
        }
        else
        {
            // Compare the entire sequence exactly.
            for (int i = 0; i < collectedNotes.Count; i++)
            {
                if (collectedNotes[i] != notePanel.GetTargetNote(i))
                {
                    isSuccess = false;
                    break;
                }
            }
        }

        string received = "";

        for (int i = 0; i < collectedNotes.Count; i++)
        {
            if (i > 0)
                received += ", ";

            received += collectedNotes[i].ToString();
        }

        UnityEngine.Debug.Log(
            "RECEIVED SEQUENCE: [" + received + "]"
        );

        if (isSuccess)
        {
            UnityEngine.Debug.Log("PERFECT!");

            onSuccess?.Invoke();

            if (customerReaction != null)
            {
                customerReaction.OnBurgerSuccess();
            }
        }
        else
        {
            UnityEngine.Debug.Log("WRONG RHYTHM!");

            onFail?.Invoke();

            if (customerReaction != null)
            {
                customerReaction.OnBurgerFail();
            }
        }
    }

    // Clear all records for the next round.
    public void ResetTrigger()
    {
        detectedFoods.Clear();
        collectedNotes.Clear();

        hasChecked = false;
        isFinished = false;

        if (notePanel != null)
        {
            notePanel.ResetNotes();
        }
    }
}