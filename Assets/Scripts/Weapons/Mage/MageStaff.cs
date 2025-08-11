using UnityEngine;
using System.Linq;

public class MageStaff : MonoBehaviour
{
    public static MageStaff Instance { get; private set; }


    public Transform shootPoint; // Where the projectile spawns
    public Animator animator;
    public float targetSearchRadius = 20f;
    public LayerMask enemyLayer; // Assign "Enemy" layer

    private void Awake()
    {
        Instance = this;
    }

    // Called by animation event
    public void ShootMagic()
    {
        GameObject proj = ShootPrefabPool.Instance.GetShoot();
        proj.transform.position = shootPoint.position;

        Transform target = GetClosestEnemy();
        Vector3 targetPosition = target != null ? target.position : shootPoint.position + shootPoint.forward * 10f;

        proj.GetComponent<ShootPrefab>().Launch(targetPosition);
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
