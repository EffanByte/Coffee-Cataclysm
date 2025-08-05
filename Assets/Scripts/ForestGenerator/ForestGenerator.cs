using System.Collections.Generic;
using UnityEngine;

public class ForestGenerator : MonoBehaviour
{
    [Header("Map Settings")]
    public int chunkSize = 20;
    public int renderDistance = 3;
    public Transform player;

    [Header("Ground")]
    public Material groundMaterial;
    public float groundSize = 1000f; // Large flat ground plane

    [Header("Forest Objects")]
    public GameObject[] trees;
    public GameObject[] rocks;
    public GameObject[] mushrooms;
    public GameObject[] bushes;

    [Header("Spawn Settings")]
    [Range(0f, 1f)] public float treeChance = 0.3f;
    [Range(0f, 1f)] public float rockChance = 0.4f;
    [Range(0f, 1f)] public float mushroomChance = 0.6f;
    [Range(0f, 1f)] public float bushChance = 0.5f;

    [Header("Pattern Settings")]
    public bool spawnAroundTrees = true;
    [Range(1, 5)] public int objectsAroundTree = 3;
    public float treeRadius = 4f;

    [Header("Spacing")]
    public float minSpacing = 2f;
    public float maxSpacing = 5f;

    private Dictionary<Vector2, GameObject> loadedChunks = new Dictionary<Vector2, GameObject>();
    private Vector2 lastPlayerChunk = Vector2.positiveInfinity;
    private GameObject groundPlane;

    void Start()
    {
        if (!player) player = Camera.main.transform;
        CreateGround();
    }

    void Update()
    {
        Vector2 playerChunk = new Vector2(
            Mathf.FloorToInt(player.position.x / chunkSize),
            Mathf.FloorToInt(player.position.z / chunkSize)
        );

        if (playerChunk != lastPlayerChunk)
        {
            UpdateChunks(playerChunk);
            lastPlayerChunk = playerChunk;
        }
    }

    void CreateGround()
    {
        groundPlane = GameObject.CreatePrimitive(PrimitiveType.Plane);
        groundPlane.name = "Ground";
        groundPlane.transform.parent = transform;
        groundPlane.transform.localScale = Vector3.one * groundSize / 10f;
        groundPlane.transform.position = Vector3.zero;

        if (groundMaterial)
            groundPlane.GetComponent<MeshRenderer>().material = groundMaterial;
    }

    void UpdateChunks(Vector2 center)
    {
        // Remove distant chunks
        var toRemove = new List<Vector2>();
        foreach (var chunk in loadedChunks.Keys)
        {
            if (Vector2.Distance(chunk, center) > renderDistance + 0.5f)
                toRemove.Add(chunk);
        }

        foreach (var chunk in toRemove)
        {
            if (loadedChunks[chunk] != null)
                DestroyImmediate(loadedChunks[chunk]);
            loadedChunks.Remove(chunk);
        }

        // Add new chunks
        for (int x = -renderDistance; x <= renderDistance; x++)
        {
            for (int z = -renderDistance; z <= renderDistance; z++)
            {
                Vector2 chunkPos = center + new Vector2(x, z);
                if (!loadedChunks.ContainsKey(chunkPos))
                {
                    loadedChunks[chunkPos] = GenerateChunk(chunkPos);
                }
            }
        }
    }

    GameObject GenerateChunk(Vector2 chunkPos)
    {
        GameObject chunk = new GameObject($"Chunk_{(int)chunkPos.x}_{(int)chunkPos.y}");
        chunk.transform.parent = transform;

        Vector3 chunkWorldPos = new Vector3(chunkPos.x * chunkSize, 0, chunkPos.y * chunkSize);
        chunk.transform.position = chunkWorldPos;

        Random.InitState((int)(chunkPos.x * 1000 + chunkPos.y));
        List<Vector3> usedPositions = new List<Vector3>();

        // === Spawn Trees ===
        int treeCount = Random.Range(2, 5);
        List<Vector3> treePositions = new List<Vector3>();

        for (int i = 0; i < treeCount; i++)
        {
            Vector3 localPos = GetRandomValidPosition(usedPositions);
            if (localPos == Vector3.zero || trees.Length == 0) continue;

            Vector3 worldPos = chunkWorldPos + localPos;
            GameObject tree = Instantiate(trees[Random.Range(0, trees.Length)], worldPos, GetRandomRotation(), chunk.transform);
            tree.transform.localScale = GetRandomScale();
            usedPositions.Add(localPos);
            treePositions.Add(worldPos);
        }

        // === Spawn Around Trees ===
        if (spawnAroundTrees)
        {
            foreach (Vector3 treePos in treePositions)
            {
                for (int i = 0; i < objectsAroundTree; i++)
                {
                    float angle = Random.Range(0f, Mathf.PI * 2f);
                    float radius = Random.Range(1f, treeRadius);
                    Vector3 offset = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * radius;

                    Vector3 nearPos = treePos + offset;
                    if (!IsValidPosition(nearPos, usedPositions)) continue;

                    GameObject extra = null;
                    if (Random.value < 0.5f && bushes.Length > 0)
                        extra = bushes[Random.Range(0, bushes.Length)];
                    else if (mushrooms.Length > 0)
                        extra = mushrooms[Random.Range(0, mushrooms.Length)];

                    if (extra != null)
                    {
                        GameObject obj = Instantiate(extra, nearPos, GetRandomRotation(), chunk.transform);
                        obj.transform.localScale = GetRandomScale();
                        usedPositions.Add(nearPos - chunkWorldPos); // Local pos
                    }
                }
            }
        }

        // === Spawn Rocks & Mushrooms separately ===
        int rockCount = Random.Range(1, 4);
        for (int i = 0; i < rockCount; i++)
        {
            Vector3 localPos = GetRandomValidPosition(usedPositions);
            if (localPos == Vector3.zero || rocks.Length == 0) continue;

            Vector3 worldPos = chunkWorldPos + localPos;
            GameObject rock = Instantiate(rocks[Random.Range(0, rocks.Length)], worldPos, GetRandomRotation(), chunk.transform);
            rock.transform.localScale = GetRandomScale();
            usedPositions.Add(localPos);
        }

        int mushroomCount = Random.Range(2, 5);
        for (int i = 0; i < mushroomCount; i++)
        {
            Vector3 localPos = GetRandomValidPosition(usedPositions);
            if (localPos == Vector3.zero || mushrooms.Length == 0) continue;

            Vector3 worldPos = chunkWorldPos + localPos;
            GameObject mushroom = Instantiate(mushrooms[Random.Range(0, mushrooms.Length)], worldPos, GetRandomRotation(), chunk.transform);
            mushroom.transform.localScale = GetRandomScale();
            usedPositions.Add(localPos);
        }

        return chunk;
    }

    bool IsValidPosition(Vector3 pos, List<Vector3> usedPositions)
    {
        foreach (Vector3 used in usedPositions)
        {
            if (Vector3.Distance(pos, used) < minSpacing)
                return false;
        }
        return true;
    }

    Vector3 GetRandomValidPosition(List<Vector3> usedPositions)
    {
        int attempts = 0;
        while (attempts < 20) // Max attempts to find valid position
        {
            Vector3 pos = new Vector3(
                Random.Range(0f, chunkSize),
                0f,
                Random.Range(0f, chunkSize)
            );

            bool validPosition = true;
            foreach (Vector3 used in usedPositions)
            {
                if (Vector3.Distance(pos, used) < minSpacing)
                {
                    validPosition = false;
                    break;
                }
            }

            if (validPosition)
                return pos;

            attempts++;
        }
        return Vector3.zero; // No valid position found
    }

    GameObject SelectRandomObject()
    {
        float rand = Random.value;

        if (rand < treeChance && trees.Length > 0)
            return trees[Random.Range(0, trees.Length)];
        else if (rand < treeChance + rockChance && rocks.Length > 0)
            return rocks[Random.Range(0, rocks.Length)];
        else if (rand < treeChance + rockChance + mushroomChance && mushrooms.Length > 0)
            return mushrooms[Random.Range(0, mushrooms.Length)];
        else if (rand < treeChance + rockChance + mushroomChance + bushChance && bushes.Length > 0)
            return bushes[Random.Range(0, bushes.Length)];

        // Fallback to any available object
        var allObjects = new List<GameObject>();
        allObjects.AddRange(trees);
        allObjects.AddRange(rocks);
        allObjects.AddRange(mushrooms);
        allObjects.AddRange(bushes);

        return allObjects.Count > 0 ? allObjects[Random.Range(0, allObjects.Count)] : null;
    }

    Quaternion GetRandomRotation()
    {
        return Quaternion.Euler(-90, Random.Range(0f, 360f), 0);
    }

    Vector3 GetRandomScale()
    {
        float scale = Random.Range(100, 100f);
        return Vector3.one * scale;
    }

    void OnDrawGizmosSelected()
    {
        if (player == null) return;

        Vector2 playerChunk = new Vector2(
            Mathf.FloorToInt(player.position.x / chunkSize),
            Mathf.FloorToInt(player.position.z / chunkSize)
        );

        Gizmos.color = Color.yellow;
        for (int x = -renderDistance; x <= renderDistance; x++)
        {
            for (int z = -renderDistance; z <= renderDistance; z++)
            {
                Vector2 chunkPos = playerChunk + new Vector2(x, z);
                Vector3 center = new Vector3(chunkPos.x * chunkSize + chunkSize / 2, 0, chunkPos.y * chunkSize + chunkSize / 2);
                Gizmos.DrawWireCube(center, new Vector3(chunkSize, 2, chunkSize));
            }
        }
    }
}