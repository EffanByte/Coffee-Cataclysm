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

    private Vector3 originalPosition;
    private float timer = 0f;
    private bool isBrewing = false;

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

    public void StartBrewing()
    {
        if (isBrewing) return;

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

        Invoke(nameof(HideProgressBar), 1.5f);
    }

    void HideProgressBar()
    {
        if (progressBar != null)
            progressBar.gameObject.SetActive(false);
    }

    private HeldItemType currentBean = HeldItemType.None; // top of class

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
}
