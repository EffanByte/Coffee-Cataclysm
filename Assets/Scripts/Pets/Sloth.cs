using UnityEngine;

public class Sloth : MonoBehaviour
{
    [SerializeField] PetsAnimationController petsAnimationController;
    [SerializeField] Player player;

    private void Awake()
    {
        petsAnimationController = GetComponent<PetsAnimationController>();
    }

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        petsAnimationController.ChangeAnimation(petsAnimationController.SlothIdle);
    }

    private void Update()
    {
        if (player.currentState == PlayerState.MOVE)
        {
            petsAnimationController.ChangeAnimation(petsAnimationController.SlothWalk);
        }
        else if (player.currentState == PlayerState.IDLE)
        {
            petsAnimationController.ChangeAnimation(petsAnimationController.SlothIdle);
        }
    }
}
