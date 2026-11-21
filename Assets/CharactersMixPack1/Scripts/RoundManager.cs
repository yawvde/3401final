using UnityEngine;
using System.Collections.Generic;

public class RoundManager : MonoBehaviour
{
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

    [Header("Testing")]
    public bool autoNextRoundForTesting = false;

    private float emptyTransportTimer = 0f;
    private bool transportingEmpty = false;

    private bool isCounting = false;
    private bool isTransporting = false;
    private bool waitingForJudgement = false;

    private List<ConveyorFoodMover> foods =
        new List<ConveyorFoodMover>();

    // Used by FoodPlacementZone.
    public bool CanRegisterFood
    {
        get { return isCounting; }
    }

    void Start()
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

        StartRound();
    }

    void Update()
    {
        // 1. Preparation countdown
        if (isCounting)
        {
            remainingTime -= Time.deltaTime;

            if (remainingTime <= 0f)
            {
                remainingTime = 0f;
                StartTransport();
            }
        }

        // 2. Empty conveyor transport
        if (isTransporting && transportingEmpty)
        {
            emptyTransportTimer -= Time.deltaTime;

            if (emptyTransportTimer <= 0f)
            {
                transportingEmpty = false;

                FinishTransport(
                    "EMPTY TRANSPORT FINISHED - ROUND FAILED!"
                );
            }
        }

        // 3. Transport with food
        if (isTransporting &&
            !transportingEmpty &&
            foods.Count > 0)
        {
            bool allArrived = true;

            foreach (ConveyorFoodMover food in foods)
            {
                if (food == null || !food.HasReachedEnd)
                {
                    allArrived = false;
                    break;
                }
            }

            if (allArrived)
            {
                FinishTransport(
                    "All food reached EndPoint!"
                );
            }
        }
    }

    // Called by FoodPlacementZone.
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

    // Called when player picks food up again.
    public void UnregisterFood(ConveyorFoodMover food)
    {
        if (food == null)
            return;

        foods.Remove(food);
        food.StopMoving();
    }

    // Begin current round.
    public void StartRound()
    {
        if (currentRound >= roundTimes.Length)
        {
            FinishGame();
            return;
        }

        foods.Clear();

        transportingEmpty = false;
        emptyTransportTimer = 0f;

        isCounting = true;
        isTransporting = false;
        waitingForJudgement = false;

        belt.StopBelt();

        remainingTime = roundTimes[currentRound];

        UnityEngine.Debug.Log(
            "ROUND " + (currentRound + 1) +
            " STARTED! Time: " +
            remainingTime + " seconds"
        );
    }

    // Start transportation after countdown.
    void StartTransport()
    {
        isCounting = false;
        isTransporting = true;

        UnityEngine.Debug.Log(
            "Registered food count: " + foods.Count
        );

        // Conveyor always starts.
        belt.StartBelt();

        UnityEngine.Debug.Log(
            "TRANSPORT STARTED!"
        );

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

            // Assign endpoint at transport time.
            food.endPoint = foodEndPoint;

            // Enable movement component.
            food.enabled = true;

            // Start moving.
            food.StartMoving();

            UnityEngine.Debug.Log(
                food.name +
                " | Endpoint: " +
                (food.endPoint == null
                    ? "NONE"
                    : food.endPoint.name)
            );
        }
    }

    // Stop conveyor when transport ends.
    void FinishTransport(string message)
    {
        belt.StopBelt();

        isTransporting = false;
        transportingEmpty = false;

        waitingForJudgement = true;

        UnityEngine.Debug.Log(message);

        // Test mode: skip the judgement system.
        if (autoNextRoundForTesting)
        {
            CompleteRound();
        }
    }

    // Called by the future order judgement system.
    public void CompleteRound()
    {
        if (!waitingForJudgement)
            return;

        waitingForJudgement = false;

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

    // Finish the game after six rounds.
    void FinishGame()
    {
        isCounting = false;
        isTransporting = false;
        transportingEmpty = false;
        waitingForJudgement = false;

        if (belt != null)
            belt.StopBelt();

        UnityEngine.Debug.Log("GAME FINISHED!");
    }
}