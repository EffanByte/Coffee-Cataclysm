using UnityEngine;
using UnityEngine.SceneManagement;

public class DoorTrigger : MonoBehaviour
{
    private int OutSideSceneIndex = 2;

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            GameStateManager.Instance.GoToOutSide(OutSideSceneIndex);
        }
    }
}
