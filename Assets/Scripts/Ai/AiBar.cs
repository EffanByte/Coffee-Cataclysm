using UnityEngine;
using UnityEngine.AI;
using UnityHFSM;

public enum AIState
{
    IDLE,
    MOVE,
}

[RequireComponent(typeof(NavMeshAgent))]
public class AiBar : MonoBehaviour
{
    public AIState currentState;

    private NavMeshAgent agent;
    private Transform targetWaypoint;
    private WaypointManager waypointManager;
    private StateMachine<AIState> fsm;

    private void Start()
    {
        SetupFSM();
    }

    public void Initialize(WaypointManager manager)
    {
        waypointManager = manager;
        agent = GetComponent<NavMeshAgent>();
        AssignWaypoint();
    }
    private void SetupFSM()
    {
        fsm = new StateMachine<AIState>();

        // Idle State
        fsm.AddState(AIState.IDLE, onEnter: State =>
        {
            Debug.Log("AI is now idle.");
        });

        // Move State
        fsm.AddState(AIState.MOVE, onEnter: state =>
        {
            Debug.Log("Entered MOVE state");
            Debug.Log("AI is moving.");
            if (targetWaypoint != null)
            {
                agent.SetDestination(targetWaypoint.position);
            }
        });
      
        // Transitions
        fsm.AddTransition(AIState.IDLE, AIState.MOVE, condition: _ => targetWaypoint != null && agent.remainingDistance > 0.1f);
        fsm.AddTransition(AIState.MOVE, AIState.IDLE, condition: _ => !agent.pathPending && agent.remainingDistance <= agent.stoppingDistance);

        // Start in Idle
        fsm.SetStartState(AIState.IDLE);
        fsm.Init();
    }


    private void Update()
    {
        fsm.OnLogic();
        string currentStateName = fsm.ActiveStateName.ToString();
        Debug.Log($"Current State: {currentStateName}");
    }

    private void AssignWaypoint()
    {
        targetWaypoint = waypointManager.GetAvailableWaypoint();
        if (targetWaypoint != null)
        {
            agent.SetDestination(targetWaypoint.position);
        }
        else
        {
            Debug.LogWarning("No available waypoint!");
            // Optional: deactivate or wait
        }
    }

    private void OnDisable()
    {
        if (targetWaypoint != null && waypointManager != null)
        {
            waypointManager.ReleaseWaypoint(targetWaypoint);
        }
    }
}
