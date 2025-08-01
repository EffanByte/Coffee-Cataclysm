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
            animator.SetBool("IsRunning", true);
        });

        // MOVE: Chase player
        fsm.AddState(AIState.MOVE, onLogic: s =>
        {
            if (player != null)
                agent.SetDestination(player.position);
            animator.SetBool("IsRunning", true);
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
        agent.SetDestination(player.position);
    }
}
