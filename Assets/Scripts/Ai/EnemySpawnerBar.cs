using UnityEngine;

public class EnemySpawnerBar : MonoBehaviour
{
    [SerializeField] private EnemyBarPool enemyPool;
    [SerializeField] private WaypointManager waypointManager;
    [SerializeField] private float spawnInterval = 5f;

    private float timer;

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= spawnInterval)
        {
            SpawnEnemy();
            timer = 0f;
        }
    }

    private void SpawnEnemy()
    {
        GameObject enemy = enemyPool.GetEnemy();
        if (enemy != null)
        {
            enemy.transform.SetPositionAndRotation(transform.position, transform.rotation);
            var ai = enemy.GetComponent<AiBar>();
            ai.Initialize(waypointManager);
        }
    }
}
