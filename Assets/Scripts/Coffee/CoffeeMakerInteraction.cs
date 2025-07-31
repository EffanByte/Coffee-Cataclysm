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

    [Header("Blend Spawn")]
    public GameObject coffeeBlendPrefab;
    public Transform blendSpawnPoint;

    private Vector3 originalPosition;
    private float timer = 0f;
    private bool isBrewing = false;

    private GameObject spawnedBlend;
    private bool isBlendReady = false;
    private float blendTimer = 0f;
    private float blendTimeout = 7f;

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

        if (isBlendReady && spawnedBlend != null)
        {
            blendTimer += Time.deltaTime;
            if (blendTimer >= blendTimeout)
            {
                Debug.Log("Blend burned!");
                Destroy(spawnedBlend);
                isBlendReady = false;
                blendTimer = 0f;
            }
        }
    }

    public void StartBrewing(HeldItemType beanType)
    {
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

        if (progressBar != null)
            progressBar.value = 1;

        if (progressBorder != null)
        {
            progressBorder.enabled = true;
            progressBorder.color = doneColor;
        }

        if (coffeeBlendPrefab != null && blendSpawnPoint != null)
        {
            spawnedBlend = Instantiate(coffeeBlendPrefab, blendSpawnPoint.position, Quaternion.identity);
            isBlendReady = true;
            blendTimer = 0f;

            var blendItem = spawnedBlend.GetComponent<CoffeeBlendItem>();
            if (blendItem != null)
            {
                HeldItemType blend = currentBean == HeldItemType.CoffeeBeanBrown
                    ? HeldItemType.CoffeeBlendBrown
                    : HeldItemType.CoffeeBlendWhite;

                blendItem.SetBlendType(blend);
            }
        }

        Invoke(nameof(HideProgressBar), 1.5f);
    }

    void HideProgressBar()
    {
        if (progressBar != null)
            progressBar.gameObject.SetActive(false);
    }

    // 🔧 New methods for PlayerInteractor access

    public bool CanStartBrewing()
    {
        return !isBrewing && !isBlendReady;
    }

    public bool IsBlendReady()
    {
        return isBlendReady && spawnedBlend != null;
    }

    public void GiveBlendToPlayer()
    {
        if (!IsBlendReady()) return;

        if (spawnedBlend.TryGetComponent(out CoffeeBlendItem blendItem))
        {
            HeldItemType blendType = blendItem.blendType;

            if (blendType != HeldItemType.None &&
                Player.transformPlayer.TryGetComponent(out HoldingManager holding))
            {
                if (!holding.IsHoldingItem)
                {
                    holding.PickUpItem(blendType);
                    Destroy(spawnedBlend);
                    spawnedBlend = null;
                    isBlendReady = false;
                    blendTimer = 0f;
                    Debug.Log("Player picked up blend.");
                }
                else
                {
                    Debug.Log("Player already holding something.");
                }
            }
        }
    }
}
