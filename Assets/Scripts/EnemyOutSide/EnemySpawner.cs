using UnityEngine;
using System.Collections;

public class EnemySpawner : MonoBehaviour
{
    [Header("Spawner Settings")]
    [SerializeField] private float spawnRadius = 20f;
    [SerializeField] private float spawnInterval = 3f;

    [Header("References")]
    [SerializeField] private Transform player;

    private void Start()
    {
        StartCoroutine(SpawnEnemiesRoutine());
    }

    private IEnumerator SpawnEnemiesRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);

            if (player == null) continue;

            Vector3 spawnPos = GetRandomPositionNearPlayer();

            GameObject enemy = EnemyPoolHandler.Instance.GetNPC();
            if (enemy != null)
            {
                enemy.transform.position = spawnPos;
            }
        }
    }

    private Vector3 GetRandomPositionNearPlayer()
    {
        Vector2 randomCircle = Random.insideUnitCircle * spawnRadius;
        return player.position + new Vector3(randomCircle.x, 0f, randomCircle.y);
    }
}