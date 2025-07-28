using UnityEngine;

public class GameInput : MonoBehaviour
{
    private PlayerInputSystem PlayerInputSystem;


    private void Awake()
    {
        PlayerInputSystem = new PlayerInputSystem();

        PlayerInputSystem.Player.Enable();
    }

    private void OnDisable()
    {
        PlayerInputSystem.Player.Disable();
    }
    public Vector2 GetMovementNormalized()
    {
        Vector2 InputVector = PlayerInputSystem.Player.Move.ReadValue<Vector2>();

        InputVector = InputVector.normalized;

        return InputVector;
    }
}
