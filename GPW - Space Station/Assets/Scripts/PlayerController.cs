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
    [SerializeField] private float rotationSpeed = 1.0f;

    [Header("Jump and Gravity Settings")]
    [SerializeField] private float jumpHeight = 1.2f;
    [SerializeField] private float gravity = -15.0f;    
    [SerializeField] private float lowGravity = -5.0f;  
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

    [Header("Camera Settings")]
    [SerializeField] private GameObject camera;
    private float _rotationX = 0.0f;


    private float _speed;
    private CharacterController _controller;
    private bool _canJump = true;
    private bool _inLowGravityZone = false;

    private void Start()
    {
        // character controller and camera settings
        _controller = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        _cameraInitialPosition = camera.transform.localPosition;
        _cameraPeekPosition = _cameraInitialPosition;
    }

    private void Update()
    {
        GroundedCheck();            
        HandleMovement();           
        HandleJumpAndGravity();     
        HandleLook();               
        HandleCrouch();             
        HandleTilt();               
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
            : (Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : moveSpeed);

        Vector3 inputDirection = new Vector3(Input.GetAxis("Horizontal"), 0.0f, Input.GetAxis("Vertical")).normalized;

        if (inputDirection.magnitude >= 0.1f)
        {
            inputDirection = transform.right * inputDirection.x + transform.forward * inputDirection.z;
            _speed = targetSpeed;  
        }
        else
        {
            _speed = 0; 
        }

        _controller.Move(inputDirection * (_speed * Time.deltaTime) + Vector3.up * _verticalVelocity * Time.deltaTime);
    }


    
    /// <summary>
    /// Handles jumping and applies gravity based on the player's environment (normal or low-gravity).
    /// </summary>
    private void HandleJumpAndGravity()
    {
        float appliedGravity = _inLowGravityZone ? lowGravity : gravity;
        if (_grounded)
        {
            
            if (_verticalVelocity < 0.0f)
            {
                _verticalVelocity = -2f;
            }

         
            if (Input.GetButtonDown("Jump") && _canJump && !_isCrouching)
            {
                _verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * appliedGravity);
                _canJump = false;  
                StartCoroutine(ResetJump());
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

    /// <summary>
    /// Handles player look/rotation / Appllies camera tilt + // Clamping to void over-rotating
    /// </summary>
    private void HandleLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * rotationSpeed;
        float mouseY = Input.GetAxis("Mouse Y") * rotationSpeed;

        _rotationX -= mouseY;
        _rotationX = Mathf.Clamp(_rotationX, -90f, 90f);  

        camera.transform.localRotation = Quaternion.Euler(_rotationX, 0.0f, _currentTilt); 
        transform.Rotate(Vector3.up * mouseX);  
    }

    /// <summary>
    /// Handles crouch functionality, adjusting player height and movement speed.
    /// </summary>
    private void HandleCrouch()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            if (_isCrouching)
            {
                _controller.height = normalHeight;
                camera.transform.localPosition = _cameraInitialPosition;
                _isCrouching = false;
            }
            else
            {
                _controller.height = crouchHeight;
                camera.transform.localPosition = new Vector3(
                    _cameraInitialPosition.x,
                    _cameraInitialPosition.y - 0.3f,
                    _cameraInitialPosition.z
                );
                _isCrouching = true;
            }
        }
    }

    /// <summary>
    /// Handles camera tiltin when the player presses (Q or E).
    /// </summary>
    private void HandleTilt()
    {
        if (Input.GetKey(KeyCode.E))  // Lean right
        {
            _currentTilt = Mathf.Lerp(_currentTilt, -tiltAngle, Time.deltaTime * tiltSpeed);
            _cameraPeekPosition = new Vector3(
                _cameraInitialPosition.x + peekOffset,
                _cameraInitialPosition.y,
                _cameraInitialPosition.z
            );
        }
        else if (Input.GetKey(KeyCode.Q))  // Lean left
        {
            _currentTilt = Mathf.Lerp(_currentTilt, tiltAngle, Time.deltaTime * tiltSpeed);
            _cameraPeekPosition = new Vector3(
                _cameraInitialPosition.x - peekOffset,
                _cameraInitialPosition.y,
                _cameraInitialPosition.z
            );
        }
        else  // Return to original postion 
        {
            _currentTilt = Mathf.Lerp(_currentTilt, 0.0f, Time.deltaTime * tiltSpeed);
            _cameraPeekPosition = _cameraInitialPosition;
        }

        // Apply the tilt and peek to camera
        camera.transform.localRotation = Quaternion.Euler(_rotationX, 0.0f, _currentTilt);
        camera.transform.localPosition = Vector3.Lerp(
            camera.transform.localPosition,
            _cameraPeekPosition,
            Time.deltaTime * tiltSpeed
        );
    }

    /// <summary>
    /// Coroutine to reset the jump ability
    /// </summary>
    private IEnumerator ResetJump()
    {
        yield return new WaitForSeconds(jumpTimeout);
        _canJump = true;
    }

    /// <summary>
    /// for whebn the player enters a low-gravity zone.
    /// </summary>
    public void EnterLowGravityZone()
    {
        _inLowGravityZone = true;
    }

    /// <summary>
    /// for when the player exits a low-gravity zone.
    /// </summary>
    public void ExitLowGravityZone()
    {
        _inLowGravityZone = false;
    }
}
