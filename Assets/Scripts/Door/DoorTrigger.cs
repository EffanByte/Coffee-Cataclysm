using UnityEngine;
using Playroom;
public class DoorTrigger : MonoBehaviour
{
    PlayroomKit _playroom;
    private int LoaderScene = 1;
    void Start()
    {
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _playroom = PlayroomManager.Instance.GetPlayroomKit();

            // Find the PlayerData for the player that entered the trigger
            PlayerData playerData = null;
            foreach (var kvp in PlayroomManager.Players)
            {
                if (kvp.Value.playerObject == other.gameObject)
                {
                    playerData = kvp.Value;
                    break;
                }
            }

            if (playerData.player == _playroom.MyPlayer())
            {
                playerData.gameState = GameSceneState.GameBar;
                GameStateManager.Instance.GoToOutSide(LoaderScene);
            }
        }
    }
}
