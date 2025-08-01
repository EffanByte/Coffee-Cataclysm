using System;
using Unity.VisualScripting;
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
    [SerializeField] private Animator animator;

    HeldItemType currentOrder = HeldItemType.None;

    [SerializeField] private GameObject orderBubblePrefab;
    public AIState currentState;
    private bool orderReceived = false;
    private NavMeshAgent agent;
    private Transform targetWaypoint;
    private WaypointManager waypointManager;
    private StateMachine<AIState> fsm;
    private GameObject orderBubble;

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

        });

        // Move State
        fsm.AddState(AIState.MOVE, onEnter: state =>
        {

            if (targetWaypoint != null)
            {
                animator.SetBool("IsRunning", true);
            }
        },
        onExit: state =>
        {
            animator.SetBool("IsRunning", false);
        });

        // Transitions
        fsm.AddTransition(AIState.IDLE, AIState.MOVE, condition: _ => targetWaypoint != null && agent.remainingDistance > 0.1f, onTransition: _ =>
        {
            if (targetWaypoint != null)
            {
                agent.SetDestination(targetWaypoint.position);
            }
        });
        fsm.AddTransition(AIState.MOVE, AIState.IDLE, condition: _ => !agent.pathPending && agent.remainingDistance <= agent.stoppingDistance, onTransition: _ =>
        {
            if (orderReceived)
            {
                transform.gameObject.SetActive(false);
            }
            OrderCoffee();
        });
        fsm.AddTransition(AIState.IDLE, AIState.MOVE, condition: _ => orderReceived, onTransition: _ =>
        {
            AssignExitWaypoint(); // Assign a new waypoint after receiving an order
        });
        // Start in Idle
        fsm.SetStartState(AIState.IDLE);
        fsm.Init();
    }


    private void Update()
    {
        fsm.OnLogic();
        string currentStateName = fsm.ActiveStateName.ToString();
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

    private void OrderCoffee()
    {
        currentOrder = (HeldItemType)UnityEngine.Random.Range(5, Enum.GetValues(typeof(HeldItemType)).Length); // Randomly select a coffee blend
        Debug.Log(currentOrder);
        orderBubble = Instantiate(orderBubblePrefab, transform.position + new Vector3(1.5f, 3, 0), Quaternion.identity, transform);
        orderBubble.GetComponent<OrderBubble>().SetIcon(currentOrder);
    }

    public void ReceiveOrder(HeldItemType order)
    {
        if (order == HeldItemType.None)
        {
            Debug.LogWarning("Received an empty order.");
            return;
        }
        if (currentOrder == order)
        {
            Debug.Log("Received correct order: " + order);
            currentOrder = HeldItemType.None; // Reset order after serving
        }
        Debug.Log($"Received order: {currentOrder}");
        orderReceived = true;
        orderBubble.SetActive(false);
    }

    private void AssignExitWaypoint()
    {
        Transform exitWaypoint = waypointManager.GetExitWaypoint();
        agent.SetDestination(exitWaypoint.position);
    }
}
