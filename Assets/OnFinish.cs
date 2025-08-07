using UnityEngine;

public class OnFinish : StateMachineBehaviour
{
    [SerializeField] private string animation;
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.GetComponentInParent<PlayerAnimatonController>().ChangeAnimation(animation,0.2f,stateInfo.length);
    }
}
