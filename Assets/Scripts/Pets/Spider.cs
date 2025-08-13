using UnityEngine;

public class Spider : MonoBehaviour
{
    [SerializeField] PetsAnimationController petsAnimationController;
    [SerializeField] Player player;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        petsAnimationController = GetComponent<PetsAnimationController>();
        petsAnimationController.ChangeAnimation(petsAnimationController.SpiderIdle);
    }

    private void Update()
    {
        if(player.currentState == PlayerState.MOVE)
        {
            petsAnimationController.ChangeAnimation(petsAnimationController.SpiderWalk);
        }
        else if(player.currentState == PlayerState.IDLE)
        {
            petsAnimationController.ChangeAnimation(petsAnimationController.SpiderIdle);
        }
    }
}
