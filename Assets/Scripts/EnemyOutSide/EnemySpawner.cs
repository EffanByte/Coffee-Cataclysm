using UnityEngine;
using System.Collections;
using Playroom;

public class EnemySpawner : MonoBehaviour
{
    [Header("Spawner Settings")]
    [SerializeField] private float spawnRadius = 20f;
    [SerializeField] private float spawnInterval = 3f;

    [Header("References")]
    [SerializeField] private Transform player;

    private PlayroomKit _playroom;
    private bool isOutsideHost = false;
    private string outsideHostId = null;

    private void Start()
    {
        _playroom = PlayroomManager.Instance.GetPlayroomKit();
        StartCoroutine(SpawnEnemiesRoutine());
        StartCoroutine(RequestSnapshotIfNeeded());
    }

    private IEnumerator SpawnEnemiesRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);

            if (player == null) continue;

            // Only act in outside scene
            if (GameStateManager.Instance == null || GameStateManager.Instance.currentState != GameSceneState.OutSide)
                continue;

            // Recompute outside host each tick
            UpdateOutsideHost();

            if (!isOutsideHost)
                continue; // non-host does nothing; will receive spawn RPCs

            // If only one outside client (me), we still spawn locally without broadcasting
            Vector3 spawnPos = GetRandomPositionNearHost();
            int prefabIndex = Random.Range(0, EnemyPoolHandler.Instance.npcPrefabs.Length);

            // Spawn locally
            GameObject enemy = EnemyPoolHandler.Instance.GetNPC(prefabIndex);
            if (enemy != null)
            {
                enemy.transform.position = spawnPos;
            }

            // If there is more than one outside player, broadcast
            if (CountOutsidePlayers() > 1)
            {
                string payload = prefabIndex + "|?|" + spawnPos.x + "|?|" + spawnPos.y + "|?|" + spawnPos.z;
                _playroom.RpcCall("HandleEnemySpawn", payload, PlayroomKit.RpcMode.OTHERS);
            }
        }
    }

    private Vector3 GetRandomPositionNearHost()
    {
        Vector2 randomCircle = Random.insideUnitCircle * spawnRadius;
        return player.position + new Vector3(randomCircle.x, 0f, randomCircle.y);
    }

    private IEnumerator RequestSnapshotIfNeeded()
    {
        // Wait a short time for scene transitions/state to settle
        yield return new WaitForSeconds(0.5f);

        while (true)
        {
            // Only in Outside and only if I'm not the host and there is at least one outside player
            if (GameStateManager.Instance != null && GameStateManager.Instance.currentState == GameSceneState.OutSide)
            {
                UpdateOutsideHost();
                if (!isOutsideHost && CountOutsidePlayers() > 0)
                {
                    _playroom.RpcCall("RequestEnemySnapshot", string.Empty, PlayroomKit.RpcMode.OTHERS);
                    yield break; // request once
                }
            }
            yield return new WaitForSeconds(0.5f);
        }
    }

    private int CountOutsidePlayers()
    {
        int count = 0;
        foreach (var kv in PlayroomManager.Players)
        {
            if (kv.Value.gameState == GameSceneState.OutSide)
                count++;
        }
        return count;
    }

    private void UpdateOutsideHost()
    {
        // Elect earliest-arrived outside client as host using PlayroomManager's queue
        var queue = (System.Collections.Generic.IReadOnlyList<string>)PlayroomManager.Instance.GetOutsideQueue();
        outsideHostId = queue.Count > 0 ? queue[0] : null;
        isOutsideHost = outsideHostId != null && _playroom.MyPlayer().id == outsideHostId;
    }
}