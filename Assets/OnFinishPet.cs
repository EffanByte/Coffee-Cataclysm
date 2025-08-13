using UnityEngine;

public class OnFinishPet : StateMachineBehaviour
{
    [SerializeField] private int animation;
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.GetComponentInParent<PetsAnimationController>().ChangeAnimation(animation, 0.2f, stateInfo.length);
    }
}
