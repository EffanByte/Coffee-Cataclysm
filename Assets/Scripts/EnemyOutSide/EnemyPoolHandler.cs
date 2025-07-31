using UnityEngine;
using UnityEngine.Pool;

public class EnemyPoolHandler : MonoBehaviour
{

    private ObjectPool<GameObject> pool;

    public void Initialize(ObjectPool<GameObject> objectPool)
    {
        pool = objectPool;
    }

    public void Despawn()
    {
        if (pool != null)
        {
            pool.Release(gameObject);
        }
        else
        {
            Debug.LogWarning("No pool assigned to enemy.");
            Destroy(gameObject);
        }
    }
}
