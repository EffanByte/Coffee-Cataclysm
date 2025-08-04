using System;
using UnityEngine;
using UnityHFSM;

public enum BeanType { none, brown, white }
public enum PlayerState
{
    IDLE,
    MOVE,
}

public class Player : MonoBehaviour
{
    public static Transform transformPlayer;

    [SerializeField] private float moveSpeed;

    private StateMachine<PlayerState> fsm;

    // NEW
    private HoldingManager heldItemManager;
    private PlayerInteractor playerInteractor;
    GameInput gameInput;


    void Start()
    {
        gameInput = GetComponentInChildren<GameInput>();
        if (GameStateManager.Instance.currentState == GameSceneState.GameBar)
        {
            heldItemManager = GetComponent<HoldingManager>();
            if (heldItemManager == null)
            {
                Debug.LogError("HoldingManager component is missing on Player.");
                return;
            }
            playerInteractor = GetComponent<PlayerInteractor>();
            if (playerInteractor == null)
            {
                Debug.LogError("PlayerInteractor component is missing on Player.");
                return;
            }
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
        gameInput = GetComponentInChildren<GameInput>(); 
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
        playerInteractor.Interact();
    }


}
