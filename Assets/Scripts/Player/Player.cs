using System;
using Playroom;
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
    public Transform transformPlayer;

    [Header("Player Character Speed")]
    [SerializeField] protected float moveSpeed;
    [SerializeField] private Animator animator;

    private StateMachine<PlayerState> fsm;
    private PlayroomKit _playroomKit;
    private HoldingManager heldItemManager;
    private PlayerInteractor playerInteractor;  


    GameInput gameInput;
    public IPlayer currentClass = null;
    public PlayerState currentState;
    public PlayerAttackState currentAttackState;

    private void Awake()
    {
        if (transformPlayer != null && transformPlayer != transform)
        {
          //Destroy(gameObject); // Already a player exists, destroy the duplicate
            return;
        }
        transformPlayer = transform;
        DontDestroyOnLoad(gameObject);

        _playroomKit = PlayroomManager.Instance.GetPlayroomKit();
        gameInput = GetComponentInChildren<GameInput>();
        // TODO: will be selected from UI
        currentClass = new PlayerAttacker(PlayerType.Attacker);

    }
    protected virtual void Start()
    {
        AssignEvents();
        GetPlayerComponent();
        transformPlayer = transform;
        SetStates();
        
    }
    private void SetStates()
    {
        fsm = new StateMachine<PlayerState>();

        fsm.AddState(PlayerState.IDLE, new IdleState().state);

        fsm.AddState(PlayerState.MOVE, new MoveState(PlayroomManager.Players[_playroomKit.MyPlayer()], gameInput, moveSpeed , animator).state);

        fsm.SetStartState(PlayerState.IDLE);    
        
        fsm.AddTransition(PlayerState.IDLE, PlayerState.MOVE, _ => gameInput.GetMovementNormalized().magnitude > 0.1f);
        fsm.AddTransition(PlayerState.MOVE, PlayerState.IDLE, _ => gameInput.GetMovementNormalized().magnitude < 0.1f);

        fsm.Init();
    }
    private void AssignEvents()
    {
        gameInput.OnInteractPlayer += InputSystem_OnInteractPlayer;
    }
    void Update()
    {
        currentState = fsm.ActiveState.name;
        fsm.OnLogic();

    }
    public bool HasItem() => heldItemManager.IsHoldingItem;
    public HeldItemType GetHeldItem() => heldItemManager.HeldItem;

    private void InputSystem_OnInteractPlayer(object sender, System.EventArgs e)
    {
        playerInteractor.Interact();
    }

    private void GetPlayerComponent()
    {
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
    }

}
