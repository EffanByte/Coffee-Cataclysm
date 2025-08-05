using UnityHFSM;
using UnityEngine;
using Playroom;

public class MoveState
{
    public State<PlayerState> state;

    PlayroomKit _playroomKit = PlayroomManager.Instance.GetPlayroomKit();
    public MoveState(PlayerData playerData, GameInput input, float speed)
    {
        state = new State<PlayerState>(
            onLogic: s =>
            {
                Transform playerTransform = playerData.playerObject.transform;

                if (playerData.player.id == _playroomKit.MyPlayer().id)
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
                }
            
            },
            onExit: s => Debug.Log("Exited Move")
        );
    }
}
