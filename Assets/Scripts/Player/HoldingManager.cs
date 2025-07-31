using UnityEngine;

public enum HeldItemType
{
    None,
    CoffeeBeanBrown,
    CoffeeBeanWhite,
    Cup,
    WaterJug,
    CoffeeBlendBrown,
    CoffeeBlendWhite,
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
            Debug.Log("Already holding something.");
            return;
        }

        Collider[] hits = Physics.OverlapSphere(transform.position, interactionRange);

        Transform bestTarget = null;
        float bestAngle = interactionConeAngle;

        foreach (var hit in hits)
        {
            if (!hit.CompareTag("BrownBeanPot") && !hit.CompareTag("WhiteBeanPot"))
                continue;

            Vector3 toTarget = (hit.transform.position - transform.position).normalized;
            float angle = Vector3.Angle(transform.forward, toTarget);
            float distance = Vector3.Distance(transform.position, hit.transform.position);

            Debug.Log($"Checking: {hit.name} | Angle: {angle} | Dist: {distance}");

            if (angle < interactionConeAngle * 0.5f && distance <= interactionRange)
            {
                if (angle < bestAngle)
                {
                    bestAngle = angle;
                    bestTarget = hit.transform;
                }
            }
        }

        if (bestTarget == null)
        {
            Debug.Log("No valid item in front to pick up.");
            return;
        }

        if (bestTarget.CompareTag("BrownBeanPot"))
        {
            PickUpItem(HeldItemType.CoffeeBeanBrown);
        }
        else if (bestTarget.CompareTag("WhiteBeanPot"))
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
