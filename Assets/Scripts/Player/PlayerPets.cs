using UnityEngine;

public class PlayerPets : MonoBehaviour
{
    [SerializeField] private GameObject plantPrefab;
    [SerializeField] private GameObject slothPrefab;
    [SerializeField] private GameObject spiderPrefab;
    [SerializeField] private GameObject dragonPrefab;
    [SerializeField] private GameObject serpentrefab;

    private void Awake()
    {
        HidePets();
    }

    void HidePets()
    {
        plantPrefab.SetActive(false);
        spiderPrefab.SetActive(false);
        dragonPrefab.SetActive(false);
        serpentrefab.SetActive(false);
        slothPrefab.SetActive(false);
    }
}
