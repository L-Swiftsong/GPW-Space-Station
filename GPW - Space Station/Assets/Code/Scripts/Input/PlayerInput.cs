using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInput : MonoBehaviour
{
    private static PlayerInputActions s_playerInput;


    #region Input Events

    public static event Action OnPauseGamePerformed;


    public static event Action OnSelectNextTabPerformed;
    public static event Action OnSelectPreviousTabPerformed;


    public static event Action OnJumpPerformed;

    public static event Action OnCrouchPerformed;
    public static event Action OnCrouchStarted;
    public static event Action OnCrouchCancelled;

    public static event Action OnSprintPerformed;
    public static event Action OnSprintStarted;
    public static event Action OnSprintCancelled;


    public static event Action OnLeanLeftStarted;
    public static event Action OnLeanLeftCancelled;

    public static event Action OnLeanRightStarted;
    public static event Action OnLeanRightCancelled;


    public static event Action OnInteractPerformed;


    public static event Action OnUseItemStarted;
    public static event Action OnUseItemCancelled;

    public static event Action OnAltUseItemStarted;
    public static event Action OnAltUseItemCancelled;


    public static event Action OnOpenInventoryPerformed;
    public static event Action OnOpenInventoryStarted;
    public static event Action OnOpenInventoryCancelled;

    #endregion

    #region Input Values

    // Movement Input.
    private static Vector2 s_movementInput;
    public static Vector2 MovementInput => s_movementInput;


    // Look Input.
    private static Vector2 s_lookInput;
    public static Vector2 LookInput => s_lookInput;
    public static float LookX => s_lookInput.x;
    public static float LookY => s_lookInput.y;


    // Gamepad Select.
    private static Vector2 s_gamepadInventorySelect;
    public static Vector2 GamepadInventorySelect => s_gamepadInventorySelect;

    #endregion


    private void Awake() => CreateInputActions();
    
    private void OnEnable()
    {
        if (s_playerInput == null)
        {
            CreateInputActions();
        }
    }
    private void OnDisable() => DestroyInputActions();
    private void OnDestroy() => DestroyInputActions();
    


    private void CreateInputActions()
    {
        // Create the InputActionMap.
        s_playerInput = new PlayerInputActions();


        // Subscribe to events (Global).
        s_playerInput.Global.PauseGame.performed += PauseGame_performed;


        // Subscribe to events (Menu).
        s_playerInput.Menu.SelectNextTab.performed += SelectNextTab_performed;
        s_playerInput.Menu.SelectPreviousTab.performed += SelectPreviousTab_performed;


        // Subscribe to events (Movement).
        s_playerInput.Movement.Jump.performed += Jump_performed;

        s_playerInput.Movement.Crouch.performed += Crouch_performed;
        s_playerInput.Movement.Crouch.started += Crouch_started;
        s_playerInput.Movement.Crouch.canceled += Crouch_cancelled;

        s_playerInput.Movement.Sprint.performed += Sprint_performed;
        s_playerInput.Movement.Sprint.started += Sprint_started;
        s_playerInput.Movement.Sprint.canceled += Sprint_cancelled;

        s_playerInput.Movement.LeanLeft.started += LeanLeft_started;
        s_playerInput.Movement.LeanLeft.canceled += LeanLeft_cancelled;
        s_playerInput.Movement.LeanRight.started += LeanRight_started;
        s_playerInput.Movement.LeanRight.canceled += LeanRight_cancelled;


        // Subscribe to events (Interaction).
        s_playerInput.Interaction.Interact.performed += Interact_performed;

        s_playerInput.Interaction.UseItem.started += UseItem_started;
        s_playerInput.Interaction.UseItem.canceled += UseItem_cancelled;

        s_playerInput.Interaction.AltUseItem.started += AltUseItem_started;
        s_playerInput.Interaction.AltUseItem.canceled += AltUseItem_cancelled;


        // Subscribe to events (Inventory).
        s_playerInput.Inventory.OpenInventory.performed += OpenInventory_Performed;
        s_playerInput.Inventory.OpenInventory.started += OpenInventory_Started;
        s_playerInput.Inventory.OpenInventory.canceled += OpenInventory_Cancelled;


        // Enable maps.
        s_playerInput.Global.Enable();
        s_playerInput.Menu.Enable();
        s_playerInput.Movement.Enable();
        s_playerInput.Camera.Enable();
        s_playerInput.Interaction.Enable();
        s_playerInput.Inventory.Enable();

        s_playerInput.Enable();

        // Disable maps that have elements wishing them disabled.
        UpdateDisabledState();
    }
    private void DestroyInputActions()
    {
        // Ensure that a playerInput instance exists.
        if (s_playerInput == null)
        {
            return;
        }


        // Unsubscribe from events (Global).
        s_playerInput.Global.PauseGame.performed -= PauseGame_performed;


        // Unsubscribe from events (Menu).
        s_playerInput.Menu.SelectNextTab.performed += SelectNextTab_performed;
        s_playerInput.Menu.SelectPreviousTab.performed += SelectPreviousTab_performed;


        // Unsubscribe from events (Movement).
        s_playerInput.Movement.Jump.performed -= Jump_performed;

        s_playerInput.Movement.Crouch.performed -= Crouch_performed;
        s_playerInput.Movement.Crouch.started -= Crouch_started;
        s_playerInput.Movement.Crouch.canceled -= Crouch_cancelled;

        s_playerInput.Movement.Sprint.performed -= Sprint_performed;
        s_playerInput.Movement.Sprint.started -= Sprint_started;
        s_playerInput.Movement.Sprint.canceled -= Sprint_cancelled;

        s_playerInput.Movement.LeanLeft.started -= LeanLeft_started;
        s_playerInput.Movement.LeanLeft.canceled -= LeanLeft_cancelled;
        s_playerInput.Movement.LeanRight.started -= LeanRight_started;
        s_playerInput.Movement.LeanRight.canceled -= LeanRight_cancelled;


        // Unsubscribe from events (Interaction).
        s_playerInput.Interaction.Interact.performed -= Interact_performed;

        s_playerInput.Interaction.UseItem.started -= UseItem_started;
        s_playerInput.Interaction.UseItem.canceled -= UseItem_cancelled;

        s_playerInput.Interaction.AltUseItem.started -= AltUseItem_started;
        s_playerInput.Interaction.AltUseItem.canceled -= AltUseItem_cancelled;


        // Unsubscrive from events (Inventory).
        s_playerInput.Inventory.OpenInventory.performed -= OpenInventory_Performed;
        s_playerInput.Inventory.OpenInventory.started -= OpenInventory_Started;
        s_playerInput.Inventory.OpenInventory.canceled -= OpenInventory_Cancelled;


        // Dispose of the PlayerInputAction instance.
        s_playerInput.Dispose();
    }


    #region Input Functions

    private void PauseGame_performed(InputAction.CallbackContext context) => OnPauseGamePerformed?.Invoke();


    private void SelectNextTab_performed(InputAction.CallbackContext context) => OnSelectNextTabPerformed?.Invoke();
    private void SelectPreviousTab_performed(InputAction.CallbackContext context) => OnSelectPreviousTabPerformed?.Invoke();


    private void Jump_performed(InputAction.CallbackContext context) => OnJumpPerformed?.Invoke();


    private void Crouch_performed(InputAction.CallbackContext context) => OnCrouchPerformed?.Invoke();
    private void Crouch_started(InputAction.CallbackContext context) => OnCrouchStarted?.Invoke();
    private void Crouch_cancelled(InputAction.CallbackContext context) => OnCrouchCancelled?.Invoke();


    private void Sprint_performed(InputAction.CallbackContext context) => OnSprintPerformed?.Invoke();
    private void Sprint_started(InputAction.CallbackContext context) => OnSprintStarted?.Invoke();
    private void Sprint_cancelled(InputAction.CallbackContext context) => OnSprintCancelled?.Invoke();


    private void LeanLeft_started(InputAction.CallbackContext context) => OnLeanLeftStarted?.Invoke();
    private void LeanLeft_cancelled(InputAction.CallbackContext context) => OnLeanLeftCancelled?.Invoke();
    private void LeanRight_started(InputAction.CallbackContext context) => OnLeanRightStarted?.Invoke();
    private void LeanRight_cancelled(InputAction.CallbackContext context) => OnLeanRightCancelled?.Invoke();


    private void Interact_performed(InputAction.CallbackContext context) => OnInteractPerformed?.Invoke();


    private void UseItem_started(InputAction.CallbackContext context) => OnUseItemStarted?.Invoke();
    private void UseItem_cancelled(InputAction.CallbackContext context) => OnUseItemCancelled?.Invoke();

    private void AltUseItem_started(InputAction.CallbackContext context) => OnAltUseItemStarted?.Invoke();
    private void AltUseItem_cancelled(InputAction.CallbackContext context) => OnAltUseItemCancelled?.Invoke();


    private void OpenInventory_Performed(InputAction.CallbackContext context) => OnOpenInventoryPerformed?.Invoke();
    private void OpenInventory_Started(InputAction.CallbackContext context) => OnOpenInventoryStarted?.Invoke();
    private void OpenInventory_Cancelled(InputAction.CallbackContext context) => OnOpenInventoryCancelled?.Invoke();

    #endregion


    private void Update()
    {
        // Detect input.
        s_movementInput = s_playerInput.Movement.Movement.ReadValue<Vector2>();
        s_lookInput = s_playerInput.Camera.LookInput.ReadValue<Vector2>();

        s_gamepadInventorySelect = s_playerInput.Inventory.GamepadInventorySelect.ReadValue<Vector2>();
    }



#region Action Prevention

    // Movement Map.
    private static int s_movementPreventionCount = 0;
    public static void PreventMovementActions()
    {
        s_movementPreventionCount++;

        if (s_playerInput != null)
        {
            // Disable our 'Movement' map.
            s_playerInput.Movement.Disable();
        }
    }
    public static void RemoveMovementActionPrevention()
    {
        s_movementPreventionCount--;

        if (s_movementPreventionCount == 0 && s_playerInput != null)
        {
            // There is no longer anything wishing to disable our movement controls.
            // Enable the 'Movement' map.
            s_playerInput.Movement.Enable();
        }
    }


    // Camera Map.
    private static int s_cameraPreventionCount = 0;
    public static void PreventCameraActions()
    {
        s_cameraPreventionCount++;

        if (s_playerInput != null)
        {
            // Disable our 'Camera' map.
            s_playerInput.Camera.Disable();
        }
    }
    public static void RemoveCameraActionPrevention()
    {
        s_cameraPreventionCount--;

        if (s_cameraPreventionCount == 0 && s_playerInput != null)
        {
            // There is no longer anything wishing to disable our camera controls.
            // Enable the 'Camera' map.
            s_playerInput.Camera.Enable();
        }
    }


    // Interaction Map.
    private static int s_interactionPreventionCount = 0;
    public static void PreventInteractionActions()
    {
        s_interactionPreventionCount++;

        if (s_playerInput != null)
        {
            // Disable our 'Interaction' map.
            s_playerInput.Interaction.Disable();
        }
    }
    public static void RemoveInteractionActionPrevention()
    {
        s_interactionPreventionCount--;

        if (s_interactionPreventionCount == 0 && s_playerInput != null)
        {
            // There is no longer anything wishing to disable our interaction controls.
            // Enable the 'Interaction' map.
            s_playerInput.Interaction.Enable();
        }
    }


    // Inventory Map.
    private static int s_inventoryPreventionCount = 0;
    public static void PreventInventoryActions()
    {
        s_inventoryPreventionCount++;

        if (s_playerInput != null)
        {
            // Disable our 'Inventory' map.
            s_playerInput.Inventory.Disable();
        }
    }
    public static void RemoveInventoryActionPrevention()
    {
        s_inventoryPreventionCount--;

        if (s_inventoryPreventionCount == 0 && s_playerInput != null)
        {
            // There is no longer anything wishing to disable our inventory controls.
            // Enable the 'Inventory' map.
            s_playerInput.Inventory.Enable();
        }
    }


    // All Maps.
    public static void PreventAllActions()
    {
        PreventMovementActions();
        PreventCameraActions();
        PreventInteractionActions();
        PreventInventoryActions();
    }
    public static void RemoveAllActionPrevention()
    {
        RemoveMovementActionPrevention();
        RemoveCameraActionPrevention();
        RemoveInteractionActionPrevention();
        RemoveInventoryActionPrevention();
    }

    private static void UpdateDisabledState()
    {
        // Movement.
        if (s_movementPreventionCount > 0)
            s_playerInput.Movement.Disable();
        else
            s_playerInput.Movement.Enable();

        // Camera.
        if (s_cameraPreventionCount > 0)
            s_playerInput.Camera.Disable();
        else
            s_playerInput.Camera.Enable();

        // Interaction.
        if (s_interactionPreventionCount > 0)
            s_playerInput.Interaction.Disable();
        else
            s_playerInput.Interaction.Enable();

        // Inventory.
        if (s_inventoryPreventionCount > 0)
            s_playerInput.Inventory.Disable();
        else
            s_playerInput.Inventory.Enable();
    }

    #endregion
}
