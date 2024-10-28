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

    public static event Action OnThrowFlarePerformed;

    #endregion

    #region Values

    private Vector2 _movementInput;
    private Vector2 _lookInput;

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
        _playerInput.Default.ThrowFlare.performed += ThrowFlare_performed;

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
        _playerInput.Default.ThrowFlare.performed -= ThrowFlare_performed;


        // Dispose of the PlayerInputAction instance.
        _playerInput.Dispose();
    }


    private void ThrowFlare_performed(InputAction.CallbackContext obj) => OnThrowFlarePerformed?.Invoke();


    private void Update()
    {
        // Detect input.
        _movementInput = _playerInput.Default.Movement.ReadValue<Vector2>();
    }
}
