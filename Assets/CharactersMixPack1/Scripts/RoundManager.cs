using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RoundManager : MonoBehaviour
{
    public static bool AllArrived;

    [Header("Conveyor Settings")]
    public ConveyorPieceLoop belt;

    [Header("Food Destination")]
    public Transform foodEndPoint;

    [Header("Six Round Timer Settings")]
    public float[] roundTimes =
    {
        10f, 10f, 25f, 10f, 20f, 25f
    };

    [Header("Current Round")]
    public int currentRound = 0;
    public float remainingTime;

    [Header("Empty Conveyor Settings")]
    public float emptyTransportDuration = 3f;

    [Header("Judgement Settings")]
    public float resultDisplayDuration = 2f;

    [Header("Testing")]
    public bool autoNextRoundForTesting = false;

    private float emptyTransportTimer = 0f;

    private bool transportingEmpty = false;
    private bool isCounting = false;
    private bool isTransporting = false;
    private bool waitingForJudgement = false;

    private Coroutine finishRoutine;

    private List<ConveyorFoodMover> foods =
        new List<ConveyorFoodMover>();

    public bool CanRegisterFood
    {
        get { return isCounting; }
    }

    public bool IsTransporting
    {
        get { return isTransporting; }
    }

    private void Start()
    {
        if (belt == null)
        {
            UnityEngine.Debug.LogError(
                "RoundManager: Please assign Belt!"
            );
            enabled = false;
            return;
        }

        if (foodEndPoint == null)
        {
            UnityEngine.Debug.LogError(
                "RoundManager: Please assign Food End Point!"
            );
            enabled = false;
            return;
        }

        if (NoteTrigger.Instance == null)
        {
            UnityEngine.Debug.LogError(
                "RoundManager: NoteTrigger is missing!"
            );
            enabled = false;
            return;
        }

        StartRound();
    }

    private void Update()
    {
        // Preparation countdown
        if (isCounting)
        {
            remainingTime -= Time.deltaTime;

            if (remainingTime <= 0f)
            {
                remainingTime = 0f;
                StartTransport();
            }
        }

        // Empty conveyor
        if (isTransporting && transportingEmpty)
        {
            emptyTransportTimer -= Time.deltaTime;

            if (emptyTransportTimer <= 0f)
            {
                FinishTransport(
                    "EMPTY TRANSPORT FINISHED!"
                );
            }
        }

        // Transport with ingredients
        if (isTransporting &&
            !transportingEmpty &&
            foods.Count > 0)
        {
            bool allReached = true;

            foreach (ConveyorFoodMover food in foods)
            {
                if (food == null || !food.HasReachedEnd)
                {
                    allReached = false;
                    break;
                }
            }

            AllArrived = allReached;

            if (allReached)
            {
                FinishTransport(
                    "ALL FOOD ARRIVED!"
                );
            }
        }
    }

    public void RegisterFood(ConveyorFoodMover food)
    {
        if (food == null || !isCounting)
            return;

        if (!foods.Contains(food))
        {
            foods.Add(food);

            UnityEngine.Debug.Log(
                food.name + " registered successfully!"
            );
        }
    }

    public void UnregisterFood(ConveyorFoodMover food)
    {
        if (food == null)
            return;

        foods.Remove(food);
        food.StopMoving();
    }

    // Start a new round.
    public void StartRound()
    {
        if (currentRound >= roundTimes.Length)
        {
            FinishGame();
            return;
        }

        foods.Clear();

        AllArrived = false;
        transportingEmpty = false;
        emptyTransportTimer = 0f;

        isCounting = true;
        isTransporting = false;
        waitingForJudgement = false;

        belt.StopBelt();

        remainingTime = roundTimes[currentRound];

        // Load the correct answer for this round.
        if (NoteTrigger.Instance != null)
        {
            NoteTrigger.Instance.PrepareRound(currentRound);
        }

        UnityEngine.Debug.Log(
            "ROUND " + (currentRound + 1) +
            " STARTED! Time: " +
            remainingTime + " seconds"
        );
    }

    // Begin transportation.
    private void StartTransport()
    {
        isCounting = false;
        isTransporting = true;
        AllArrived = false;

        UnityEngine.Debug.Log(
            "Registered food count: " + foods.Count
        );

        belt.StartBelt();

        UnityEngine.Debug.Log("TRANSPORT STARTED!");

        // Empty round still moves the belt.
        if (foods.Count == 0)
        {
            transportingEmpty = true;
            emptyTransportTimer = emptyTransportDuration;

            UnityEngine.Debug.Log(
                "EMPTY CONVEYOR STARTED!"
            );

            return;
        }

        transportingEmpty = false;

        foreach (ConveyorFoodMover food in foods)
        {
            if (food == null)
                continue;

            food.endPoint = foodEndPoint;
            food.enabled = true;
            food.StartMoving();
        }
    }

    // End transport and begin judgement.
    private void FinishTransport(string message)
    {
        if (!isTransporting)
            return;

        belt.StopBelt();

        isTransporting = false;
        transportingEmpty = false;
        waitingForJudgement = true;

        UnityEngine.Debug.Log(message);

        if (finishRoutine != null)
        {
            StopCoroutine(finishRoutine);
        }

        finishRoutine =
            StartCoroutine(FinishRoundRoutine());
    }

    private IEnumerator FinishRoundRoutine()
    {
        // Give the trigger time to receive final physics events.
        yield return new WaitForFixedUpdate();
        yield return new WaitForFixedUpdate();

        if (!waitingForJudgement)
            yield break;

        // Compare the complete sequence once.
        if (NoteTrigger.Instance != null)
        {
            NoteTrigger.Instance.CheckResult();
        }
        else
        {
            UnityEngine.Debug.LogError(
                "NoteTrigger is missing!"
            );
        }

        // Let the success/failure message stay visible.
        yield return new WaitForSeconds(
            Mathf.Max(0f, resultDisplayDuration)
        );

        if (waitingForJudgement)
        {
            CompleteRound();
        }

        finishRoutine = null;
    }

    // Called after judgement.
    public void CompleteRound()
    {
        if (!waitingForJudgement)
            return;

        waitingForJudgement = false;

        // Remove previous round's transported ingredients.
        foreach (ConveyorFoodMover food in foods)
        {
            if (food != null)
            {
                Destroy(food.gameObject);
            }
        }

        foods.Clear();

        currentRound++;

        if (currentRound >= roundTimes.Length)
        {
            FinishGame();
        }
        else
        {
            StartRound();
        }
    }

    private void FinishGame()
    {
        isCounting = false;
        isTransporting = false;
        transportingEmpty = false;
        waitingForJudgement = false;

        AllArrived = false;

        if (belt != null)
            belt.StopBelt();

        UnityEngine.Debug.Log("GAME FINISHED!");
    }
}