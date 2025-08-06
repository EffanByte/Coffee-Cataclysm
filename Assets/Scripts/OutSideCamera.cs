using Cinemachine;
using UnityEngine;

public class OutSideCamera : MonoBehaviour
{
    private CinemachineVirtualCamera virtualCamera;

    private void Start()
    {
        virtualCamera = GetComponent<CinemachineVirtualCamera>();
        if(Player.transformPlayer != null)
        {
            virtualCamera.Follow = Player.transformPlayer;
        }
        else
        {
            Debug.LogWarning("transformPlayer is null — cannot set camera follow.");
        }
    }

}
