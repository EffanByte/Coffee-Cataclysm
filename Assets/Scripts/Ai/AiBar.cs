using System;
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

    public string CustomerID;
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
        fsm.ActiveStateName.ToString();
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
        
        if (!string.IsNullOrEmpty(CustomerID))
        {
            OrderManager.Instance?.CancelOrder(CustomerID);
        }
    }

    private void OrderCoffee()
    {
        if (OrderManager.Instance != null)
        {
            OrderManager.Instance.CreateOrder(CustomerID);
        }
        else
        {
            Debug.LogError("OrderManager instance not found!");
        }
    }

    public void ReceiveOrder(HeldItemType order)
    {
        if (OrderManager.Instance == null)
        {
            Debug.LogError("OrderManager instance not found!");
            return;
        }
        
        bool orderServed = OrderManager.Instance.ReceiveOrder(CustomerID, order);
        
        if (orderServed)
        {
            Debug.Log($"Order served successfully for customer {CustomerID}");
            orderReceived = true;
            
            // Hide order bubble
            if (orderBubble != null)
            {
                orderBubble.SetActive(false);
            }
        }
        else
        {
            Debug.LogWarning($"Failed to serve order for customer {CustomerID}");
        }
    }

    private void AssignExitWaypoint()
    {
        Transform exitWaypoint = waypointManager.GetExitWaypoint();
        agent.SetDestination(exitWaypoint.position);
    }

    public static AiBar GetByCustomerId(string customerId)
    {
        AiBar[] allBars = FindObjectsByType<AiBar>(FindObjectsSortMode.None);
        foreach (var bar in allBars)
        {
            Debug.Log(bar.CustomerID);
            if (bar.CustomerID == customerId)
            {
                Debug.Log($"Found AiBar with CustomerID: {customerId}");
                return bar;
            }
        }
        return null;
    }
    
    /// <summary>
    /// Sets the order bubble reference (called when bubble is created via RPC)
    /// </summary>
    /// <param name="bubble">The order bubble GameObject</param>
    public void SetOrderBubble(GameObject bubble)
    {
        orderBubble = bubble;
    }
}
