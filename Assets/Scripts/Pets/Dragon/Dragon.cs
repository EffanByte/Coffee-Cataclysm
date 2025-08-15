using UnityEngine;

public class Dragon : MonoBehaviour
{
    [SerializeField] PetsAnimationController petsAnimationController;
    [SerializeField] ParticleSystem fireParticle;
    private float attackCooldown = 0f;
    private float attackDuration = 0.8f;

    private void Awake()
    {
        petsAnimationController = GetComponent<PetsAnimationController>();
    }
    private void Start()
    {
        petsAnimationController.ChangeAnimation(petsAnimationController.DragonFly);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Enemy"))
        {
            FaceEnemy(other.gameObject);
            fireParticle.Play();
            petsAnimationController.ChangeAnimation(petsAnimationController.DragonAttack);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            FaceEnemy(other.gameObject);
            fireParticle.Play();
            if (attackCooldown <= 0f)
            {
                petsAnimationController.ChangeAnimation(petsAnimationController.PlantAttack);
                attackCooldown = attackDuration;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Enemy"))
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
