using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInput : MonoBehaviour
{
    #region Singleton

    private static PlayerInput _instance;
    public static PlayerInput Instance
    {
        get => _instance;
        set
        {
            if (_instance != null)
            {
                Debug.LogError("Error: A PlayerInput instance already exists within the scene: " + _instance.name + ". Destroying " + value.name);
                Destroy(value.gameObject);
                return;
            }

            _instance = value;
        }
    }

    #endregion


    private PlayerInputActions _playerInput;


    #region Events

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

    #region Values

    private static Vector2 s_movementInput;
    private static Vector2 s_lookInput;

    private static Vector2 s_gamepadInventorySelect;

    public static Vector2 MovementInput => s_movementInput;

    public static Vector2 LookInput => s_lookInput;
    public static float LookX => s_lookInput.x;
    public static float LookY => s_lookInput.y;

    public static Vector2 GamepadInventorySelect => s_gamepadInventorySelect;

    #endregion


    private void Awake()
    {
        _instance = this;

        CreateInputActions();
    }
    private void OnEnable()
    {
        if (_playerInput == null)
        {
            CreateInputActions();
        }
    }
    private void OnDisable()
    {
        DestroyInputActions();
    }
    private void OnDestroy()
    {
        DestroyInputActions();
    }


    private void CreateInputActions()
    {
        // Create the InputActionMap.
        _playerInput = new PlayerInputActions();


        // Subscribe to events.
        _playerInput.Default.Jump.performed += Jump_performed;


        _playerInput.Default.Crouch.performed += Crouch_performed;
        _playerInput.Default.Crouch.started += Crouch_started;
        _playerInput.Default.Crouch.canceled += Crouch_cancelled;


        _playerInput.Default.Sprint.performed += Sprint_performed;
        _playerInput.Default.Sprint.started += Sprint_started;
        _playerInput.Default.Sprint.canceled += Sprint_cancelled;


        _playerInput.Default.LeanLeft.started += LeanLeft_started;
        _playerInput.Default.LeanLeft.canceled += LeanLeft_cancelled;
        _playerInput.Default.LeanRight.started += LeanRight_started;
        _playerInput.Default.LeanRight.canceled += LeanRight_cancelled;


        _playerInput.Default.Interact.performed += Interact_performed;


        _playerInput.Default.UseItem.started += UseItem_started;
        _playerInput.Default.UseItem.canceled += UseItem_cancelled;

        _playerInput.Default.AltUseItem.started += AltUseItem_started;
        _playerInput.Default.AltUseItem.canceled += AltUseItem_cancelled;


        // Subscribe to events (Inventory).
        _playerInput.Inventory.OpenInventory.performed += OpenInventory_Performed;
        _playerInput.Inventory.OpenInventory.started += OpenInventory_Started;
        _playerInput.Inventory.OpenInventory.canceled += OpenInventory_Cancelled;


        // Enable maps.
        _playerInput.Default.Enable();
        _playerInput.Inventory.Enable();

        _playerInput.Enable();
    }
    private void DestroyInputActions()
    {
        // Ensure that a playerInput instance exists.
        if (_playerInput == null)
        {
            return;
        }

        // Unsubscribe from events.
        _playerInput.Default.Jump.performed -= Jump_performed;


        _playerInput.Default.Crouch.performed -= Crouch_performed;
        _playerInput.Default.Crouch.started -= Crouch_started;
        _playerInput.Default.Crouch.canceled -= Crouch_cancelled;


        _playerInput.Default.Sprint.performed -= Sprint_performed;
        _playerInput.Default.Sprint.started -= Sprint_started;
        _playerInput.Default.Sprint.canceled -= Sprint_cancelled;


        _playerInput.Default.LeanLeft.started -= LeanLeft_started;
        _playerInput.Default.LeanLeft.canceled -= LeanLeft_cancelled;
        _playerInput.Default.LeanRight.started -= LeanRight_started;
        _playerInput.Default.LeanRight.canceled -= LeanRight_cancelled;


        _playerInput.Default.Interact.performed -= Interact_performed;


        _playerInput.Default.UseItem.started -= UseItem_started;
        _playerInput.Default.UseItem.canceled -= UseItem_cancelled;

        _playerInput.Default.AltUseItem.started -= AltUseItem_started;
        _playerInput.Default.AltUseItem.canceled -= AltUseItem_cancelled;


        // Unsubscrive from events (Inventory).
        _playerInput.Inventory.OpenInventory.performed -= OpenInventory_Performed;
        _playerInput.Inventory.OpenInventory.started -= OpenInventory_Started;
        _playerInput.Inventory.OpenInventory.canceled -= OpenInventory_Cancelled;


        // Dispose of the PlayerInputAction instance.
        _playerInput.Dispose();
    }


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


    private void Update()
    {
        // Detect input.
        s_movementInput = _playerInput.Default.Movement.ReadValue<Vector2>();
        s_lookInput = _playerInput.Default.LookInput.ReadValue<Vector2>();

        s_gamepadInventorySelect = _playerInput.Inventory.GamepadInventorySelect.ReadValue<Vector2>();
    }
}
