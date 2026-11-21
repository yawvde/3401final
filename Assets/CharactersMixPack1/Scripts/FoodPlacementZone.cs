using UnityEngine;

public class FoodPlacementZone : MonoBehaviour
{
    [Header("Round Manager")]
    public RoundManager roundManager;

    [Header("Debug")]
    public bool showDebug = true;

    private void OnTriggerEnter(Collider other)
    {
        TryRegisterFood(other);
    }

    private void OnTriggerStay(Collider other)
    {
        TryRegisterFood(other);
    }

    private void TryRegisterFood(Collider other)
    {
        if (roundManager == null)
            return;

        if (!roundManager.CanRegisterFood)
            return;

        // Find the Rigidbody that owns this collider.
        Rigidbody rb = other.attachedRigidbody;

        if (rb == null)
            return;

        // Only accept released, physics-driven food.
        // Food still held by the player is kinematic.
        if (rb.isKinematic)
            return;

        GameObject foodObject = rb.gameObject;

        // Find movement component.
        ConveyorFoodMover mover =
            foodObject.GetComponent<ConveyorFoodMover>();

        if (mover == null)
        {
            mover =
                foodObject.GetComponentInChildren
                    <ConveyorFoodMover>(true);
        }

        if (mover == null)
        {
            if (showDebug)
            {
                UnityEngine.Debug.LogWarning(
                    "ZONE: Food mover missing on " +
                    foodObject.name
                );
            }

            return;
        }

        // Check the ingredient marker if available.
        PlacedIngredient marker =
            foodObject.GetComponent<PlacedIngredient>();

        if (marker == null)
        {
            marker =
                foodObject.GetComponentInChildren
                    <PlacedIngredient>(true);
        }

        if (marker != null && !marker.isOnConveyor)
            return;

        // Register only during the preparation phase.
        roundManager.RegisterFood(mover);
    }
}