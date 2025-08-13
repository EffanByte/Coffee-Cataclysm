using UnityEngine;
using System.Collections;
using System.Linq;
public class PlayerAnimatonController : MonoBehaviour
{
   [SerializeField] Animator animator;
    Player player;
    private string currentAnimation = "";
    readonly string[] blockAnimations = { "Shoot", "Melee", "Mage_Shoot","Hit_A","Hit_B" };

    private void Awake()
    {
        player = GetComponent<Player>();
        if(animator == null)
        animator = GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        CheckAnimation();
    }

    private void CheckAnimation()
    {
        if (blockAnimations.Contains(currentAnimation))
            return;

        if (player.currentState == PlayerState.MOVE && GameStateManager.Instance.currentState == GameSceneState.GameBar)
            ChangeAnimation("Walk");
        else if (player.currentState == PlayerState.MOVE && GameStateManager.Instance.currentState == GameSceneState.OutSide)
            ChangeAnimation("Run");
        else if (player.currentState == PlayerState.IDLE && GameStateManager.Instance.currentState == GameSceneState.GameBar)
            ChangeAnimation("GameBar_Idle");
        else if (player.currentState == PlayerState.IDLE && GameStateManager.Instance.currentState == GameSceneState.OutSide)
            ChangeAnimation("Idle");
    }

    public void ChangeAnimation(string animation, float crossFade = 0.2f, float time = 0f)
    {
        if (time > 0f) StartCoroutine(Wait(time, animation, crossFade));
        else Validate(animation, crossFade);
    }
    IEnumerator Wait(float time, string animation, float crossFade)
    {
        yield return new WaitForSeconds(time - crossFade);
        Validate(animation, crossFade);
    }

    void Validate(string animation, float crossFade)
    {
        if (currentAnimation != animation)
        {
            currentAnimation = animation;
            if (currentAnimation == "")
                CheckAnimation();
            else
                animator.CrossFade(animation, crossFade);
        }
    }

    public string GetCurrentAnimation()
    {
        return currentAnimation;
    }
}
