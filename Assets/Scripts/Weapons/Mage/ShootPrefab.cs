using UnityEngine;

public class ShootPrefab : MonoBehaviour
{
    public float speed = 20f;
    public float lifetime = 3f;

    private float lifeTimer;
    private Vector3 moveDirection;

    public void Launch(Vector3 targetPosition)
    {
        moveDirection = (targetPosition - transform.position).normalized;
    }

    void OnEnable()
    {
        lifeTimer = lifetime;
    }

    void Update()
    {
        transform.position += moveDirection * speed * Time.deltaTime;

        lifeTimer -= Time.deltaTime;
        if (lifeTimer <= 0f)
        {
            ShootPrefabPool.Instance.ReturnShoot(gameObject);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            Debug.Log("MageStaff hit enemy: " + other.name);
            EnemyPoolHandler.Instance.Despawn(other.gameObject);
            ShootPrefabPool.Instance.ReturnShoot(gameObject);
        }
    }
}
