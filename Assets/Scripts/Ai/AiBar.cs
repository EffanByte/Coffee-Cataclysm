using UnityEngine;
using UnityEngine.AI;


[RequireComponent(typeof(NavMeshAgent))]
public class AiBar : MonoBehaviour
{
    private NavMeshAgent agent;
    private Transform targetWaypoint;
    private WaypointManager waypointManager;

    public void Initialize(WaypointManager manager)
    {
        waypointManager = manager;
        agent = GetComponent<NavMeshAgent>();
        AssignWaypoint();
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
