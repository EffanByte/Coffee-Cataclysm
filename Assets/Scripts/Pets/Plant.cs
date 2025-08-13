using UnityEngine;

public class Plant : MonoBehaviour
{
    [SerializeField] PetsAnimationController petsAnimationController;
    [SerializeField] Player player;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        petsAnimationController = GetComponent<PetsAnimationController>();
        petsAnimationController.ChangeAnimation(petsAnimationController.PlantIdle);
    }

    private void Update()
    {
        if (player.currentState == PlayerState.MOVE)
        {
            petsAnimationController.ChangeAnimation(petsAnimationController.PlantWalk);
        }
        else if (player.currentState == PlayerState.IDLE)
        {
            petsAnimationController.ChangeAnimation(petsAnimationController.PlantIdle);
        }
    }
}
