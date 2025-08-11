using System.Collections.Generic;
using UnityEngine;

public class ArrowPool : MonoBehaviour
{
    public static ArrowPool Instance;

    [Header("Pooling Settings")]
    public GameObject arrowPrefab;
    public int poolSize = 10;

    private Queue<GameObject> arrowPool = new Queue<GameObject>();

    void Awake()
    {
        Instance = this;
        CreatePool();
    }

    private void CreatePool()
    {
        for (int i = 0; i < poolSize; i++)
        {
            GameObject arrow = Instantiate(arrowPrefab, transform);
            arrow.SetActive(false);
            arrowPool.Enqueue(arrow);
        }
    }

    public GameObject GetArrow()
    {
        if (arrowPool.Count > 0)
        {
            GameObject arrow = arrowPool.Dequeue();
            arrow.SetActive(true);
            return arrow;
        }
        else
        {
            // Optional: expand pool if needed
            GameObject arrow = Instantiate(arrowPrefab, transform);
            return arrow;
        }
    }

    public void ReturnArrow(GameObject arrow)
    {
        arrow.SetActive(false);
        arrowPool.Enqueue(arrow);
    }
}
