
using UnityEngine;

public class PlayerIngredientInteractor : MonoBehaviour
{
    [Header("Interaction Key")]
    public KeyCode interactKey = KeyCode.E;

    [Header("Pickup Settings")]
    public Transform holdPoint;
    public float pickupRange = 10f;

    [Header("Placement Settings")]
    public Transform ingredientStack;
    public BoxCollider placementArea;
    public float placementRange = 10f;
    public float stackHeightStep = 0.2f;

    private GameObject heldIngredient;

    void Update()
    {
        if (Input.GetKeyDown(interactKey))
        {
            if (heldIngredient == null)
            {
                TryPickup();
            }
            else
            {
                TryPlace();
            }
        }
    }

    void TryPickup()
    {
        if (holdPoint == null)
        {
            Debug.LogError("HoldPoint is missing!");
            return;
        }

        IngredientSource[] sources =
            FindObjectsOfType<IngredientSource>();

        IngredientSource nearest = null;
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
                nearest = source;
                closestDistance = distance;
            }
        }

        if (nearest == null)
        {
            Debug.Log("No ingredient nearby!");
            return;
        }

        if (nearest.ingredientPrefab == null)
        {
            Debug.LogError("Ingredient Prefab missing!");
            return;
        }

        heldIngredient = Instantiate(
            nearest.ingredientPrefab,
            holdPoint,
            false
        );

        heldIngredient.transform.localPosition =
            Vector3.zero;

        heldIngredient.transform.localRotation =
            Quaternion.identity;

        Collider[] colliders =
            heldIngredient.GetComponentsInChildren<Collider>();

        foreach (Collider col in colliders)
        {
            col.enabled = false;
        }

        Debug.Log("Ingredient picked up!");
    }

    void TryPlace()
    {
        if (heldIngredient == null)
            return;

        if (placementArea == null)
        {
            Debug.LogError("Placement Area is missing!");
            return;
        }

        if (ingredientStack == null)
        {
            Debug.LogError("IngredientStack is missing!");
            return;
        }

        // Use the player's hand as the placement reference.
        Vector3 referencePosition = holdPoint.position;

        // Convert the hand position to the area's local space.
        Transform areaTransform = placementArea.transform;

        Vector3 localPoint =
            areaTransform.InverseTransformPoint(referencePosition);

        Vector3 center = placementArea.center;
        Vector3 halfSize = placementArea.size * 0.5f;

        // Keep the position within the conveyor area.
        localPoint.x = Mathf.Clamp(
            localPoint.x,
            center.x - halfSize.x,
            center.x + halfSize.x
        );

        localPoint.z = Mathf.Clamp(
            localPoint.z,
            center.z - halfSize.z,
            center.z + halfSize.z
        );

        // Put the ingredient on the top surface.
        localPoint.y = center.y + halfSize.y;

        Vector3 placePosition =
            areaTransform.TransformPoint(localPoint);

        // Check distance on the horizontal plane.
        Vector3 playerPos = transform.position;
        Vector3 targetPos = placePosition;

        playerPos.y = 0f;
        targetPos.y = 0f;

        float distance =
            Vector3.Distance(playerPos, targetPos);

        if (distance > placementRange)
        {
            Debug.Log("Move closer to the conveyor!");
            return;
        }

        // Remove the ingredient from the player's hand.
        heldIngredient.transform.SetParent(
            ingredientStack,
            true
        );

        // Place it at the calculated conveyor position.
        heldIngredient.transform.position =
            placePosition + Vector3.up * 0.05f;

        heldIngredient.transform.rotation =
            Quaternion.identity;

        heldIngredient = null;

        Debug.Log("Ingredient placed on conveyor!");
    }
}
