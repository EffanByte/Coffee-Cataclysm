using UnityEngine;

public enum HeldItemType
{
    None,
    CoffeeBeanBrown,
    CoffeeBeanWhite,
    Cup,
    WaterJug,
    // Add more as needed
}

public class HoldingManager : MonoBehaviour
{
    public HeldItemType HeldItem { get; private set; } = HeldItemType.None;
    public bool IsHoldingItem => HeldItem != HeldItemType.None;

    [SerializeField] private HeldItemVisualizer visualizer;
    [SerializeField] private float interactionRange = 3f;
    [SerializeField] private float interactionConeAngle = 40f;

    public void TryPickupInFront()
    {
        if (IsHoldingItem)
        {
            Debug.Log("Already holding an item.");
            return;
        }

        Collider[] hits = Physics.OverlapSphere(transform.position, interactionRange);

        Transform bestCandidate = null;
        float bestAngle = interactionConeAngle; // smaller is better

        foreach (var hit in hits)
        {
            if (hit.CompareTag("BrownBeanPot") || hit.CompareTag("WhiteBeanPot"))
            {
                Vector3 toTarget = (hit.transform.position - transform.position).normalized;
                float angle = Vector3.Angle(transform.forward, toTarget);
                float distance = Vector3.Distance(transform.position, hit.transform.position);

                if (angle < interactionConeAngle * 0.5f && distance <= interactionRange)
                {
                    if (angle < bestAngle)
                    {
                        bestAngle = angle;
                        bestCandidate = hit.transform;
                    }
                }
            }
        }

        if (bestCandidate == null)
        {
            Debug.Log("No valid item in front to pick up.");
            return;
        }

        // Identify type
        if (bestCandidate.CompareTag("BrownBeanPot"))
        {
            PickUpItem(HeldItemType.CoffeeBeanBrown);
        }
        else if (bestCandidate.CompareTag("WhiteBeanPot"))
        {
            PickUpItem(HeldItemType.CoffeeBeanWhite);
        }
    }


    public void PickUpItem(HeldItemType item)
    {
        HeldItem = item;
        visualizer?.Show(item);
        Debug.Log($"Picked up: {item}");
    }

    public void DropItem()
    {
        Debug.Log($"Dropped: {HeldItem}");
        HeldItem = HeldItemType.None;
        visualizer?.Hide();
    }
}
