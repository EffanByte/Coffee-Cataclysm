using UnityEngine;
using UnityEngine.SceneManagement;

public class DoorTrigger : MonoBehaviour
{
    private int LoaderScene = 1;

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            other.transform.position = new Vector3(0f, 1f, 0f);
            GameStateManager.Instance.GoToOutSide(LoaderScene);
        }
    }
}
