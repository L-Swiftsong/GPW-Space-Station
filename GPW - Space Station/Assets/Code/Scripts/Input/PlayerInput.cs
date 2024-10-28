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
    public static event Action OnThrowFlarePerformed;

    public static event Action OnToggleFlashlightPerformed;
    public static event Action OnFocusFlashlightStarted;
    public static event Action OnFocusFlashlightCancelled;

    #endregion

    #region Values

    private static Vector2 s_movementInput;
    private static Vector2 s_lookInput;

    public static Vector2 MovementInput => s_movementInput;

    public static Vector2 LookInput => s_lookInput;
    public static float LookX => s_lookInput.x;
    public static float LookY => s_lookInput.y;

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
        _playerInput.Default.ThrowFlare.performed += ThrowFlare_performed;

        _playerInput.Default.ToggleFlashlight.performed += ToggleFlashlight_performed;
        _playerInput.Default.FocusFlashlight.started += FocusFlashlight_started;
        _playerInput.Default.FocusFlashlight.canceled += FocusFlashlight_cancelled;

        // Enable maps.
        _playerInput.Default.Enable();

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
        _playerInput.Default.ThrowFlare.performed -= ThrowFlare_performed;

        _playerInput.Default.ToggleFlashlight.performed -= ToggleFlashlight_performed;
        _playerInput.Default.FocusFlashlight.started -= FocusFlashlight_started;
        _playerInput.Default.FocusFlashlight.canceled -= FocusFlashlight_cancelled;


        // Dispose of the PlayerInputAction instance.
        _playerInput.Dispose();
    }


    private void Jump_performed(InputAction.CallbackContext obj) => OnJumpPerformed?.Invoke();

    private void Crouch_performed(InputAction.CallbackContext obj) => OnCrouchPerformed?.Invoke();
    private void Crouch_started(InputAction.CallbackContext obj) => OnCrouchStarted?.Invoke();
    private void Crouch_cancelled(InputAction.CallbackContext obj) => OnCrouchCancelled?.Invoke();

    private void Sprint_performed(InputAction.CallbackContext obj) => OnSprintPerformed?.Invoke();
    private void Sprint_started(InputAction.CallbackContext obj) => OnSprintStarted?.Invoke();
    private void Sprint_cancelled(InputAction.CallbackContext obj) => OnSprintCancelled?.Invoke();

    private void LeanLeft_started(InputAction.CallbackContext obj) => OnLeanLeftStarted?.Invoke();
    private void LeanLeft_cancelled(InputAction.CallbackContext obj) => OnLeanLeftCancelled?.Invoke();
    private void LeanRight_started(InputAction.CallbackContext obj) => OnLeanRightStarted?.Invoke();
    private void LeanRight_cancelled(InputAction.CallbackContext obj) => OnLeanRightCancelled?.Invoke();

    private void Interact_performed(InputAction.CallbackContext obj) => OnInteractPerformed?.Invoke();
    private void ThrowFlare_performed(InputAction.CallbackContext obj) => OnThrowFlarePerformed?.Invoke();

    private void ToggleFlashlight_performed(InputAction.CallbackContext obj) => OnToggleFlashlightPerformed?.Invoke();
    private void FocusFlashlight_started(InputAction.CallbackContext obj) => OnFocusFlashlightStarted?.Invoke();
    private void FocusFlashlight_cancelled(InputAction.CallbackContext obj) => OnFocusFlashlightCancelled?.Invoke();


    private void Update()
    {
        // Detect input.
        s_movementInput = _playerInput.Default.Movement.ReadValue<Vector2>();
        s_lookInput = _playerInput.Default.LookInput.ReadValue<Vector2>();
    }
}
