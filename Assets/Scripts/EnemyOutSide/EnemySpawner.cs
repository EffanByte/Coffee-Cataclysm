using UnityEngine;
using UnityEngine.Pool;
using System.Collections;

public class EnemySpawner : MonoBehaviour
{
    [Header("Spawner Settings")]
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private int poolCapacity = 10;
    [SerializeField] private float spawnRadius = 20f;
    [SerializeField] private float spawnInterval = 3f;

    [Header("References")]
    [SerializeField] private Transform player;

    private ObjectPool<GameObject> enemyPool;

    private void Start()
    {
        // Create the pool
        enemyPool = new ObjectPool<GameObject>(
            createFunc: () =>
            {
                GameObject enemy = Instantiate(enemyPrefab);
                enemy.SetActive(false);
                enemy.AddComponent<EnemyPoolHandler>().Initialize(enemyPool); // handles OnDespawn
                return enemy;
            },
            actionOnGet: (enemy) => { enemy.SetActive(true); },
            actionOnRelease: (enemy) => { enemy.SetActive(false); },
            actionOnDestroy: (enemy) => { Destroy(enemy); },
            collectionCheck: false,
            defaultCapacity: poolCapacity,
            maxSize: poolCapacity * 2
        );

        StartCoroutine(SpawnEnemiesRoutine());
    }

    private IEnumerator SpawnEnemiesRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);

            if (player == null) continue;

            Vector3 spawnPos = GetRandomPositionNearPlayer();
            GameObject enemy = enemyPool.Get();
            enemy.transform.position = spawnPos;
        }
    }

    private Vector3 GetRandomPositionNearPlayer()
    {
        Vector2 randomCircle = Random.insideUnitCircle * spawnRadius;
        Vector3 spawnPos = player.position + new Vector3(randomCircle.x, 1, randomCircle.y);
        return spawnPos;
    }
}
