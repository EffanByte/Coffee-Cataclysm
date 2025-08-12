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
            // Check if this is the local player
            if (PlayroomManager.Players[_playroom.MyPlayer()].playerObject == other.gameObject)
                GameStateManager.Instance.GoToGameBar(LoaderScene);
        }
    }
}
