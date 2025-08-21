using UnityEngine;

public class Sloth : MonoBehaviour
{
    [SerializeField] PetsAnimationController petsAnimationController;
    [SerializeField] Player player;


    private float attackCooldown = 0f;
    private float attackDuration = 0.8f;

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

        if (attackCooldown > 0f)
        {
            attackCooldown -= Time.deltaTime;
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            FaceEnemy(other.gameObject);
            petsAnimationController.ChangeAnimation(petsAnimationController.SlothAttack);
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            FaceEnemy(other.gameObject);
            if (attackCooldown <= 0f)
            {
                petsAnimationController.ChangeAnimation(petsAnimationController.SlothAttack);
                attackCooldown = attackDuration;
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("Enemy"))
        {
            transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        }
    }

    void FaceEnemy(GameObject other)
    {
        Vector3 currentRotation = transform.eulerAngles;
        currentRotation.y = other.transform.eulerAngles.y;
        transform.rotation = Quaternion.Euler(currentRotation);
    }
}
