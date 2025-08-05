using UnityEngine;
using Playroom;
public enum HeldItemType
{
    None,
    CoffeeBeanBrown,
    CoffeeBeanWhite,
    Cup,
    WaterJug,
    CoffeeBlendBrown, 
    CoffeeBlendWhite,    
    MixedCoffeeBlend    
}


public class HoldingManager : MonoBehaviour
{
    public HeldItemType HeldItem { get; private set; } = HeldItemType.None;
    public bool IsHoldingItem => HeldItem != HeldItemType.None;

    private PlayroomKit _playroomKit;
    private HeldItemVisualizer visualizer;
    [SerializeField] private float interactionRange = 3f;
    [SerializeField] private float interactionConeAngle = 30f;

    private void Awake()
    {
        visualizer = GetComponentInChildren<HeldItemVisualizer>();
    }
    public void TryPickupInFront()
    {
        if (IsHoldingItem)
        {
            Debug.Log("Already holding an item.");
            return;
        }

        Collider[] hits = Physics.OverlapSphere(transform.position, interactionRange);

        Transform bestCandidate = null;

        foreach (var hit in hits)
        {
            if (hit.CompareTag("BrownBeanPot") || hit.CompareTag("WhiteBeanPot"))
            {
                bestCandidate = hit.transform;
            }
        }

        if (bestCandidate == null) return;
        // Identify type
        if (bestCandidate.CompareTag("BrownBeanPot"))
        {
            Debug.Log("Picking up brown coffee bean pot.");
            PickUpItem(HeldItemType.CoffeeBeanBrown);
        }
        else if (bestCandidate.CompareTag("WhiteBeanPot"))
        {
            Debug.Log("Picking up white coffee bean pot.");
            PickUpItem(HeldItemType.CoffeeBeanWhite);
        }
    }



    public void PickUpItem(HeldItemType item)
    {
        HeldItem = item;
        visualizer?.Show(item);
        _playroomKit.RpcCall("HandleHeldItem", item.ToString());
    }

    public void DropItem()
    {
        HeldItem = HeldItemType.None;
        visualizer?.Hide();
    }
}
