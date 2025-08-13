using UnityEngine;

public class SpiderWeb : MonoBehaviour
{
    private Transform target;
    public float speed = 10f;

    public void SetTarget(Transform enemy)
    {
        target = enemy;
    }

    private void Update()
    {
        if (target == null)
        {
            SpiderWebPool.Instance.ReturnProjectile(gameObject);
            return;
        }

        transform.position = Vector3.MoveTowards(transform.position, target.position, speed * Time.deltaTime);

        // If hit target
        if (Vector3.Distance(transform.position, target.position) < 0.1f)
        {
            // Enemy hit logic can go here
            SpiderWebPool.Instance.ReturnProjectile(gameObject);
        }
    }
}
