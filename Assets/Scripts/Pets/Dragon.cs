using UnityEngine;

public class Dragon : MonoBehaviour
{
    [SerializeField] PetsAnimationController petsAnimationController;
    private void Awake()
    {
        petsAnimationController = GetComponent<PetsAnimationController>();
    }
    private void Start()
    {
        petsAnimationController.ChangeAnimation(petsAnimationController.DragonFly);
    }

}
