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
}
