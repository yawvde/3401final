

using UnityEngine;



public class ConveyorFoodMover : MonoBehaviour

{

    public Transform endPoint;



    public float moveSpeed = 2f;



    public bool IsMoving = false;



    void Update()

    {

        if (Input.GetKeyDown(KeyCode.Space))

        {

            StartMoving();

        }



        if (!IsMoving || endPoint == null)

            return;



        transform.position = Vector3.MoveTowards(

            transform.position,

            endPoint.position,

            moveSpeed * Time.deltaTime

        );



        if (Vector3.Distance(

            transform.position,

            endPoint.position

        ) < 0.02f)

        {

            transform.position = endPoint.position;

            IsMoving = false;



            UnityEngine.Debug.Log("Food reached endpoint!");

        }

    }


    public void StartMoving()

    {

        IsMoving = true;

    }



    public void StopMoving()

    {

        IsMoving = false;

    }

}