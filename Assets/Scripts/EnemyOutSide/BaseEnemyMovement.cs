using UnityEngine;
using UnityEngine.AI;
using UnityHFSM;

public abstract class BaseEnemyMovement : MonoBehaviour
{
    [SerializeField] private float updateSpeed = 0.1f;
    [SerializeField] private Animator animator;
    private NavMeshAgent agent;

    private StateMachine<AIState> fsm;
    [SerializeField] private float chaseStartDistance = 10f;
    [SerializeField] private float chaseStopDistance = 12f;

    private float hitInterval = 1.0f; // Time between hits
    private float hitTimer = 0f;

    protected virtual void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        SetupFSM();
    }
    
    protected virtual void Update()
    {
        fsm.OnLogic();
    }

    // Find the closest player with the "Player" tag
    private Transform FindClosestPlayer()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        Transform closestPlayer = null;
        float closestDistance = Mathf.Infinity;

        foreach (GameObject player in players)
        {
            float distance = Vector3.Distance(transform.position, player.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestPlayer = player.transform;
            }
        }

        return closestPlayer;
    }

    protected virtual void SetupFSM()
    {
        fsm = new StateMachine<AIState>();

        // IDLE: Don't move
        fsm.AddState(AIState.IDLE, onEnter: s =>
        {
            if(agent.isActiveAndEnabled)
            agent.ResetPath();
            Debug.Log("Enemy is idle.");
        },
        onLogic: s =>
        {
            Transform closestPlayer = FindClosestPlayer();
            if (closestPlayer == null) return;
            
            float distance = Vector3.Distance(transform.position, closestPlayer.position);
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
            Transform closestPlayer = FindClosestPlayer();
            if (closestPlayer == null) return;
            
            float distance = Vector3.Distance(transform.position, closestPlayer.position);
            if (distance < agent.stoppingDistance)
            {
                animator.SetTrigger("Attack");
            }
            else
            {
                if (animator.GetCurrentAnimatorStateInfo(0).IsName("Attack")) return;
                if(agent.isActiveAndEnabled)
                agent.SetDestination(closestPlayer.position);
            }
        },
        onExit: s =>
        {
            animator.SetBool("IsRunning", false);
        }
        );

        // Transition: IDLE ? MOVE
        fsm.AddTransition(AIState.IDLE, AIState.MOVE, condition: s =>
        {
            Transform closestPlayer = FindClosestPlayer();
            if (closestPlayer == null) return false;
            return Vector3.Distance(transform.position, closestPlayer.position) <= chaseStartDistance;
        });

        // Transition: MOVE ? IDLE
        fsm.AddTransition(AIState.MOVE, AIState.IDLE, condition: s =>
        {
            Transform closestPlayer = FindClosestPlayer();
            if (closestPlayer == null) return true;
            return Vector3.Distance(transform.position, closestPlayer.position) >= chaseStopDistance;
        });

        fsm.SetStartState(AIState.IDLE);
        fsm.Init();
    }

    protected virtual void FixedUpdate()
    {
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Attack")) return;
        
        Transform closestPlayer = FindClosestPlayer();
        if (closestPlayer != null && agent.isActiveAndEnabled)
        {
            agent.SetDestination(closestPlayer.position);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("PlayerHit"))
        {
            EnemyPoolHandler.Instance.Despawn(this.gameObject);
            UIManager.Instance.ScoreUp();
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            hitTimer += Time.deltaTime;

            if (hitTimer >= hitInterval)
            {
                var anim = other.GetComponent<PlayerAnimatonController>();

                if (anim.GetCurrentAnimation() == "Hit_A")
                    anim.ChangeAnimation("Hit_B");
                else
                    anim.ChangeAnimation("Hit_A");

                other.GetComponent<ClassManager>().Damage();
                hitTimer = 0f; // reset timer
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            hitTimer = 0f; // reset when player leaves
        }
    }

}
