using System.Collections.Generic;
using UnityEngine;

public class EnemyPoolHandler : MonoBehaviour
{
    [Header("Pool Settings")]
    public GameObject[] npcPrefabs;
    public int poolSize = 10;

    private List<GameObject> pool = new List<GameObject>();

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
        for (int i = 0; i < poolSize; i++)
        {
            GameObject npc = Instantiate(npcPrefabs[Random.Range(0, npcPrefabs.Length)],transform);
            npc.GetComponent<AiBar>().CustomerID = "NPC_" + i; // Assign a unique ID to each NPC
            npc.SetActive(false);
            pool.Add(npc);
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