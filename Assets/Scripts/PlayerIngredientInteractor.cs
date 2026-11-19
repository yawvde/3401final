
using UnityEngine;

public class PlayerIngredientInteractor : MonoBehaviour
{
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

    [Header("Drop Settings")]
    public float dropForwardDistance = 0.8f;
    public float conveyorReach = 2.5f;

    [Header("Ingredient Size")]
    public float placedIngredientScale = 0.8f;

    private GameObject heldIngredient;

    void Update()
    {
        if (!Input.GetKeyDown(interactKey))
            return;

        if (heldIngredient == null)
            TryPickup();
        else
            TryPlace();
    }

    void TryPickup()
    {
        if (holdPoint == null)
        {
            Debug.LogError("HoldPoint is missing!");
            return;
        }

        // First, look for ingredients already placed.
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
            // Reuse the existing object.
            heldIngredient = nearestItem.gameObject;
            nearestItem.isOnConveyor = false;

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

            // Keep the intended hand-held size.
            heldIngredient.transform.localScale =
                Vector3.one;

            Debug.Log("Existing ingredient picked up!");
            return;
        }

        // Otherwise create a new ingredient from a tray.
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
                nearestSource = source;
                closestDistance = distance;
            }
        }

        if (nearestSource == null)
        {
            Debug.Log("No ingredient nearby!");
            return;
        }

        if (nearestSource.ingredientPrefab == null)
        {
            Debug.LogError("Ingredient Prefab missing!");
            return;
        }

        heldIngredient = Instantiate(
            nearestSource.ingredientPrefab,
            holdPoint,
            false
        );

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

        Debug.Log("New ingredient created from tray!");
    }

    void TryPlace()
    {
        if (heldIngredient == null)
            return;

        if (holdPoint == null)
        {
            Debug.LogError("HoldPoint is missing!");
            return;
        }

        // Position slightly in front of the player.
        Vector3 dropPosition =
            holdPoint.position +
            transform.forward * dropForwardDistance;

        bool onConveyor = false;
        Vector3 conveyorPosition = dropPosition;

        if (placementArea != null)
        {
            Transform area = placementArea.transform;

            Vector3 local =
                area.InverseTransformPoint(dropPosition);

            Vector3 center = placementArea.center;
            Vector3 half =
                placementArea.size * 0.5f;

            // Do not teleport the item onto the belt.
            bool insideArea =
                local.x >= center.x - half.x &&
                local.x <= center.x + half.x &&
                local.z >= center.z - half.z &&
                local.z <= center.z + half.z;

            Vector3 playerPos = transform.position;
            Vector3 targetPos = dropPosition;

            playerPos.y = 0f;
            targetPos.y = 0f;

            bool withinReach =
                Vector3.Distance(playerPos, targetPos)
                <= conveyorReach;

            if (insideArea && withinReach)
            {
                onConveyor = true;

                local.y = center.y + half.y;

                conveyorPosition =
                    area.TransformPoint(local);

                conveyorPosition +=
                    Vector3.up * 0.05f;
            }
        }

        PlacedIngredient marker =
            heldIngredient.GetComponent<PlacedIngredient>();

        if (marker == null)
        {
            marker =
                heldIngredient.AddComponent<PlacedIngredient>();
        }

        // Existing script uses this flag to indicate
        // that the ingredient can be picked up again.
        marker.isOnConveyor = true;

        Rigidbody rb =
            heldIngredient.GetComponent<Rigidbody>();

        if (onConveyor)
        {
            if (ingredientStack != null)
            {
                heldIngredient.transform.SetParent(
                    ingredientStack, true);
            }
            else
            {
                heldIngredient.transform.SetParent(null, true);
            }

            heldIngredient.transform.localScale =
                Vector3.one * placedIngredientScale;

            heldIngredient.transform.position =
                conveyorPosition;

            heldIngredient.transform.rotation =
                Quaternion.identity;

            if (rb != null)
            {
                rb.isKinematic = true;
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }

            SetCollidersEnabled(heldIngredient, true);

            Debug.Log("Ingredient placed on conveyor!");
        }
        else
        {
            heldIngredient.transform.SetParent(null, true);

            heldIngredient.transform.localScale =
                Vector3.one * placedIngredientScale;

            heldIngredient.transform.position =
                dropPosition;

            // Place it horizontally before dropping.
            heldIngredient.transform.rotation =
                Quaternion.identity;

            if (rb != null)
            {
                rb.isKinematic = true;
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }

            SetCollidersEnabled(heldIngredient, true);

            if (rb != null)
            {
                rb.useGravity = true;
                rb.isKinematic = false;
            }

            Debug.Log("Ingredient dropped on floor!");
        }

        heldIngredient = null;
    }

    void SetCollidersEnabled(GameObject obj, bool enabled)
    {
        Collider[] colliders =
            obj.GetComponentsInChildren<Collider>();

        foreach (Collider col in colliders)
        {
            col.enabled = enabled;
        }
    }
}
