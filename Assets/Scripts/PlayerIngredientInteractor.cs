
using System.Diagnostics;
using UnityEngine;

public class PlayerIngredientInteractor : MonoBehaviour
{
    [Header("Round Manager")]
    public RoundManager roundManager;

    [Header("Conveyor Endpoint")]
    public Transform conveyorEndPoint;

    [Header("Interaction Key")]
    public KeyCode interactKey = KeyCode.E;

    [Header("Pickup Settings")]
    public Transform holdPoint;
    public float pickupRange = 10f;

    [Header("Existing Ingredient Pickup")]
    public float rePickupRange = 3f;

    [Header("Placement Settings")]
    public Transform ingredientStack;
    public BoxCollider placementArea;
    public float placementRange = 10f;
    public float stackHeightStep = 0.2f;

    [Header("Throw Settings")]
    public float throwForwardSpeed = 5f;
    public float throwUpSpeed = 5f;

    [Header("Ingredient Size")]
    public float placedIngredientScale = 0.8f;

    private GameObject heldIngredient;

    void Update()
    {
        if (!Input.GetKeyDown(interactKey))
            return;

        if (heldIngredient == null)
        {
            TryPickup();
        }
        else
        {
            TryThrow();
        }
    }

    void TryPickup()
    {
        if (holdPoint == null)
        {
            UnityEngine.Debug.LogError("HoldPoint is missing!");
            return;
        }

        // First, look for an existing ingredient.
        PlacedIngredient[] items =
            FindObjectsOfType<PlacedIngredient>();

        PlacedIngredient nearestItem = null;
        float nearestDistance = rePickupRange;

        foreach (PlacedIngredient item in items)
        {
            if (!item.isOnConveyor)
                continue;

            Vector3 playerPos = transform.position;
            Vector3 itemPos = item.transform.position;

            playerPos.y = 0f;
            itemPos.y = 0f;

            float distance =
                Vector3.Distance(playerPos, itemPos);

            if (distance <= nearestDistance)
            {
                nearestDistance = distance;
                nearestItem = item;
            }
        }

        if (nearestItem != null)
        {
            // Reuse the same existing ingredient.
            heldIngredient = nearestItem.gameObject;
            nearestItem.isOnConveyor = false;

            ConveyorFoodMover mover =
                heldIngredient.GetComponent<ConveyorFoodMover>();

            if (roundManager != null && mover != null)
            {
                roundManager.UnregisterFood(mover);
                mover.StopMoving();
            }

            Rigidbody rb =
                heldIngredient.GetComponent<Rigidbody>();

            if (rb != null)
            {
                rb.isKinematic = true;
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }

            SetCollidersEnabled(heldIngredient, false);

            heldIngredient.transform.SetParent(
                holdPoint, false);

            heldIngredient.transform.localPosition =
                Vector3.zero;

            heldIngredient.transform.localRotation =
                Quaternion.identity;

            heldIngredient.transform.localScale =
                Vector3.one;

            UnityEngine.Debug.Log("Existing ingredient picked up!");
            return;
        }

        // Otherwise create a new ingredient from tray.
        IngredientSource[] sources =
            FindObjectsOfType<IngredientSource>();

        IngredientSource nearestSource = null;
        float closestDistance = pickupRange;

        foreach (IngredientSource source in sources)
        {
            Vector3 playerPos = transform.position;
            Vector3 sourcePos = source.transform.position;

            playerPos.y = 0f;
            sourcePos.y = 0f;

            float distance =
                Vector3.Distance(playerPos, sourcePos);

            if (distance <= closestDistance)
            {
                closestDistance = distance;
                nearestSource = source;
            }
        }

        if (nearestSource == null)
        {
            UnityEngine.Debug.Log("No ingredient nearby!");
            return;
        }

        if (nearestSource.ingredientPrefab == null)
        {
            UnityEngine.Debug.LogError("Ingredient Prefab missing!");
            return;
        }

        heldIngredient = Instantiate(
            nearestSource.ingredientPrefab,
            holdPoint,
            false
        );

        ConveyorFoodMover newMover =
    heldIngredient.GetComponent<ConveyorFoodMover>();

        if (newMover == null)
        {
            newMover =
                heldIngredient.GetComponentInChildren<ConveyorFoodMover>(true);
        }

        if (newMover != null)
        {
            newMover.endPoint = conveyorEndPoint;
            newMover.StopMoving();

            UnityEngine.Debug.Log(
                "CLONE CREATED: " + heldIngredient.name +
                " | Endpoint: " +
                (newMover.endPoint == null
                    ? "NONE"
                    : newMover.endPoint.name)
            );
        }
        else
        {
            UnityEngine.Debug.LogError(
                heldIngredient.name +
                " has no ConveyorFoodMover!"
            );
        }

        heldIngredient.transform.localPosition =
            Vector3.zero;

        heldIngredient.transform.localRotation =
            Quaternion.identity;

        heldIngredient.transform.localScale =
            Vector3.one;

        Rigidbody newRb =
            heldIngredient.GetComponent<Rigidbody>();

        if (newRb != null)
        {
            newRb.isKinematic = true;
            newRb.velocity = Vector3.zero;
            newRb.angularVelocity = Vector3.zero;
        }

        SetCollidersEnabled(heldIngredient, false);

        UnityEngine.Debug.Log("New ingredient created from tray!");
    }

    void TryThrow()
    {
        if (heldIngredient == null)
            return;

        if (holdPoint == null)
        {
            UnityEngine.Debug.LogError("HoldPoint is missing!");
            return;
        }

        GameObject item = heldIngredient;

        // Release the same object from the hand.
        item.transform.SetParent(null, true);

        // Set its world size.
        item.transform.localScale =
            Vector3.one * placedIngredientScale;

        // Begin the throw at the player's hand.
        item.transform.position = holdPoint.position;

        // Keep the ingredient level.
        item.transform.rotation =
            Quaternion.identity;

        // Allow this item to be picked up again.
        PlacedIngredient marker =
            item.GetComponent<PlacedIngredient>();

        if (marker == null)
        {
            marker =
                item.AddComponent<PlacedIngredient>();
        }

        // Existing marker uses this flag for pickup.
        marker.isOnConveyor = true;

        Rigidbody rb = item.GetComponent<Rigidbody>();

        if (rb == null)
        {
            UnityEngine.Debug.LogError(
                "Rigidbody missing on ingredient!"
            );

            // Keep holding if throwing is impossible.
            item.transform.SetParent(holdPoint, false);
            item.transform.localPosition =
                Vector3.zero;

            item.transform.localRotation =
                Quaternion.identity;

            item.transform.localScale =
                Vector3.one;

            marker.isOnConveyor = false;
            return;
        }

        // Reset physics before throwing.
        rb.isKinematic = true;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        rb.useGravity = true;

        // Enable collision before applying velocity.
        SetCollidersEnabled(item, true);

        // Return the item to physics simulation.
        rb.isKinematic = false;

        Vector3 direction =
            transform.forward * throwForwardSpeed +
            Vector3.up * throwUpSpeed;

        rb.velocity = direction;

        // Prepare the thrown ingredient for conveyor movement.
        rb.velocity = direction;

        // Find or add the movement component.
        ConveyorFoodMover foodMover =
            item.GetComponent<ConveyorFoodMover>();

        if (foodMover == null)
        {
            foodMover = item.AddComponent<ConveyorFoodMover>();
        }

        // Assign the destination from the player or GameManager.
        if (roundManager != null && roundManager.foodEndPoint != null)
        {
            foodMover.endPoint = roundManager.foodEndPoint;
        }
        else
        {
            foodMover.endPoint = conveyorEndPoint;
        }

        foodMover.enabled = true;
        foodMover.StopMoving();

        // Check whether this throw is aimed at the placement area.
        bool aimedAtConveyor = false;

        if (placementArea != null)
        {
            RaycastHit hit;
            Vector3 rayStart = item.transform.position;

            if (Physics.Raycast(
                rayStart,
                direction.normalized,
                out hit,
                20f))
            {
                aimedAtConveyor =
                    hit.collider == placementArea;
            }
        }

        UnityEngine.Debug.Log(
            "THROWN FOOD: " + item.name +
            " | Endpoint: " +
            (foodMover.endPoint != null
                ? foodMover.endPoint.name
                : "NONE")
        );

        // Keep the original code below.
        heldIngredient = null;

        UnityEngine.Debug.Log("Ingredient thrown!");

        heldIngredient = null;

        UnityEngine.Debug.Log("Ingredient thrown!");
    }

    void SetCollidersEnabled(
        GameObject obj,
        bool enabled
    )
    {
        Collider[] colliders =
            obj.GetComponentsInChildren<Collider>();

        foreach (Collider col in colliders)
        {
            // Do not enable colliders intentionally
            // disabled in the prefab.
            if (col is SphereCollider)
                continue;

            col.enabled = enabled;
        }
    }
}
