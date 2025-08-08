using System.Collections.Generic;
using UnityEngine;

public class EnemyBarPool : MonoBehaviour
{
    [SerializeField] private GameObject[] enemyPrefabs;
    [SerializeField] private int poolSize = 10;

    private Queue<GameObject> pool = new Queue<GameObject>();

    void Start()
    {
        for (int i = 0; i < poolSize; i++)
        {
            GameObject randomPrefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];
            randomPrefab.GetComponent<AiBar>().CustomerID = "NPC_" + i; // Assign a unique ID to each NPC
            GameObject enemy = Instantiate(randomPrefab);
            enemy.SetActive(false);
            pool.Enqueue(enemy);
        }
    }

    public GameObject GetEnemy()
    {
        if (pool.Count > 0)
        {
            GameObject enemy = pool.Dequeue();
            enemy.SetActive(true);
            return enemy;
        }

        return null; // Optional: expand pool here
    }

    public void ReturnEnemy(GameObject enemy)
    {
        enemy.SetActive(false);
        pool.Enqueue(enemy);
    }
}
