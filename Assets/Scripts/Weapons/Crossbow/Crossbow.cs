using UnityEngine;
using System.Linq;

public class Crossbow : MonoBehaviour
{
    public static Crossbow Instance { get; private set; }

    public Transform shootPoint; // Where arrow spawns
    public Animator animator;
    public float targetSearchRadius = 20f;
    public LayerMask enemyLayer; // Assign "Enemy" layer in Inspector

    private void Awake()
    {
        Instance = this;
    }

    // Called by animation event
    public void ShootArrow()
    {
        GameObject arrow = ArrowPool.Instance.GetArrow();
        arrow.transform.position = shootPoint.position;

        Transform target = GetClosestEnemy();
        Vector3 targetPosition = target != null ? target.position : shootPoint.position + shootPoint.forward * 10f;

        // Lock rotation toward target
        arrow.transform.LookAt(targetPosition);

        // Pass locked direction to arrow
        arrow.GetComponent<Arrow>().Launch(targetPosition);
    }


    private Transform GetClosestEnemy()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, targetSearchRadius, enemyLayer);

        if (hits.Length == 0)
            return null;

        return hits
            .OrderBy(h => Vector3.Distance(transform.position, h.transform.position))
            .First()
            .transform;
    }
}
