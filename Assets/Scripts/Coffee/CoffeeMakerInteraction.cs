using UnityEngine;
using UnityEngine.UI;
using UnityHFSM;
public enum BlendState
{
    NoBlend,
    LowBlend,
    MediumBlend,
    HighBlend
}
public class CoffeeMakerInteraction : MonoBehaviour
{

    private StateMachine<BlendState> blendFSM;

    [Header("Brewing Settings")]
    [SerializeField] private float brewDuration = 5f;
    public float vibrationMagnitude = 0.05f;

    [Header("Progress Bar UI")]
    public Slider progressBar;
    public Color doneColor = Color.yellow;

    [Header("Blend Spawn")]
    public GameObject coffeeBlendPrefab;
    public Transform blendSpawnPoint;

    private Vector3 originalPosition;
    private float timer = 0f;
    private bool isBrewing = false;

    private GameObject spawnedBlend;
    private bool isBlendReady = false;
    [SerializeField] private float blendTimer = 0f;
    [SerializeField] private float blendTimeout = 7f;


    private HeldItemType currentBean = HeldItemType.None;


    void Start()
    {
        blendFSM = new StateMachine<BlendState>();


        blendFSM.AddState(BlendState.NoBlend, new State<BlendState>(
            onEnter: s => Debug.Log("Entered: NoBlend")
        ));
        blendFSM.AddState(BlendState.LowBlend, new State<BlendState>(
            onEnter: s => Debug.Log("Entered: LowBlend")
        ));
        blendFSM.AddState(BlendState.MediumBlend, new State<BlendState>(
            onEnter: s => Debug.Log("Entered: MediumBlend")
        ));
        blendFSM.AddState(BlendState.HighBlend, new State<BlendState>(
            onEnter: s => Debug.Log("Entered: HighBlend")
        ));

        blendFSM.SetStartState(BlendState.NoBlend);

        blendFSM.AddTransition(BlendState.NoBlend, BlendState.LowBlend, _ => timer >= brewDuration * 0.2f && timer < brewDuration * 0.4f);
        blendFSM.AddTransition(BlendState.LowBlend, BlendState.MediumBlend, _ =>timer >= brewDuration * 0.4f && timer < brewDuration * 0.7f);
        blendFSM.AddTransition(BlendState.MediumBlend, BlendState.HighBlend, _ =>timer >= brewDuration * 0.7f && timer < brewDuration * 1.2f);

        blendFSM.Init();

        originalPosition = transform.localPosition;
       
        if (progressBar != null)
        {
            progressBar.gameObject.SetActive(false);
            progressBar.value = 0;
        }
        
    }

    void Update()
    {
        blendFSM.OnLogic();
        Debug.Log(timer);
        float percent = timer / brewDuration;
        
        if (isBrewing)
        {
            timer += Time.deltaTime;

            if (timer < brewDuration + 0.2f)
            {
                transform.localPosition = originalPosition + Random.insideUnitSphere * vibrationMagnitude;
                if (progressBar != null)
                    progressBar.value = percent;
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
    }

    void FinishBrewing()
    {
        isBrewing = false;
        transform.localPosition = originalPosition;

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
