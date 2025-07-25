using UnityEngine;

public class Player : MonoBehaviour
{
    public static Transform transformPlayer;

    [SerializeField] private float moveSpeed;
    [SerializeField] private GameInput gameInput;

    void Start()
    {
        transformPlayer = this.transform;
    }

    void Update()
    {
        PlayerMovement();
    }

    private void PlayerMovement()
    {
        Vector2 InputVector = gameInput.GetMovementNormalized();
        Vector3 moveDir = new(InputVector.x, 0f , InputVector.y);

        float moveDistance = Time.deltaTime * moveSpeed;

        transform.position += moveDir * moveDistance;
    }
}
