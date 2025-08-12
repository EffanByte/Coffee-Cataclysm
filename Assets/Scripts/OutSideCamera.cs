using Cinemachine;
using UnityEngine;
using Playroom;
public class OutSideCamera : MonoBehaviour
{
    private CinemachineVirtualCamera virtualCamera;
    PlayroomKit _playroom;
    private void Start()
    {
        _playroom = PlayroomManager.Instance.GetPlayroomKit();
        virtualCamera = GetComponent<CinemachineVirtualCamera>();
        foreach (var player in PlayroomManager.Players)
        {
            if (player.Key.id == _playroom.MyPlayer().id)
                virtualCamera.Follow = player.Value.playerObject.transform;
        }
    }
}
