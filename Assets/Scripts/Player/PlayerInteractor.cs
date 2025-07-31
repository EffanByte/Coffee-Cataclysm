using UnityEngine;

public class PlayerInteractor : MonoBehaviour
{
    [SerializeField] private float interactionRange = 2.5f;
    [SerializeField] private float coneAngle = 45f;
    private HoldingManager holdingManager;


    private void Start()
    {
        holdingManager = GetComponent<HoldingManager>();
        if (holdingManager == null)
        {
            Debug.LogError("HoldingManager component is missing on Player.");
            return;
        }
    }
    public void Interact()
    {
        if (TryUseCoffeeMaker()) return;

        // Try to pick up a bean or item
        holdingManager.TryPickupInFront();
    }

    private bool TryUseCoffeeMaker()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, interactionRange);

        foreach (var hit in hits)
        {
            if (!hit.CompareTag("CoffeeMaker"))
                continue;
            CoffeeMakerInteraction coffeeMaker = hit.GetComponent<CoffeeMakerInteraction>();
            var heldItem = holdingManager.HeldItem;
            if (heldItem == HeldItemType.CoffeeBeanBrown || heldItem == HeldItemType.CoffeeBeanWhite)
            {
                Debug.Log($"Starting brewing with item: {heldItem}");
                coffeeMaker.StartBrewing(heldItem);
                holdingManager.DropItem();
                return true;
            }

        }

        return false;
    }

    
}
