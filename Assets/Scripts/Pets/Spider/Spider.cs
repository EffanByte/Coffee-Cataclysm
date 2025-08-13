using UnityEngine;
using System.Collections;

public class Spider : MonoBehaviour
{
    [SerializeField] PetsAnimationController petsAnimationController;
    [SerializeField] Player player;

    //Spider Ranged attack
    public float detectionRadius = 5f;
    public float attackCooldown = 1.5f;
    private float cooldownTimer;
    private float shootDelay = 0.2f;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        petsAnimationController = GetComponent<PetsAnimationController>();
        petsAnimationController.ChangeAnimation(petsAnimationController.SpiderIdle);
    }

    private void Update()
    {
        if(player.currentState == PlayerState.MOVE)
        {
            petsAnimationController.ChangeAnimation(petsAnimationController.SpiderWalk);
        }
        else if(player.currentState == PlayerState.IDLE)
        {
            petsAnimationController.ChangeAnimation(petsAnimationController.SpiderIdle);
        }

        Attack();
    }

    private void Attack()
    {
        cooldownTimer -= Time.deltaTime;

        Collider[] hits = Physics.OverlapSphere(transform.position, detectionRadius);
        foreach (Collider hit in hits)
        {
            if (hit.CompareTag("Enemy") && cooldownTimer <= 0f)
            {
                petsAnimationController.ChangeAnimation(petsAnimationController.SpiderAttack);
                //ShootAtEnemy(hit.transform);
                StartCoroutine(ShootWithDelay(hit.transform));
                cooldownTimer = attackCooldown;
                break;
            }
        }
    }

    private IEnumerator ShootWithDelay(Transform enemy)
    {
        yield return new WaitForSeconds(shootDelay); // wait before firing

        if (enemy != null) // still there?
        {
            GameObject projectile = SpiderWebPool.Instance.GetProjectile();
            projectile.transform.position = transform.position;
            projectile.GetComponent<SpiderWeb>().SetTarget(enemy);
        }
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }

}
