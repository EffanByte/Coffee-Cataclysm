using UnityEngine;
using UnityEngine.AI;
using UnityHFSM;

public abstract class BaseEnemyMovement : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private float updateSpeed = 0.1f;
    [SerializeField] private Animator animator;
    private NavMeshAgent agent;


    private StateMachine<AIState> fsm;
    [SerializeField] private float chaseStartDistance = 10f;
    [SerializeField] private float chaseStopDistance = 12f;

    protected virtual void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();

        SetupFSM();
    }
    protected virtual void Update()
    {
        fsm.OnLogic();
    }

    protected virtual void SetupFSM()
    {
        fsm = new StateMachine<AIState>();

        // IDLE: Don't move
        fsm.AddState(AIState.IDLE, onEnter: s =>
        {
            agent.ResetPath();
            Debug.Log("Enemy is idle.");
        },
        onLogic: s =>
        {
            if (Player.transformPlayer == null) return;
            float distance = Vector3.Distance(transform.position, Player.transformPlayer.position);
            if (distance < 3f)
            {
                animator.SetTrigger("Attack");
            }
        },
        onExit: s =>
        {
            animator.SetBool("IsRunning", true);
        }
        );

        // MOVE: Chase player
        fsm.AddState(AIState.MOVE, onLogic: s =>
        {
            if (Player.transformPlayer == null) return;
            float distance = Vector3.Distance(transform.position, Player.transformPlayer.position);
            if (distance < agent.stoppingDistance)
            {
                animator.SetTrigger("Attack");
            }
            else
            {
                if (player != null)
                    if (animator.GetCurrentAnimatorStateInfo(0).IsName("Attack")) return;
                agent.SetDestination(player.position);
            }
        },
        onExit: s =>
        {
            animator.SetBool("IsRunning", false);
        }
        );

        // Transition: IDLE ? MOVE
        fsm.AddTransition(AIState.IDLE, AIState.MOVE, condition: s =>
            Vector3.Distance(transform.position, player.position) <= chaseStartDistance);

        // Transition: MOVE ? IDLE
        fsm.AddTransition(AIState.MOVE, AIState.IDLE, condition: s =>
            Vector3.Distance(transform.position, player.position) >= chaseStopDistance);

        fsm.SetStartState(AIState.IDLE);
        fsm.Init();
    }

    protected virtual void FixedUpdate()
    {
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Attack")) return;
        agent.SetDestination(player.position);
    }
}
