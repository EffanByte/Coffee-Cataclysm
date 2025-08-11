using System.Collections.Generic;
using UnityEngine;

public class ShootPrefabPool : MonoBehaviour
{
    public static ShootPrefabPool Instance;

    [Header("Pooling Settings")]
    public GameObject shootPrefab; // Projectile prefab
    public int poolSize = 10;

    private Queue<GameObject> shootPool = new Queue<GameObject>();

    void Awake()
    {
        Instance = this;
        CreatePool();
    }

    private void CreatePool()
    {
        for (int i = 0; i < poolSize; i++)
        {
            GameObject proj = Instantiate(shootPrefab, transform);
            proj.SetActive(false);
            shootPool.Enqueue(proj);
        }
    }

    public GameObject GetShoot()
    {
        if (shootPool.Count > 0)
        {
            GameObject proj = shootPool.Dequeue();
            proj.SetActive(true);
            return proj;
        }
        else
        {
            // Optional: Expand pool if needed
            GameObject proj = Instantiate(shootPrefab, transform);
            return proj;
        }
    }

    public void ReturnShoot(GameObject proj)
    {
        proj.SetActive(false);
        shootPool.Enqueue(proj);
    }
}