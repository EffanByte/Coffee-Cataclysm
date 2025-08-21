using UnityEngine;
using System.Collections;
using Playroom;
using System.Linq;

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

    private void Awake()
    {
        // Grab PlayroomKit as early as possible
        _playroom = PlayroomManager.Instance.GetPlayroomKit();
    }

    private void Start()
    {
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

            // Spawn around every outside player's position once per interval
            int outsideCount = CountOutsidePlayers();
            if (EnemyPoolHandler.Instance == null || EnemyPoolHandler.Instance.npcPrefabs == null || EnemyPoolHandler.Instance.npcPrefabs.Length == 0)
                continue;

            bool shouldBroadcast = outsideCount > 1;
            foreach (var anchor in GetOutsidePlayerPositions())
            {
                Vector3 spawnPos = GetRandomPositionNear(anchor);
                int prefabIndex = Random.Range(0, EnemyPoolHandler.Instance.npcPrefabs.Length);

                // Spawn locally
                GameObject enemy = EnemyPoolHandler.Instance.GetNPC(prefabIndex);
                if (enemy != null)
                {
                    enemy.transform.position = spawnPos;
                }

                // Broadcast to other outside clients so they mirror this spawn
                if (shouldBroadcast)
                {
                    string payload = prefabIndex + "|?|" + spawnPos.x + "|?|" + spawnPos.y + "|?|" + spawnPos.z;
                    _playroom.RpcCall("HandleEnemySpawn", payload, PlayroomKit.RpcMode.OTHERS);
                }
            }
        }
    }

    private Vector3 GetRandomPositionNear(Vector3 center)
    {
        Vector2 randomCircle = Random.insideUnitCircle * spawnRadius;
        return center + new Vector3(randomCircle.x, 0f, randomCircle.y);
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
                Debug.Log("Outside host updated: " + isOutsideHost + ", Host ID: " + outsideHostId);

                if (!isOutsideHost && CountOutsidePlayers() > 0)
                {
                    _playroom.RpcCall("RequestEnemySnapshot", string.Empty, PlayroomKit.RpcMode.OTHERS);
                    yield break; // request once; routine ends. It will be restarted next OnEnable.
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
        var queue = PlayroomManager.Instance.GetOutsideQueue();

        outsideHostId = null;
        foreach (var kv in queue)
        {
            if (kv.Value) // true means this player is the outside host
            {
                outsideHostId = kv.Key; // first true is the host
                break;
            }
        }

        if (outsideHostId == null)
        {
            var me = _playroom?.MyPlayer().id;
            queue[me] = true;                   //no host found, mark me as host
            outsideHostId = me;
        }

        isOutsideHost = _playroom != null && _playroom.MyPlayer() != null &&
                        _playroom.MyPlayer().id == outsideHostId;
    }

    private System.Collections.Generic.List<Vector3> GetOutsidePlayerPositions()
    {
        var list = new System.Collections.Generic.List<Vector3>();
        foreach (var kv in PlayroomManager.Players)
        {
            if (kv.Value.gameState == GameSceneState.OutSide && kv.Value.playerObject != null)
            {
                list.Add(kv.Value.playerObject.transform.position);
            }
        }
        return list;
    }
}
