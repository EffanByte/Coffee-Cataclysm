using UnityEngine;

public class Plant : MonoBehaviour
{
    [SerializeField] PetsAnimationController petsAnimationController;
    [SerializeField] Player player;


    private float attackCooldown = 0f;
    private float attackDuration = 0.8f;

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

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            petsAnimationController.ChangeAnimation(petsAnimationController.PlantAttack);
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            if (attackCooldown <= 0f)
            {
                petsAnimationController.ChangeAnimation(petsAnimationController.PlantAttack);
                attackCooldown = attackDuration;
            }
        }
    }
}
