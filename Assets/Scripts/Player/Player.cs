using System;
using UnityEngine;
using UnityHFSM;

public enum BeanType { none, brown, white }
public enum PlayerState
{
    IDLE,
    MOVE,
    HOLDING,
}

public class Player : MonoBehaviour
{
    public static Transform transformPlayer;

    [SerializeField] private float moveSpeed;
    [SerializeField] private GameInput gameInput;

    private StateMachine<PlayerState> fsm;

    // NEW
    [SerializeField] private HoldingManager heldItemManager;

    void Start()
    {
        heldItemManager = GetComponent<HoldingManager>();
        if (heldItemManager == null)
        {
            Debug.LogError("HoldingManager component is missing on Player.");
            return;
        }

        transformPlayer = transform;

        fsm = new StateMachine<PlayerState>();

        fsm.AddState(PlayerState.IDLE, new IdleState().state);
        fsm.AddState(PlayerState.MOVE, new MoveState(transform, gameInput, moveSpeed).state);

        fsm.SetStartState(PlayerState.IDLE);

        fsm.AddTransition(PlayerState.IDLE, PlayerState.MOVE, _ => gameInput.GetMovementNormalized().magnitude > 0.1f);
        fsm.AddTransition(PlayerState.MOVE, PlayerState.IDLE, _ => gameInput.GetMovementNormalized().magnitude < 0.1f);

        fsm.Init();
    }

    private void AssignEvents()
    {
        gameInput.OnInteractPlayer += InputSystem_OnInteractPlayer;
    }

    private void Awake()
    {
        AssignEvents();
    }
    void Update()
    {
        fsm.OnLogic();
    }

    public bool HasItem() => heldItemManager.IsHoldingItem;
    public HeldItemType GetHeldItem() => heldItemManager.HeldItem;

    private void InputSystem_OnInteractPlayer(object sender, System.EventArgs e)
    {
        // First try item pickup
        if (!heldItemManager.IsHoldingItem)
        {
            heldItemManager.TryPickupInFront();
            return;
        }

        // Then try CoffeeMaker interaction
        TryUseCoffeeMaker();
    }

    private void TryUseCoffeeMaker()
    {
        float range = 2.5f;
        float coneAngle = 45f;

        Collider[] hits = Physics.OverlapSphere(transform.position, range);

        foreach (var hit in hits)
        {
            if (!hit.TryGetComponent(out CoffeeMakerInteraction coffeeMaker))
                continue;

            Vector3 toTarget = (hit.transform.position - transform.position).normalized;
            float angle = Vector3.Angle(transform.forward, toTarget);

            if (angle < coneAngle * 0.5f)
            {
                // If holding a coffee bean, drop it
                HeldItemType heldItem = heldItemManager.HeldItem;

                if (heldItem == HeldItemType.CoffeeBeanBrown || heldItem == HeldItemType.CoffeeBeanWhite)
                {
                    if (coffeeMaker.CanStartBrewing())
                    {
                        coffeeMaker.StartBrewing(heldItem);
                        heldItemManager.DropItem();
                    }
                    return;
                }
            }
        }

        Debug.Log("No coffee maker in front.");
    }


}
