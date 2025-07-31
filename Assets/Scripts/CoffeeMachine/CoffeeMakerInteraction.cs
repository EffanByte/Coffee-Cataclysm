using UnityEngine;
using UnityEngine.UI;

public class CoffeeMakerInteraction : MonoBehaviour
{
    [Header("Player Interaction")]
    public Transform player;
    public float interactionRange = 3f;
    public float viewAngle = 45f;

    [Header("Brewing Settings")]
    public float brewDuration = 2f;
    public float vibrationMagnitude = 0.05f;

    [Header("Progress Bar UI")]
    public Slider progressBar;
    public Image progressBorder; // Optional border or glow
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
        if (Input.GetKeyDown(KeyCode.E) && IsPlayerFacingMe() && !isBrewing)
        {
            StartBrewing();
        }

        if (isBrewing)
        {
            timer += Time.deltaTime;

            if (timer < brewDuration)
            {
                // Vibrate the coffee maker
                transform.localPosition = originalPosition + Random.insideUnitSphere * vibrationMagnitude;

                // Update progress bar
                if (progressBar != null)
                    progressBar.value = timer / brewDuration;
            }
            else
            {
                FinishBrewing();
            }
        }
    }

    bool IsPlayerFacingMe()
    {
        Vector3 toCoffeeMaker = (transform.position - player.position).normalized;
        float angle = Vector3.Angle(player.forward, toCoffeeMaker);
        float distance = Vector3.Distance(player.position, transform.position);
        return angle < viewAngle && distance < interactionRange;
    }

    void StartBrewing()
    {
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

        // Optional: hide progress bar after delay
        Invoke(nameof(HideProgressBar), 1.5f);
    }

    void HideProgressBar()
    {
        if (progressBar != null)
            progressBar.gameObject.SetActive(false);
    }
}
