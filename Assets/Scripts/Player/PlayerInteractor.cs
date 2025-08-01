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
        TryUseCoffeeMaker();
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

            // 1. If blend is ready, give to player
            if (coffeeMaker.IsBlendReady())
            {
                coffeeMaker.GiveBlendToPlayer();
                return true;
            }

            // 2. Otherwise, try to start brewing
            var heldItem = holdingManager.HeldItem;

            if (heldItem == HeldItemType.CoffeeBeanBrown || heldItem == HeldItemType.CoffeeBeanWhite)
            {
                coffeeMaker.InsertBean(heldItem);
                holdingManager.DropItem();
                return true;
            }

            return false; // nothing valid to interact with
        }

        return false;
    }



}
