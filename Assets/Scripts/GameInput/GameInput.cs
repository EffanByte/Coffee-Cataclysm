using System;
using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif
public class GameInput : MonoBehaviour
{
    private PlayerInputSystem PlayerInputSystem;
    public event EventHandler OnInteractPlayer;
    public event EventHandler OnAttackPlayer;


    private void Awake()
    {
        PlayerInputSystem = new PlayerInputSystem();
        PlayerInputSystem.Player.Enable();
        PlayerInputSystem.Player.Interact.performed += OnInteractPerformed;
        PlayerInputSystem.Player.Attack.performed += Attack_performed;
    }

    private void Attack_performed(InputAction.CallbackContext obj)
    {
        OnAttackPlayer?.Invoke(this,EventArgs.Empty);
    }

    private void OnDisable()
    {
        if (PlayerInputSystem == null)
        {
            Debug.LogError("PlayerInputSystem or Player is null in OnDisable!");
            return;
        }

        PlayerInputSystem.Player.Disable();
    }
    

    public Vector2 GetMovementNormalized()
    {
        Debug.Log("PlayerMoving");
        Vector2 InputVector = PlayerInputSystem.Player.Move.ReadValue<Vector2>();
        return InputVector.normalized;
    }
#if ENABLE_INPUT_SYSTEM
    public void OnInteractPerformed(InputAction.CallbackContext value)
    {
        Debug.Log("Interact performed");
        OnInteractPlayer?.Invoke(this, EventArgs.Empty);
    }
#endif 
}
