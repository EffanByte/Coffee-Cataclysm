using System;
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
        TryServeCoffee();
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
            HeldItemType heldItem = holdingManager.HeldItem;

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

    private void TryServeCoffee()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, interactionRange);

        foreach (var hit in hits)
        {
            if (!hit.CompareTag("Customer"))
                continue;
            Debug.Log("Interacting with Customer");
            AiBar aiBar = hit.GetComponent<AiBar>();
            Debug.Log(holdingManager.IsHoldingItem);
            Debug.Log(aiBar);
            Debug.Log($"AI Bar: {aiBar}, Holding Item: {holdingManager.HeldItem}");
            if (aiBar != null && holdingManager.IsHoldingItem)
            {
                HeldItemType heldItem = holdingManager.HeldItem;
                Debug.Log($"Serving {heldItem} to AI Bar");
                aiBar.ReceiveOrder(heldItem);
                holdingManager.DropItem();
                return;
            }
        }
    }
}
