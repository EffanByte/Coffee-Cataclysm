using UnityEngine;

public class CoffeeMakerVibration : MonoBehaviour
{
    public Transform player; // Drag the Player's transform here
    public float interactionRange = 3f;
    public float viewAngle = 45f; // Half-angle of the cone in degrees
    public float vibrationDuration = 2f;
    public float vibrationMagnitude = 0.05f;

    private Vector3 originalPosition;
    private float timer;
    private bool isVibrating = false;

    void Start()
    {
        originalPosition = transform.localPosition;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && IsPlayerFacingMe())
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

    bool IsPlayerFacingMe()
    {
        Vector3 toCoffeeMaker = (transform.position - player.position).normalized;

        // Angle between player's forward direction and direction to this object
        float angle = Vector3.Angle(player.forward, toCoffeeMaker);

        // Distance between player and coffee maker
        float distance = Vector3.Distance(player.position, transform.position);

        return angle < viewAngle && distance < interactionRange;
    }

    public void StartVibration()
    {
        isVibrating = true;
        timer = 0;
        originalPosition = transform.localPosition;
    }
}
