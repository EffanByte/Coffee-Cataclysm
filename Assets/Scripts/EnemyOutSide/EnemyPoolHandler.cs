using System.Collections.Generic;
using UnityEngine;

public class EnemyPoolHandler : MonoBehaviour
{
    [Header("Pool Settings")]
    public GameObject[] npcPrefabs;
    public int poolSize = 10;

    private List<GameObject> pool = new List<GameObject>();
    private Dictionary<int, Queue<GameObject>> poolByPrefabIndex = new Dictionary<int, Queue<GameObject>>();

    public static EnemyPoolHandler Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        poolByPrefabIndex.Clear();
        pool.Clear();

        // Find all pooled enemies under this handler (include inactive children)
        var metas = GetComponentsInChildren<EnemyMeta>(includeInactive: true);

        foreach (var meta in metas)
        {
            var go = meta.gameObject;

            // Ensure pooled objects start disabled
            if (go.activeSelf) go.SetActive(false);

            pool.Add(go);

            // Group by PrefabIndex (must be set on each child in the editor or beforehand)
            int prefabIndex = meta.PrefabIndex;
            if (!poolByPrefabIndex.TryGetValue(prefabIndex, out var q))
            {
                q = new Queue<GameObject>();
                poolByPrefabIndex[prefabIndex] = q;
            }
            q.Enqueue(go);
        }

        if (pool.Count == 0)
        {
            Debug.LogWarning($"{nameof(EnemyPoolHandler)}: No pooled enemies found under '{name}'. " +
                             "Place inactive child objects with EnemyMeta (PrefabIndex set) to use as the pool.");
        }
    }


    public GameObject GetNPC()
    {
        foreach (GameObject npc in pool)
        {
            if (!npc.activeInHierarchy)
            {
                npc.SetActive(true);
                return npc;
            }
        }

        Debug.LogWarning("No available NPC in pool!");
        return null;
    }

    public GameObject GetNPC(int prefabIndex)
    {
        if (npcPrefabs == null || npcPrefabs.Length == 0) return null;
        if (prefabIndex < 0 || prefabIndex >= npcPrefabs.Length) prefabIndex = 0;

        // Ensure a queue exists for this index
        if (!poolByPrefabIndex.TryGetValue(prefabIndex, out var queue) || queue.Count == 0)
        {
            Debug.LogWarning($"[EnemyPoolHandler] No pooled NPCs available for prefabIndex={prefabIndex}.");
            return null;
        }

        // Round-robin scan for an inactive instance
        int count = queue.Count;
        for (int i = 0; i < count; i++)
        {
            var npc = queue.Dequeue();
            queue.Enqueue(npc); // keep order

            if (npc == null) continue;                 // skip destroyed entries
            if (!npc.activeInHierarchy)
            {
                npc.SetActive(true);
                return npc;
            }
        }
        
        Debug.LogWarning($"[EnemyPoolHandler] Pool exhausted for prefabIndex={prefabIndex}. " +
                         $"Increase pool size or release an active NPC.");
        return null;
    }

    public int GetPrefabIndexByName(string prefabName)
    {
        if (npcPrefabs == null) return 0;
        for (int i = 0; i < npcPrefabs.Length; i++)
        {
            if (npcPrefabs[i] != null && npcPrefabs[i].name == prefabName)
                return i;
        }
        return 0;
    }

    /// <summary>
    /// Despawns a specific enemy and returns it to the pool.
    /// </summary>
    public void Despawn(GameObject enemy)
    {
        if (pool.Contains(enemy))
        {
            enemy.SetActive(false);
        }
        else
        {
            Debug.LogWarning("Trying to despawn an enemy that is not in the pool!");
        }
    }
}