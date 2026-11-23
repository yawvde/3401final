
using UnityEngine;

public class BeltOnlyFoodScale : MonoBehaviour
{
    [Header("Belt Detection")]
    public BoxCollider beltFoodArea;

    [Header("Scale Settings")]
    public float groundScale = 0.8f;
    public float beltScale = 0.5f;

    private bool isOnBelt = false;

    private void Start()
    {
        if (beltFoodArea == null)
        {
            GameObject area =
                GameObject.Find("BeltFoodArea");

            if (area != null)
            {
                beltFoodArea =
                    area.GetComponent<BoxCollider>();
            }
        }

        if (beltFoodArea == null)
        {
            Debug.LogWarning(
                "BeltOnlyFoodScale: BeltFoodArea not found!"
            );
        }
    }

    private void Update()
    {
        if (beltFoodArea == null)
            return;

        // When held, leave scale control to pickup script.
        if (transform.parent != null)
        {
            isOnBelt = false;
            return;
        }

        Vector3 localPosition =
            beltFoodArea.transform.InverseTransformPoint(
                transform.position
            );

        bool insideBelt =
            beltFoodArea.center.x -
            beltFoodArea.size.x * 0.5f <= localPosition.x &&
            localPosition.x <= beltFoodArea.center.x +
            beltFoodArea.size.x * 0.5f &&

            beltFoodArea.center.z -
            beltFoodArea.size.z * 0.5f <= localPosition.z &&
            localPosition.z <= beltFoodArea.center.z +
            beltFoodArea.size.z * 0.5f;

        if (insideBelt && !isOnBelt)
        {
            isOnBelt = true;
            transform.localScale =
                Vector3.one * beltScale;
        }
        else if (!insideBelt && isOnBelt)
        {
            isOnBelt = false;
            transform.localScale =
                Vector3.one * groundScale;
        }
    }
}
