using UnityHFSM;
using UnityEngine;
using UnityEngine.Windows;
public class MoveState
{
    public State<PlayerState> state;

    public MoveState(Transform playerTransform, GameInput input, float speed)
    {
        state = new State<PlayerState>(
            onLogic: s =>
            {
                Vector2 inputVector = input.GetMovementNormalized();
                Vector3 moveDir = new(inputVector.x, 0f, inputVector.y);
                float moveDistance = Time.deltaTime * speed;

                playerTransform.position += moveDir * moveDistance;
            },
            onExit: s => Debug.Log("Exited Move")
        );
    }

}
