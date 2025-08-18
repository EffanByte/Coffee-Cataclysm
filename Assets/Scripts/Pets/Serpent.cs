using UnityEngine;

public class Serpent : MonoBehaviour
{
    [SerializeField] PetsAnimationController petsAnimationController;

    private void Awake()
    {
        petsAnimationController = GetComponent<PetsAnimationController>();
    }

    private void Start()
    {
        petsAnimationController.ChangeAnimation(petsAnimationController.SerpentFly);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Enemy"))
        {
            petsAnimationController.ChangeAnimation(petsAnimationController.SerpentAttack);
            Debug.Log("SerpentAttacking");
        }
    }
}
