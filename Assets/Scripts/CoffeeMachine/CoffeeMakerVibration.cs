using UnityEngine;

public class CoffeeMakerVibration : MonoBehaviour
{
    public float vibrationDuration = 2f;  // How long to shake
    public float vibrationMagnitude = 0.05f; // How far to move each frame

    private Vector3 originalPosition;
    private float timer;
    private bool isVibrating = false;

    void Start()
    {
        originalPosition = transform.localPosition;
    }

    void Update()
    {
        // Simulate interaction (e.g., press E when nearby)
        if (Input.GetKeyDown(KeyCode.E)) // You can change to OnTrigger logic
        {
            StartVibration();
        }

        if (isVibrating)
        {
            timer += Time.deltaTime;

            if (timer < vibrationDuration)
            {
                transform.localPosition = originalPosition + Random.insideUnitSphere * vibrationMagnitude;
            }
            else
            {
                isVibrating = false;
                transform.localPosition = originalPosition;
                timer = 0;
            }
        }
    }

    public void StartVibration()
    {
        isVibrating = true;
        timer = 0;
        originalPosition = transform.localPosition;
    }
}
