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
            PlayerData playerData = PlayroomManager.Players[_playroom.MyPlayer()];

            if (PlayroomManager.Players[_playroom.MyPlayer()].playerObject == other.gameObject)
            {
                playerData.gameState = GameSceneState.GameBar;
                GameStateManager.Instance.GoToGameBar(LoaderScene);
            }
        }
    }
}
