using UnityEngine;

public class PortalTrigger : MonoBehaviour
{
    private int LoaderScene = 1;


    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            GameStateManager.Instance.GoToGameBar(LoaderScene);
        }
    }
}
