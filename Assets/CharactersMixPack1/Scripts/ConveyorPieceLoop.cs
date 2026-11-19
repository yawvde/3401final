

using System.Diagnostics;
using UnityEngine;



public class ConveyorPieceLoop : MonoBehaviour

{

    public Transform[] pieces;



    public float moveSpeed = 1f;

    public float pieceSpacing = 0.5f;



    public bool isMoving = false;



    // 勾选后沿 Z 轴移动，否则沿 X 轴移动 

    public bool moveAlongZ = true;



    private Vector3[] originalPositions;

    private float startCoordinate;

    private float beltLength;

    private float distanceTravelled;



    void Start()

    {

        if (pieces == null || pieces.Length == 0)

        {

            UnityEngine.Debug.LogError("Please assign belt pieces!");

            enabled = false;

            return;

        }



        originalPositions = new Vector3[pieces.Length];



        startCoordinate = float.MaxValue;

        float maxCoordinate = float.MinValue;



        for (int i = 0; i < pieces.Length; i++)

        {

            originalPositions[i] = pieces[i].localPosition;



            float coordinate = moveAlongZ

                ? originalPositions[i].z

                : originalPositions[i].x;



            startCoordinate =

                Mathf.Min(startCoordinate, coordinate);



            maxCoordinate =

                Mathf.Max(maxCoordinate, coordinate);

        }



        beltLength =

            maxCoordinate - startCoordinate + pieceSpacing;

    }



    void Update()

    {

        if (!isMoving) return;



        distanceTravelled += moveSpeed * Time.deltaTime;



        for (int i = 0; i < pieces.Length; i++)

        {

            Vector3 position = originalPositions[i];



            float originalCoordinate = moveAlongZ

                ? position.z

                : position.x;



            float newCoordinate = startCoordinate +

                Mathf.Repeat(

                    originalCoordinate - startCoordinate

                    + distanceTravelled,

                    beltLength

                );



            if (moveAlongZ)

                position.z = newCoordinate;

            else

                position.x = newCoordinate;



            pieces[i].localPosition = position;

        }

    }



    public void StartBelt()

    {

        isMoving = true;

    }



    public void StopBelt()

    {

        isMoving = false;

    }

}