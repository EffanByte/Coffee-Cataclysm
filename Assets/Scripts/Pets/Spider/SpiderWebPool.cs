using System.Collections.Generic;
using UnityEngine;

public class SpiderWebPool : MonoBehaviour
{
    public static SpiderWebPool Instance;
    public GameObject projectilePrefab;
    public int poolSize = 10;

    private Queue<GameObject> pool = new Queue<GameObject>();

    private void Awake()
    {
        Instance = this;
        for (int i = 0; i < poolSize; i++)
        {
            GameObject obj = Instantiate(projectilePrefab, transform);
            obj.SetActive(false);
            pool.Enqueue(obj);
        }
    }

    public GameObject GetProjectile()
    {
        if (pool.Count > 0)
        {
            GameObject obj = pool.Dequeue();
            obj.SetActive(true);
            return obj;
        }
        else
        {
            GameObject obj = Instantiate(projectilePrefab, transform);
            return obj;
        }
    }

    public void ReturnProjectile(GameObject projectile)
    {
        projectile.SetActive(false);
        pool.Enqueue(projectile);
    }
}
