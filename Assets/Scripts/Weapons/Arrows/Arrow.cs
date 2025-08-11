using TMPro;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    public float speed = 20f;
    public float lifetime = 3f;

    private float lifeTimer;
    private Vector3 moveDirection;

    public void Launch(Vector3 targetPosition)
    {
        // Lock direction toward target at the moment of firing
        moveDirection = (targetPosition - transform.position).normalized;
        transform.forward = moveDirection;
    }

    void OnEnable()
    {
        lifeTimer = lifetime;
    }

    void Update()
    {
        transform.position += moveDirection * speed * Time.deltaTime;
        if (moveDirection != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(moveDirection);
            transform.rotation *= Quaternion.Euler(-90f, 0f, 0f); // model offset
        }
        lifeTimer -= Time.deltaTime;
        if (lifeTimer <= 0f)
        {
            ArrowPool.Instance.ReturnArrow(gameObject);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            Debug.Log("Hit enemy: " + other.name);
           // other.gameObject.SetActive(false);
            EnemyPoolHandler.Instance.Despawn(other.gameObject);
            ArrowPool.Instance.ReturnArrow(gameObject);
        }
    }
}
