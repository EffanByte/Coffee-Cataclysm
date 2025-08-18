using UnityEngine;
using Playroom;
public class PortalTrigger : MonoBehaviour
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
            // Only do it for local player
            if (playerData.player == _playroom.MyPlayer())
            {
                // Sync state change to all players via RPC
                _playroom.RpcCall("SyncPlayerStateChange", GameSceneState.GameBar.ToString(), PlayroomKit.RpcMode.ALL);
                GameStateManager.Instance.GoToGameBar(LoaderScene);
            }
            else
                playerData.playerObject.SetActive(false);
        }
    }
}
