using UnityEngine;

public class FoodPlacementZone : MonoBehaviour
{
    [Header("Round Manager")]
    public RoundManager roundManager;

    // Detect food inside conveyor area
    private void OnTriggerStay(Collider other)
    {
        if (roundManager == null)
            return;
        if (!roundManager.CanRegisterFood)
            return;

        // Find ingredient marker
        PlacedIngredient marker =
            other.GetComponentInParent<PlacedIngredient>();

        if (marker == null)
            return;

        // Only accept ingredients released by player
        if (!marker.isOnConveyor)
            return;

        // Find food movement script
        ConveyorFoodMover mover =
            marker.GetComponent<ConveyorFoodMover>();

        if (mover == null)
            return;

        // Register food to RoundManager
        roundManager.RegisterFood(mover);
    }
}