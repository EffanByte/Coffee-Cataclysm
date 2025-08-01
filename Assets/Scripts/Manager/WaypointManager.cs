using UnityEngine;

public class WaypointManager : MonoBehaviour
{
    [SerializeField] private Transform[] waypoints;
    [SerializeField] private Transform exitWaypoint;
    private bool[] isOccupied;

    void Awake()
    {
        isOccupied = new bool[waypoints.Length];
    }

    public Transform GetAvailableWaypoint()
    {
        for (int i = 0; i < waypoints.Length; i++)
        {
            if (!isOccupied[i])
            {
                isOccupied[i] = true;
                return waypoints[i];
            }
        }
        return null; // No available waypoint
    }

    public void ReleaseWaypoint(Transform waypoint)
    {
        for (int i = 0; i < waypoints.Length; i++)
        {
            if (waypoints[i] == waypoint)
            {
                isOccupied[i] = false;
                break;
            }
        }
    }
    public Transform GetExitWaypoint()
    {
    return exitWaypoint;
    }
}
