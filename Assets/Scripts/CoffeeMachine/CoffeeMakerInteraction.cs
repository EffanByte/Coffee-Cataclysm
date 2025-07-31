using UnityEngine;
using UnityEngine.UI;

public class CoffeeMakerInteraction : MonoBehaviour
{
    [Header("Brewing Settings")]
    public float brewDuration = 2f;
    public float vibrationMagnitude = 0.05f;

    [Header("Progress Bar UI")]
    public Slider progressBar;
    public Image progressBorder;
    public Color doneColor = Color.yellow;

    [Header("Spawn Settings")]
    public GameObject coffeeBlendPrefab; // Assign a prefab that has CoffeeBlendItem on it
    public Transform spawnPoint; // Where the coffee appears

    private Vector3 originalPosition;
    private float timer = 0f;
    private bool isBrewing = false;
    private bool isBlendReady = false;

    private HeldItemType currentBean = HeldItemType.None;

    void Start()
    {
        originalPosition = transform.localPosition;

        if (progressBar != null)
        {
            progressBar.gameObject.SetActive(false);
            progressBar.value = 0;
        }

        if (progressBorder != null)
            progressBorder.enabled = false;
    }

    void Update()
    {
        if (isBrewing)
        {
            timer += Time.deltaTime;

            if (timer < brewDuration)
            {
                transform.localPosition = originalPosition + Random.insideUnitSphere * vibrationMagnitude;
                if (progressBar != null)
                    progressBar.value = timer / brewDuration;
            }
            else
            {
                FinishBrewing();
            }
        }
    }

    public bool CanStartBrewing() => !isBrewing && !isBlendReady;

    public void StartBrewing(HeldItemType beanType)
    {
        if (!CanStartBrewing()) return;

        currentBean = beanType;
        isBrewing = true;
        timer = 0;
        originalPosition = transform.localPosition;

        if (progressBar != null)
        {
            progressBar.gameObject.SetActive(true);
            progressBar.value = 0;
        }

        if (progressBorder != null)
            progressBorder.enabled = false;
    }

    void FinishBrewing()
    {
        isBrewing = false;
        transform.localPosition = originalPosition;
        isBlendReady = true;

        if (progressBar != null)
            progressBar.value = 1;

        if (progressBorder != null)
        {
            progressBorder.enabled = true;
            progressBorder.color = doneColor;
        }

        // if (coffeeBlendPrefab != null && spawnPoint != null)
        // {
        //     GameObject blend = Instantiate(coffeeBlendPrefab, spawnPoint.position, Quaternion.identity);
        //     var blendItem = blend.GetComponent<CoffeeBlendItem>();

        //     if (blendItem != null)
        //     {
        //         if (currentBean == HeldItemType.CoffeeBeanBrown)
        //             blendItem.SetBlendType(HeldItemType.CoffeeBlendBrown);
        //         else if (currentBean == HeldItemType.CoffeeBeanWhite)
        //             blendItem.SetBlendType(HeldItemType.CoffeeBlendWhite);
        //     }
        // }

        Invoke(nameof(HideProgressBar), 1.5f);
    }

    void HideProgressBar()
    {
        if (progressBar != null)
            progressBar.gameObject.SetActive(false);
    }
}
