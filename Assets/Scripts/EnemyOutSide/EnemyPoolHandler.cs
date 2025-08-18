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
        if (npcPrefabs == null || npcPrefabs.Length == 0) return;
        poolByPrefabIndex.Clear();
        for (int idx = 0; idx < npcPrefabs.Length; idx++)
        {
            poolByPrefabIndex[idx] = new Queue<GameObject>();
        }

        for (int i = 0; i < poolSize; i++)
        {
            int prefabIndex = i % npcPrefabs.Length;
            GameObject npc = Instantiate(npcPrefabs[prefabIndex], transform);
            var meta = npc.GetComponent<EnemyMeta>();
            if (meta == null) meta = npc.AddComponent<EnemyMeta>();
            meta.PrefabIndex = prefabIndex;
            npc.SetActive(false);
            pool.Add(npc);
            poolByPrefabIndex[prefabIndex].Enqueue(npc);
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

        if (!poolByPrefabIndex.ContainsKey(prefabIndex))
        {
            poolByPrefabIndex[prefabIndex] = new Queue<GameObject>();
        }

        // Try to find an inactive of this type
        Queue<GameObject> queue = poolByPrefabIndex[prefabIndex];
        int count = queue.Count;
        for (int i = 0; i < count; i++)
        {
            GameObject npc = queue.Dequeue();
            queue.Enqueue(npc);
            if (!npc.activeInHierarchy)
            {
                npc.SetActive(true);
                return npc;
            }
        }

        // Expand pool for this prefab type if needed
        GameObject created = Instantiate(npcPrefabs[prefabIndex], transform);
        var metaCreated = created.GetComponent<EnemyMeta>();
        if (metaCreated == null) metaCreated = created.AddComponent<EnemyMeta>();
        metaCreated.PrefabIndex = prefabIndex;
        created.SetActive(true);
        pool.Add(created);
        queue.Enqueue(created);
        return created;
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