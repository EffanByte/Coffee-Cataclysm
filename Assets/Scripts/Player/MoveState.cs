using UnityHFSM;
using UnityEngine;

public class MoveState
{
    public State<PlayerState> state;

    public MoveState(Transform playerTransform, GameInput input, float speed,Animator animator)
    {
        state = new State<PlayerState>(
            onEnter: s =>
            {
                animator.SetBool("IsWalking", true);
            },
            onLogic: s =>
            {
                Vector2 inputVector = input.GetMovementNormalized();
                Vector3 moveDir = new(inputVector.x, 0f, inputVector.y);
                if (moveDir.sqrMagnitude > 0.01f)
                {
                    // Rotate player to face move direction
                    Quaternion targetRotation = Quaternion.LookRotation(moveDir);
                    playerTransform.rotation = Quaternion.Slerp(playerTransform.rotation, targetRotation, Time.deltaTime * 10f);

                    // Move
                    float moveDistance = Time.deltaTime * speed;
                    playerTransform.position += moveDir * moveDistance;

                }
            },
            onExit: s => { 
                animator.SetBool("IsWalking", false);
            }
        );
    }
}
