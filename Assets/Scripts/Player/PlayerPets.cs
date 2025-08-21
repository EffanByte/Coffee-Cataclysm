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
    private void Start()
    {
        OutsideCanvas.OnSlothActive += OutsideCanvas_OnSlothActive;
        OutsideCanvas.OnDragonActive += OutsideCanvas_OnDragonActive;
        OutsideCanvas.OnPlantActive += OutsideCanvas_OnPlantActive;
        OutsideCanvas.OnSerpentActive += OutsideCanvas_OnSerpentActive;
        OutsideCanvas.OnSpiderActive += OutsideCanvas_OnSpiderActive;
    }

    private void OutsideCanvas_OnSpiderActive()
    {
        spiderPrefab.SetActive(true);
    }

    private void OutsideCanvas_OnSerpentActive()
    {
       serpentrefab.SetActive(true);
    }

    private void OutsideCanvas_OnPlantActive()
    {
        plantPrefab.SetActive(true);
    }

    private void OutsideCanvas_OnDragonActive()
    {
        dragonPrefab.SetActive(true);
    }

    private void OutsideCanvas_OnSlothActive()
    {
        slothPrefab.SetActive(true);
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
