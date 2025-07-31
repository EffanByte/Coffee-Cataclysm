using System;
using UnityEngine;
using UnityHFSM;

public enum BeanType {none, brown, white}
public enum PlayerState
{
    IDLE,
    MOVE,
    HOLDING,
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
        var move = new MoveState();

        fsm.AddState(PlayerState.IDLE, idle.state);
        fsm.AddState(PlayerState.MOVE, move.state);

        fsm.SetStartState(PlayerState.IDLE);
        fsm.Init();

        fsm.AddTransition(PlayerState.IDLE, PlayerState.MOVE, condition: _ => gameInput.GetMovementNormalized().magnitude > 0);
        fsm.AddTransition(PlayerState.MOVE, PlayerState.IDLE, condition: _ => gameInput.GetMovementNormalized().magnitude == 0);
    }


    void Update()
    {
        PlayerMovement();
    }

    private void PlayerMovement()
    {
        Vector2 InputVector = gameInput.GetMovementNormalized();
        Vector3 moveDir = new(InputVector.x, 0f , InputVector.y);

        float moveDistance = Time.deltaTime * moveSpeed;
        transform.position += moveDir * moveDistance;
    }
}
