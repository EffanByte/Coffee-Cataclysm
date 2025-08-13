using UnityEngine;
using System.Collections;
using System.Linq;


public class PetsAnimationController : MonoBehaviour
{
    ////Spider
    public readonly int SpiderWalk = Animator.StringToHash("SpiderWalk");
    public readonly int SpiderIdle = Animator.StringToHash("SpiderIdle");
    public readonly int SpiderAttack = Animator.StringToHash("SpiderAttack");

    //// Serpent
    public readonly int SerpentFly = Animator.StringToHash("SerpentFly");
    public readonly int SerpentAttack = Animator.StringToHash("SerpentAttack");

    //// Dragon
    public readonly int DragonFly = Animator.StringToHash("DragonFly");
    public readonly int DragonAttack = Animator.StringToHash("DragonAttack");

    //// Plant
    public readonly int PlantWalk = Animator.StringToHash("PlantWalk");
    public readonly int PlantAttack = Animator.StringToHash("PlantAttack");
    public readonly int PlantIdle = Animator.StringToHash("PlantIdle");

    ////Sloth
    public readonly int SlothAttack = Animator.StringToHash("SlothAttack");
    public readonly int SlothIdle = Animator.StringToHash("SlothIdle");
    public readonly int SlothWalk = Animator.StringToHash("SlothWalk");



    Animator animator;
    private int currentAnimation = 0;
    //readonly string[] blockAnimations = { "PlantAttack", "SerpentAttack", "SpiderAttack", "SlothAttack", "DragonAttack"};

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }
    private void CheckAnimation()
    {
        //if (blockAnimations.Contains(currentAnimation))
          //  return;
    }
    public void ChangeAnimation(int animation, float crossFade = 0.2f, float time = 0f)
    {
        if (time > 0f) StartCoroutine(Wait(time, animation, crossFade));
        else Validate(animation, crossFade);
    }
    IEnumerator Wait(float time, int animation, float crossFade)
    {
        yield return new WaitForSeconds(time - crossFade);
        Validate(animation, crossFade);
    }

    void Validate(int animation, float crossFade)
    {
        if (currentAnimation != animation)
        {
            currentAnimation = animation;
            if (currentAnimation == 0)
                CheckAnimation();
            else
                animator.CrossFade(animation, crossFade);
        }
    }
    public int GetCurrentAnimation()
    {
        return currentAnimation;
    }
}
