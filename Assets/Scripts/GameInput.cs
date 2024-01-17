using System;
using UnityEngine;

public class GameInput : MonoBehaviour
{
    public static GameInput Instance { get; private set; }

    public event EventHandler OnInteractAction;
    public event EventHandler OnAlternateInteractAction;
    public event EventHandler OnUseItemAction;
    public event EventHandler OnBackpackAction;
    public event EventHandler OnAttackAction;

    private PlayerInputAction playerInputActions;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        playerInputActions = new PlayerInputAction();
        playerInputActions.Player.Enable();

        playerInputActions.Player.Interact.performed += Interact_Performed; // interact "E" by default
        playerInputActions.Player.AlternateInteract.performed += AlternateInteract_Perform; // alternate interact "F" by default
        playerInputActions.Player.UseItem.performed += UseItem_Performed;
        playerInputActions.Player.Backpack.performed += Backpack_Performed;
        playerInputActions.Player.Attack.performed += Attack_Performed;
    }

    private void AlternateInteract_Perform(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        OnAlternateInteractAction?.Invoke(this, EventArgs.Empty);
    }

    private void Interact_Performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    // playerInputActions.Player.
    {
        OnInteractAction?.Invoke(this, EventArgs.Empty);
    }

    private void UseItem_Performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        OnUseItemAction?.Invoke(this, EventArgs.Empty);
    }

    private void Attack_Performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        OnAttackAction?.Invoke(this, EventArgs.Empty);
    }

    private void Backpack_Performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        OnBackpackAction?.Invoke(this, EventArgs.Empty);
    }

    public Vector2 GetMovementVectorNormalized()
    {
        Vector2 inputVector = playerInputActions.Player.Move.ReadValue<Vector2>();

        inputVector = inputVector.normalized;

        return inputVector;
    }

    public bool IsSprintButtonPressed()
    {
        return playerInputActions.Player.Sprint.IsPressed();
    }
    public bool IsMovementPressed()
    {
        return playerInputActions.Player.Move.IsPressed();
    }

    private void OnDestroy()
    {
        playerInputActions.Player.Interact.performed -= Interact_Performed;
        playerInputActions.Player.AlternateInteract.performed -= AlternateInteract_Perform;
        playerInputActions.Player.UseItem.performed -= UseItem_Performed;
    }
}
