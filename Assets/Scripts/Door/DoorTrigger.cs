using UnityEngine;
using Playroom;
public class DoorTrigger : MonoBehaviour
{
    PlayroomKit _playroom;
    private int LoaderScene = 1;
    void Start()
    {
        _playroom = PlayroomManager.Instance.GetPlayroomKit();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _playroom = PlayroomManager.Instance.GetPlayroomKit();

            // Find the PlayerData for the player that entered the trigger
            PlayerData playerData = null;
            foreach (var kvp in PlayroomManager.Players) // can be improved by trygetvalue I guess
            {
                if (kvp.Value.playerObject == other.gameObject)
                {
                    playerData = kvp.Value;
                    break;
                }
            }

            if (playerData.player == _playroom.MyPlayer()) 
            {
                // Update only local player's state; remotes will update via RPC handler
                playerData.gameState = GameSceneState.OutSide;
                #if !UNITY_EDITOR
                // Sync state change to all players via RPC
                _playroom.RpcCall("SyncPlayerStateChange", GameSceneState.OutSide.ToString(), PlayroomKit.RpcMode.ALL);
                #endif
                GameStateManager.Instance.GoToOutSide(LoaderScene);
            }
            else
                playerData.playerObject.SetActive(false); // Better to delete it and reinstantiate when player joins back
        }
    }
}
