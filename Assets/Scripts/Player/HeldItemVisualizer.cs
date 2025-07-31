using UnityEngine;

public class HeldItemVisualizer : MonoBehaviour
{
    [SerializeField] private Transform visualAnchor;

    [Header("Held Item Prefabs")]
    [SerializeField] private GameObject coffeeBeanBrownPrefab;
    [SerializeField] private GameObject coffeeBeanWhitePrefab;
    [SerializeField] private GameObject cupPrefab;
    [SerializeField] private GameObject waterJugPrefab;

    private GameObject currentVisual;

    public void Show(HeldItemType item)
    {
        Hide(); // Clear old one

        GameObject prefab = GetPrefabForItem(item);
        if (prefab == null)
        {
            Debug.LogWarning($"No prefab assigned for: {item}");
            return;
        }

        currentVisual = Instantiate(prefab, visualAnchor);
        currentVisual.transform.localPosition = Vector3.zero;

        // Optional: face camera
        //  currentVisual.AddComponent<Billboard>();
    }

    public void Hide()
    {
        if (currentVisual != null)
        {
            Destroy(currentVisual);
            currentVisual = null;
        }
    }

    private GameObject GetPrefabForItem(HeldItemType item)
    {
        return item switch
        {
            HeldItemType.CoffeeBeanBrown => coffeeBeanBrownPrefab,
            HeldItemType.CoffeeBeanWhite => coffeeBeanWhitePrefab,
            HeldItemType.Cup => cupPrefab,
            HeldItemType.WaterJug => waterJugPrefab,
            _ => null
        };
    }
}
