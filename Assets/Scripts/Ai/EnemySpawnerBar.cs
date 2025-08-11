using UnityEngine;
using Playroom;
public class EnemySpawnerBar : MonoBehaviour
{
    [SerializeField] private EnemyBarPool enemyPool;
    [SerializeField] private WaypointManager waypointManager;
    [SerializeField] private float spawnInterval = 5f;
    private PlayroomKit _playroom;
    private float timer;
    void Start()
    {
        _playroom = PlayroomManager.Instance.GetPlayroomKit();
        if (_playroom == null)
        {
            Debug.LogError("PlayroomKit instance not found!");
            return;
        }

        if (enemyPool == null)
        {
            Debug.LogError("EnemyBarPool is not assigned!");
            return;
        }

        if (waypointManager == null)
        {
            Debug.LogError("WaypointManager is not assigned!");
            return;
        }

        timer = 0f;
    }
    void Update()
    {
        if (PlayroomManager.Instance.spawned)
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
