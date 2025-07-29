using UnityEngine;

public class PortalTrigger : MonoBehaviour
{
    private int GameBarSceneIndex = 0;


    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            GameStateManager.Instance.GoToGameBar(GameBarSceneIndex);
        }
    }
}
