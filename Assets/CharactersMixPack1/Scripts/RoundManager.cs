

using UnityEngine;

using System.Collections.Generic;



public class RoundManager : MonoBehaviour

{

    [Header("Conveyor Settings")]

    public ConveyorPieceLoop belt;



    [Header("Six Round Timer Settings")]

    public float[] roundTimes =

    {

        10f, 10f, 25f, 10f, 20f, 25f

    };



    [Header("Current Round")]

    public int currentRound = 0;



    public float remainingTime;



    private bool isCounting = false;

    private bool isTransporting = false;

    private bool waitingForJudgement = false;

    



    private List<ConveyorFoodMover> foods =

        new List<ConveyorFoodMover>();



    void Start()

    {

        if (belt == null)

        {

            UnityEngine.Debug.LogError(

                "Please assign Conveyor Belt!"

            );

            enabled = false;

            return;

        }



        StartRound();

    }



    void Update()

    {

        // 1. Countdown 

        if (isCounting)

        {

            remainingTime -= Time.deltaTime;



            if (remainingTime <= 0f)

            {

                remainingTime = 0f;

                StartTransport();

            }

        }



        // 2. Check whether all food has arrived 

        if (isTransporting && foods.Count > 0)

        {

            bool allArrived = true;



            foreach (ConveyorFoodMover food in foods)

            {

                if (food != null && food.IsMoving)

                {

                    allArrived = false;

                    break;

                }

            }



            if (allArrived)

            {

                belt.StopBelt();



                isTransporting = false;

                waitingForJudgement = true;



                UnityEngine.Debug.Log(

                    "All food arrived. Ready for judgement!"

                );

            }

        }

    }



    // Register food when player places it 

    public void RegisterFood(ConveyorFoodMover food)

    {

        if (food == null) return;



        if (!isCounting)

        {

            UnityEngine.Debug.LogWarning(

                "Food cannot be registered now!"

            );

            return;

        }



        if (!foods.Contains(food))

        {

            foods.Add(food);

        }

    }



    // Start a new round 

    public void StartRound()

    {

        if (currentRound >= roundTimes.Length)

        {

            FinishGame();

            return;

        }



        foods.Clear();



        belt.StopBelt();



        remainingTime = roundTimes[currentRound];



        isCounting = true;

        isTransporting = false;

        waitingForJudgement = false;



        UnityEngine.Debug.Log(

            "ROUND " + (currentRound + 1) +

            " STARTED! Time: " + remainingTime + " seconds"

        );

    }



    // Start conveyor and food movement 

    void StartTransport()

    {

        isCounting = false;

        isTransporting = true;



        belt.StartBelt();



        foreach (ConveyorFoodMover food in foods)

        {

            if (food != null)

            {

                food.StartMoving();

            }

        }



        UnityEngine.Debug.Log("TRANSPORT STARTED!");

    }



    // Call this after order judgement 

    public void CompleteRound()

    {

        if (!waitingForJudgement)

        {

            UnityEngine.Debug.LogWarning(

                "Round is not ready to complete!"

            );

            return;

        }



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



    // Finish the six-round game 

    void FinishGame()

    {

        



        isCounting = false;

        isTransporting = false;

        waitingForJudgement = false;



        if (belt != null)

        {

            belt.StopBelt();

        }



        UnityEngine.Debug.Log("GAME FINISHED!");

    }

}