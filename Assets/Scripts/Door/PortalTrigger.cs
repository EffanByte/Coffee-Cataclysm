using UnityEngine;

public class PortalTrigger : MonoBehaviour
{
    private int LoaderScene = 1;


    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            other.transform.position = new Vector3(13f, 1f, -6f);
            GameStateManager.Instance.GoToGameBar(LoaderScene);
        }
    }
}
