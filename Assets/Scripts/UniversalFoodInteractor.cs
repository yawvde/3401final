
using UnityEngine;

// Marks a spawned ingredient as a real, reusable object.
public class RuntimeFood : MonoBehaviour
{
    public bool isHeld = false;
}

public class UniversalFoodInteractor : MonoBehaviour
{
    [Header("Controls")]
    public KeyCode interactKey = KeyCode.E;

    [Header("Hand")]
    public Transform holdPoint;
    public float heldLocalScale = 1f;

    [Header("Pickup")]
    public float sourcePickupRange = 3.5f;
    public float droppedPickupRange = 2.5f;

    [Header("Throw")]
    public float throwForwardSpeed = 9f;
    public float throwUpSpeed = 8f;
    public float throwStartForward = 0.7f;
    public float throwStartUp = 0.4f;
    public float thrownWorldScale = 0.8f;

    private GameObject heldFood;

    void Update()
    {
        if (!Input.GetKeyDown(interactKey))
            return;

        if (heldFood != null)
            ThrowFood();
        else
            PickupFood();
    }

    float FlatDistance(Vector3 a, Vector3 b)
    {
        a.y = 0f;
        b.y = 0f;

        return Vector3.Distance(a, b);
    }

    void PickupFood()
    {
        if (holdPoint == null)
        {
            Debug.LogError("HoldPoint is not assigned!");
            return;
        }

        // Find nearest ingredient source.
        IngredientSource bestSource = null;
        float sourceDistance = sourcePickupRange;

        foreach (IngredientSource source in
                 FindObjectsOfType<IngredientSource>())
        {
            if (source.ingredientPrefab == null)
                continue;

            float d = FlatDistance(
                transform.position,
                source.transform.position
            );

            if (d < sourceDistance)
            {
                sourceDistance = d;
                bestSource = source;
            }
        }

        // Find nearest previously thrown food.
        RuntimeFood bestDropped = null;
        float droppedDistance = droppedPickupRange;

        foreach (RuntimeFood food in
                 FindObjectsOfType<RuntimeFood>())
        {
            if (food.isHeld)
                continue;

            float d = FlatDistance(
                transform.position,
                food.transform.position
            );

            if (d < droppedDistance)
            {
                droppedDistance = d;
                bestDropped = food;
            }
        }

        GameObject target = null;

        // Choose whichever valid target is closer.
        if (bestSource != null &&
            (bestDropped == null ||
             sourceDistance <= droppedDistance))
        {
            target = Instantiate(
                bestSource.ingredientPrefab
            );

            RuntimeFood marker =
                target.GetComponent<RuntimeFood>();

            if (marker == null)
                target.AddComponent<RuntimeFood>();

            Debug.Log(
                "Created: " +
                bestSource.ingredientPrefab.name
            );
        }
        else if (bestDropped != null)
        {
            target = bestDropped.gameObject;

            Debug.Log(
                "Re-picked: " + target.name
            );
        }

        if (target == null)
        {
            Debug.Log("No ingredient nearby!");
            return;
        }

        Rigidbody rb = target.GetComponent<Rigidbody>();

        if (rb == null ||
            target.GetComponent<BoxCollider>() == null)
        {
            Debug.LogError(
                "Food needs Rigidbody and root BoxCollider: " +
                target.name
            );

            // Do not destroy existing dropped food.
            if (target.name.EndsWith("(Clone)") &&
                target.GetComponent<RuntimeFood>() != null &&
                !target.GetComponent<RuntimeFood>().isHeld)
            {
                // Leave it in the scene for inspection.
            }

            return;
        }

        heldFood = target;

        RuntimeFood foodMarker =
            target.GetComponent<RuntimeFood>();

        foodMarker.isHeld = true;

        rb.isKinematic = true;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        SetFoodColliders(target, false);

        target.transform.SetParent(holdPoint, false);
        target.transform.localPosition = Vector3.zero;
        target.transform.localRotation = Quaternion.identity;
        target.transform.localScale =
            Vector3.one * heldLocalScale;

        Debug.Log("Holding: " + target.name);
    }

    void ThrowFood()
    {
        if (heldFood == null)
            return;

        GameObject target = heldFood;

        Rigidbody rb = target.GetComponent<Rigidbody>();

        if (rb == null)
        {
            Debug.LogError("Food Rigidbody missing!");
            return;
        }

        Vector3 forward = transform.forward;
        forward.y = 0f;

        if (forward.sqrMagnitude < 0.001f)
            forward = Vector3.forward;

        forward.Normalize();

        // Separate the ingredient from the hand.
        target.transform.SetParent(null, true);

        target.transform.localScale =
            Vector3.one * thrownWorldScale;

        // Spawn in front of the player to reduce
        // immediate collision with the player.
        target.transform.position =
            holdPoint.position +
            forward * throwStartForward +
            Vector3.up * throwStartUp;

        target.transform.rotation = Quaternion.identity;

        RuntimeFood marker =
            target.GetComponent<RuntimeFood>();

        marker.isHeld = false;

        rb.isKinematic = true;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.useGravity = true;

        SetFoodColliders(target, true);

        rb.isKinematic = false;
        rb.collisionDetectionMode =
            CollisionDetectionMode.ContinuousDynamic;

        rb.velocity =
            forward * throwForwardSpeed +
            Vector3.up * throwUpSpeed;

        heldFood = null;

        Debug.Log("Thrown: " + target.name);
    }

    void SetFoodColliders(
        GameObject target,
        bool enabled
    )
    {
        // Use one main collider on the root.
        Collider[] allColliders =
            target.GetComponentsInChildren<Collider>(true);

        foreach (Collider col in allColliders)
        {
            bool isMainBox =
                col.gameObject == target &&
                col is BoxCollider;

            col.enabled = enabled && isMainBox;
        }
    }
}
