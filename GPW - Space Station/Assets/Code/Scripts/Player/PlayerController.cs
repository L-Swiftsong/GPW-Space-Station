using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 
using TMPro; 
/*
 * CONTEXT:
 * 
 * This script handles the player's movement, jumping, gravity effects, crouching, tilting, and gravity. 
 */


public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 4.0f;
    [SerializeField] private float sprintSpeed = 6.0f;
    [SerializeField] private float speedChangeRate = 10.0f;

    [Space(5)]
    [SerializeField] private bool _toggleSprint = false;
    private bool _isSprinting = false;


    [Header("Jump and Gravity Settings")]
    [SerializeField] private float jumpHeight = 1.2f;
    [SerializeField] private float gravity = -15.0f;    
    [SerializeField] private float lowGravity = -2.0f;  
    [SerializeField] private float jumpTimeout = 0.1f;
    [SerializeField] private float fallTimeout = 0.15f;
    private float _verticalVelocity;
    private float _terminalVelocity = 53.0f;


    [Header("Grounded Settings")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundedRadius = 0.5f;
    [SerializeField] private LayerMask groundLayers;
    private bool _grounded = true; 


    [Header("Crouch Settings")]
    [SerializeField] private bool _toggleCrouch = false;

    [Space(5)]
    [SerializeField] private float crouchHeight = 1.0f;
    [SerializeField] private float normalHeight = 2.0f;
    [SerializeField] private float crouchSpeedMultiplier = 0.5f;
    private bool _isCrouching = false;


    [Header("Tilt Settings")]
    [SerializeField] private float tiltAngle = 15.0f;
    [SerializeField] private float tiltSpeed = 5.0f;
    [SerializeField] private float peekOffset = 0.3f;  
    private float _currentTilt = 0.0f;
    private Vector3 _cameraInitialPosition;
    private Vector3 _cameraPeekPosition;

    private Coroutine _tiltingCoroutine;
    private bool _tiltingLeft;


    [Header("Camera Settings")]
    [SerializeField] private GameObject camera;
    private float _rotationX = 0.0f;

    [SerializeField] private float _horizontalLookSensitivity = 100.0f;
    [SerializeField] private float _verticalLookSensitivity = 75.0f;


    private float _speed;
    private CharacterController _controller;
    private bool _canJump = true;
    private bool _inLowGravityZone = false;

    private bool isHiding = false;

    private void Start()
    {
        // character controller and camera settings
        _controller = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        _cameraInitialPosition = camera.transform.localPosition;
        _cameraPeekPosition = _cameraInitialPosition;
    }
    private void OnEnable()
    {
        // Subscribe to PlayerInput events.
        PlayerInput.OnJumpPerformed += PlayerInput_OnJumpPerformed;

        PlayerInput.OnCrouchPerformed += PlayerInput_OnCrouchPerformed;
        PlayerInput.OnCrouchStarted += PlayerInput_OnCrouchStarted;
        PlayerInput.OnCrouchCancelled += PlayerInput_OnCrouchCancelled;

        PlayerInput.OnSprintPerformed += PlayerInput_OnSprintPerformed;
        PlayerInput.OnSprintStarted += PlayerInput_OnSprintStarted;
        PlayerInput.OnSprintCancelled += PlayerInput_OnSprintCancelled;

        PlayerInput.OnLeanLeftStarted += PlayerInput_OnLeanLeftStarted;
        PlayerInput.OnLeanLeftCancelled += PlayerInput_OnLeanLeftCancelled;
        PlayerInput.OnLeanRightStarted += PlayerInput_OnLeanRightStarted;
        PlayerInput.OnLeanRightCancelled += PlayerInput_OnLeanRightCancelled;
    }
    private void OnDisable()
    {
        // Unsubscribe from PlayerInput events.
        PlayerInput.OnJumpPerformed -= PlayerInput_OnJumpPerformed;

        PlayerInput.OnCrouchPerformed -= PlayerInput_OnCrouchPerformed;
        PlayerInput.OnCrouchStarted -= PlayerInput_OnCrouchStarted;
        PlayerInput.OnCrouchCancelled -= PlayerInput_OnCrouchCancelled;

        PlayerInput.OnSprintPerformed -= PlayerInput_OnSprintPerformed;
        PlayerInput.OnSprintStarted -= PlayerInput_OnSprintStarted;
        PlayerInput.OnSprintCancelled -= PlayerInput_OnSprintCancelled;

        PlayerInput.OnLeanLeftStarted -= PlayerInput_OnLeanLeftStarted;
        PlayerInput.OnLeanLeftCancelled -= PlayerInput_OnLeanLeftCancelled;
        PlayerInput.OnLeanRightStarted -= PlayerInput_OnLeanRightStarted;
        PlayerInput.OnLeanRightCancelled -= PlayerInput_OnLeanRightCancelled;
    }


    #region Input Functions

    private void PlayerInput_OnJumpPerformed() => PerformJump();

    private void PlayerInput_OnCrouchPerformed()
    {
        if (!_toggleCrouch)
        {
            // We aren't using toggle crouch.
            return;
        }

        if (_isCrouching)
        {
            // We are currently crouching. Stop crouching.
            StopCrouching();
        }
        else
        {
            // We are currently not crouching. Start crouching.
            StartCrouching();
        }
    }
    private void PlayerInput_OnCrouchStarted()
    {
        if (!_toggleCrouch)
        {
            // Toggle Crouch is disabled.
            // Start crouching now the button is pressed.
            StartCrouching();
        }
    }
    private void PlayerInput_OnCrouchCancelled()
    {
        if (!_toggleCrouch)
        {
            // Toggle Crouch is disabled.
            // Stop crouching now the button is released.
            StopCrouching();
        }
    }

    private void PlayerInput_OnSprintPerformed()
    {
        if (_toggleSprint)
        {
            _isSprinting = !_isSprinting;
        }
    }
    private void PlayerInput_OnSprintStarted()
    {
        if (!_toggleSprint)
        {
            _isSprinting = true;
        }
    }
    private void PlayerInput_OnSprintCancelled()
    {
        if (!_toggleSprint)
        {
            _isSprinting = false;
        }
    }

    private void PlayerInput_OnLeanLeftStarted() => StartTilting(tiltLeft: true);
    private void PlayerInput_OnLeanLeftCancelled()
    {
        if (_tiltingLeft)
        {
            // We were leaning to the left. Stop leaning.
            StopTilting();
        }
    }

    private void PlayerInput_OnLeanRightStarted() => StartTilting(tiltLeft: false);
    private void PlayerInput_OnLeanRightCancelled()
    {
        if (!_tiltingLeft)
        {
            // We were leaning to the right. Stop leaning.
            StopTilting();
        }
    }


    #endregion


    private void Update()
    {
        GroundedCheck();

        if (!isHiding)
        {
            HandleMovement();
            HandleSprintCheck();
            HandleGravity();
            HandleTilt();
        }
        
        HandleLook();             
    }

    private void GroundedCheck()
    {
        _grounded = Physics.CheckSphere(groundCheck.position, groundedRadius, groundLayers, QueryTriggerInteraction.Ignore);
    }

    /// <summary>
    /// Handles player movement, sprinting, and crouch speed adjustment.
    /// </summary>
    private void HandleMovement()
    {
        float targetSpeed = _isCrouching
            ? moveSpeed * crouchSpeedMultiplier
            : (_isSprinting ? sprintSpeed : moveSpeed);

        Vector2 movementInput = PlayerInput.MovementInput;
        Vector3 inputDirection = transform.right * movementInput.x + transform.forward * movementInput.y;

        _speed = inputDirection.magnitude >= 0.1f ? targetSpeed : 0.0f;
        

        _controller.Move(inputDirection * (_speed * Time.deltaTime) + Vector3.up * _verticalVelocity * Time.deltaTime);
    }

    /// <summary>
    /// Handles player look/rotation using clamping to avoid over-rotating. Also appllies camera tilt.
    /// </summary>
    private void HandleLook()
    {
        float lookX = PlayerInput.LookX * _horizontalLookSensitivity * Time.deltaTime;
        float lookY = PlayerInput.LookY * _verticalLookSensitivity * Time.deltaTime;

        _rotationX -= lookY;
        _rotationX = Mathf.Clamp(_rotationX, -90f, 90f);

        camera.transform.localRotation = Quaternion.Euler(_rotationX, 0.0f, _currentTilt);
        transform.Rotate(Vector3.up * lookX);
    }


    private void HandleSprintCheck()
    {
        if (!_toggleSprint || !_isSprinting)
        {
            // We aren't using Toggle Sprint (Or aren't sprinting), so this function shouldn't be run.
            return;
        }

        if (_speed == 0.0f)
        {
            // We aren't moving. Stop Sprinting
            _isSprinting = false;
        }
    }


    #region Jumping

    /// <summary> Handles jumping.</summary>
    private void PerformJump()
    {
        if (!_grounded && _canJump && !_isCrouching)
        {
            // We cannot jump.
            return;
        }

        
        float appliedGravity = _inLowGravityZone ? lowGravity : gravity;
        _verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * appliedGravity);
        _canJump = false;

        StartCoroutine(ResetJump());
    }

    /// <summary> Coroutine to reset the jump ability.</summary>
    private IEnumerator ResetJump()
    {
        yield return new WaitForSeconds(jumpTimeout);
        _canJump = true;
    }

    #endregion


    #region Crouching

    /// <summary> Start Crouching. Adjusts player height and movement speed.</summary>
    private void StartCrouching()
    {
        _controller.height = crouchHeight;
        camera.transform.localPosition = new Vector3(
            _cameraInitialPosition.x,
            _cameraInitialPosition.y - 0.3f,
            _cameraInitialPosition.z
        );

        _isCrouching = true;
    }
    /// <summary> Stop Crouching. Adjusts player height and movement speed.</summary>
    private void StopCrouching()
    {
        _controller.height = normalHeight;
        camera.transform.localPosition = _cameraInitialPosition;

        _isCrouching = false;
    }

    #endregion


    #region Tilting

    /// <summary>
    /// Handles the camera's current tilt.
    /// </summary>
    private void HandleTilt()
    {
        // Apply the tilt and peek to camera
        camera.transform.localRotation = Quaternion.Euler(_rotationX, 0.0f, _currentTilt);
        camera.transform.localPosition = Vector3.Lerp(
            camera.transform.localPosition,
            _cameraPeekPosition,
            Time.deltaTime * tiltSpeed
        );
    }

    /// <summary> Starts the camera tilting in the desired direction.</summary>
    private void StartTilting(bool tiltLeft)
    {
        // Start tilting towards our desired tilt angle.
        if (_tiltingCoroutine != null)
        {
            StopCoroutine(_tiltingCoroutine);
        }
        _tiltingCoroutine = StartCoroutine(LerpTilt(tiltLeft ? tiltAngle : -tiltAngle));

        _tiltingLeft = tiltLeft;


        // Set the camera's position to the desired lean position.
        _cameraPeekPosition = new Vector3(
            _cameraInitialPosition.x + (tiltLeft ? -peekOffset : peekOffset),
            _cameraInitialPosition.y,
            _cameraInitialPosition.z
        );
    }
    /// <summary> Reverts the camera tilt so that it is no longer tilting.</summary>
    private void StopTilting()
    {
        // Return our tilting towards 0.0f.
        if (_tiltingCoroutine != null)
        {
            StopCoroutine(_tiltingCoroutine);
        }
        _tiltingCoroutine = StartCoroutine(LerpTilt(0.0f));

        _tiltingLeft = false;

        // Reset the camera's position.
        _cameraPeekPosition = _cameraInitialPosition;
    }

    /// <summary> Lerp the value of '_currentTilt' towards the value of 'targetTilt'.</summary>
    private IEnumerator LerpTilt(float targetTilt)
    {
        // Loop until we have reached our desired tilt.
        while(_currentTilt != targetTilt)
        {
            // Lerp the current tilt towards the target tilt.
            _currentTilt = Mathf.Lerp(_currentTilt, targetTilt, tiltSpeed * Time.deltaTime);
            yield return null;
        }
    }

    #endregion


    #region Gravity

    /// <summary> Applies gravity based on the player's environment (normal or low-gravity).</summary>
    private void HandleGravity()
    {
        float appliedGravity = _inLowGravityZone ? lowGravity : gravity;
        if (_grounded)
        {
            if (_verticalVelocity < 0.0f)
            {
                _verticalVelocity = -2f;
            }
        }
        else
        {
            if (_verticalVelocity < _terminalVelocity)
            {
                _verticalVelocity += appliedGravity * Time.deltaTime;
            }
        }
    }

    /// <summary> For when the player enters a low-gravity zone.</summary>
    public void EnterLowGravityZone()
    {
        _inLowGravityZone = true;
    }

    /// <summary> For when the player exits a low-gravity zone.</summary>
    public void ExitLowGravityZone()
    {
        _inLowGravityZone = false;
    }

    #endregion


    public void SetHiding(bool hiding)
    {
        isHiding = hiding;
    }
}
