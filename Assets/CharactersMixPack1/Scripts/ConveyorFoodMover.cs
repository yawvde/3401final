using UnityEngine;

public class ConveyorFoodMover : MonoBehaviour
{
    [Header("Movement Settings")]
    public Transform endPoint;
    public float moveSpeed = 2f;

    [Header("Movement Status")]
    public bool IsMoving = false;

    public bool HasReachedEnd { get; private set; } = false;

    private Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (!IsMoving)
            return;

        if (endPoint == null)
            return;

        transform.position = Vector3.MoveTowards(
            transform.position,
            endPoint.position,
            moveSpeed * Time.deltaTime
        );

        float distance = Vector3.Distance(
            transform.position,
            endPoint.position
        );

        if (distance <= 0.05f)
        {
            transform.position = endPoint.position;

            IsMoving = false;
            HasReachedEnd = true;

            UnityEngine.Debug.Log(
                gameObject.name +
                " REALLY reached EndPoint!"
            );
        }
    }

    public void StartMoving()
    {
        if (endPoint == null)
        {
            UnityEngine.Debug.LogError(
                gameObject.name +
                ": EndPoint is missing!"
            );
            return;
        }

        if (moveSpeed <= 0f)
        {
            UnityEngine.Debug.LogError(
                gameObject.name +
                ": Move Speed must be greater than 0!"
            );
            return;
        }

        // Ensure this component can run Update().
        enabled = true;

        if (rb == null)
            rb = GetComponent<Rigidbody>();

        // Stop throw physics before conveyor transport.
        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true;
        }

        HasReachedEnd = false;
        IsMoving = true;

        UnityEngine.Debug.Log(
            gameObject.name +
            " STARTED MOVING!"
        );
    }

    public void StopMoving()
    {
        IsMoving = false;
        HasReachedEnd = false;
    }
}