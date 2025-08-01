using UnityEngine;
using UnityEngine.Pool;
using System.Collections;

public class EnemySpawner : MonoBehaviour
{
    [Header("Spawner Settings")]
    [SerializeField] private GameObject[] enemyPrefabs;
    [SerializeField] private int poolCapacity = 10;
    [SerializeField] private float spawnRadius = 20f;
    [SerializeField] private float spawnInterval = 3f;

    [Header("References")]
    [SerializeField] private Transform player;

    private ObjectPool<GameObject>[] enemyPools;

    private void Start()
    {
        // Create a pool for each enemy prefab
        enemyPools = new ObjectPool<GameObject>[enemyPrefabs.Length];

        for (int i = 0; i < enemyPrefabs.Length; i++)
        {
            int index = i; // Avoid closure issues in lambdas

            enemyPools[i] = new ObjectPool<GameObject>(
                createFunc: () =>
                {
                    GameObject enemy = Instantiate(enemyPrefabs[index]);
                    enemy.SetActive(false);

                    var poolHandler = enemy.AddComponent<EnemyPoolHandler>();
                    poolHandler.Initialize(enemyPools[index]);
                    return enemy;
                },
                actionOnGet: (enemy) => enemy.SetActive(true),
                actionOnRelease: (enemy) => enemy.SetActive(false),
                actionOnDestroy: (enemy) => Destroy(enemy),
                collectionCheck: false,
                defaultCapacity: poolCapacity,
                maxSize: poolCapacity * 2
            );
        }

        StartCoroutine(SpawnEnemiesRoutine());
    }

    private IEnumerator SpawnEnemiesRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);

            if (player == null) continue;

            Vector3 spawnPos = GetRandomPositionNearPlayer();

            // Randomly select an enemy pool
            int index = Random.Range(0, enemyPools.Length);
            GameObject enemy = enemyPools[index].Get();
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
