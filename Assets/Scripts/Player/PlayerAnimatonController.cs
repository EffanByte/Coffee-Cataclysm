using UnityEngine;
using System.Collections;
using System.Linq;
using Playroom;
public class PlayerAnimatonController : MonoBehaviour
{
   [SerializeField] Animator animator;
    Player player;
    PlayroomKit _playroom;
    private string currentAnimation = "";
    readonly string[] blockAnimations = { "Shoot", "Melee", "Mage_Shoot","Hit_A","Hit_B" };
    private Vector3 lastPosition;
    private float smoothedSpeed = 0f;
    [SerializeField] private float speedSmoothingRate = 12f; // higher = snappier
    [SerializeField] private float enterMoveSpeed = 0.1f;    // m/s to start walking/running
    [SerializeField] private float exitMoveSpeed = 0.05f;     // m/s to return to idle (hysteresis)
    [SerializeField] private float minAnimStateDuration = 0.2f; // seconds before switching states
    private float lastAnimChangeTime = 0f;
    private bool isMovingState = false;

    private void Awake()
    {
        player = GetComponent<Player>();
        if(animator == null)
        animator = GetComponentInChildren<Animator>();
        _playroom = PlayroomManager.Instance.GetPlayroomKit();
        lastPosition = transform.position;
        lastAnimChangeTime = Time.time;
    }

    private void Update()
    {
        // Exponential smoothing of speed (based on position delta)
        float dt = Mathf.Max(Time.deltaTime, 0.0001f);
        float instantaneousSpeed = (transform.position - lastPosition).magnitude / dt;
        float lerpFactor = 1f - Mathf.Exp(-speedSmoothingRate * dt);
        smoothedSpeed = Mathf.Lerp(smoothedSpeed, instantaneousSpeed, lerpFactor);

        CheckAnimation();
        lastPosition = transform.position;
    }

    private void CheckAnimation()
    {
        if (blockAnimations.Contains(currentAnimation))
            return;

        // Hysteresis: different thresholds for entering vs exiting move state
        bool wantMoving = isMovingState ? (smoothedSpeed > exitMoveSpeed) : (smoothedSpeed > enterMoveSpeed);

        // Enforce a minimum duration in the current state to avoid flicker
        if (wantMoving != isMovingState && (Time.time - lastAnimChangeTime) < minAnimStateDuration)
            return;

        if (wantMoving != isMovingState)
        {
            isMovingState = wantMoving;
            lastAnimChangeTime = Time.time;
        }

        if (isMovingState)
        {
            if (GameStateManager.Instance.currentState == GameSceneState.GameBar)
                ChangeAnimation("Walk");
            else
                ChangeAnimation("Run");
        }
        else
        {
            if (GameStateManager.Instance.currentState == GameSceneState.GameBar)
                ChangeAnimation("GameBar_Idle");
            else
                ChangeAnimation("Idle");
        }
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
            {
                animator.CrossFade(animation, crossFade);
                // If this is my local player, broadcast to others
                var playroom = PlayroomManager.Instance.GetPlayroomKit();
                var my = playroom.MyPlayer();
                if (PlayroomManager.Players.TryGetValue(my, out var myData) && myData.playerObject == this.gameObject)
                {
                    playroom.RpcCall("HandleAnimChange", animation, PlayroomKit.RpcMode.ALL);
                }
            }
        }
    }

    public string GetCurrentAnimation()
    {
        return currentAnimation;
    }
}
