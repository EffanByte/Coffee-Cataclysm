using System;
using UnityEngine;
using UnityHFSM;


public enum PlayerState
{
    IDLE,
    MOVE,
}

public class Player : MonoBehaviour
{
    public PlayerState currentState;

    public static Transform transformPlayer;

    [SerializeField] private float moveSpeed;
    [SerializeField] private GameInput gameInput;

    private StateMachine<PlayerState> fsm;

    void Start()
    {
        transformPlayer = this.transform;

        fsm = new StateMachine<PlayerState>();

        var idle = new IdleState();
        var move = new MoveState(transform, gameInput, moveSpeed);

        fsm.AddState(PlayerState.IDLE, idle.state);
        fsm.AddState(PlayerState.MOVE, move.state);

        fsm.SetStartState(PlayerState.IDLE);

        fsm.AddTransition(PlayerState.IDLE, PlayerState.MOVE, condition: _ => gameInput.GetMovementNormalized().magnitude > 0.1f);
        fsm.AddTransition(PlayerState.MOVE, PlayerState.IDLE, condition: _ => gameInput.GetMovementNormalized().magnitude < 0.1f);

        fsm.Init();
    }


    void Update()
    {
        fsm.OnLogic();
        string currentStateName = fsm.ActiveStateName.ToString();
        Debug.Log($"Current State: {currentStateName}");
    }

    private void PlayerMovement()
    {
        Vector2 InputVector = gameInput.GetMovementNormalized();
        Vector3 moveDir = new(InputVector.x, 0f , InputVector.y);

        float moveDistance = Time.deltaTime * moveSpeed;
        transform.position += moveDir * moveDistance;
    }
}
