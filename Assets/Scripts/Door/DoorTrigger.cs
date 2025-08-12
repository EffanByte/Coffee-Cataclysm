using UnityEngine;
using Playroom;

public class DoorTrigger : MonoBehaviour
{
    PlayroomKit _playroom;
    private int LoaderScene = 1;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _playroom = PlayroomManager.Instance.GetPlayroomKit();
            PlayerData playerData = PlayroomManager.Players[_playroom.MyPlayer()];
            if (playerData.playerObject == other.gameObject)
            {
                playerData.gameState = GameSceneState.OutSide;
                GameStateManager.Instance.GoToOutSide(LoaderScene);
            }
        }
    }
}