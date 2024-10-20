using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class BasicPlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float _moveSpeed;
    private Vector2 _movementInput;

    private CharacterController _characterController;
    private Vector3 _velocity;


    [Header("Gravity & Ground Check")]
    [SerializeField] private float _gravity = -9.81f;

    [Space(5)]
    [SerializeField] private Transform _groundCheck;
    [SerializeField] private float _groundDistance = 0.4f;
    [SerializeField] private LayerMask _groundMask;
    private bool _isGrounded;


    [Header("Camera")]
    [SerializeField] private Transform _rotationPivot;
    [SerializeField] private Camera _camera;

    private Vector2 _lookInput;
    private float _xRotation;


    [Space(5)]
    [SerializeField] private float _xSensitivity;
    [SerializeField] private float _ySensitivity;



    private void Awake()
    {
        _characterController = GetComponent<CharacterController>();

        Cursor.lockState = CursorLockMode.Locked;
    }


    private void Update()
    {
        GetInput();
        _isGrounded = Physics.CheckSphere(_groundCheck.position, _groundDistance, _groundMask);

        HandleMovement();
        HandleCamera();
        HandleGravity();
    }

    private void GetInput()
    {
        _movementInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;
        _lookInput = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));
    }


    private void HandleMovement()
    {
        Vector3 moveDirection = _rotationPivot.right * _movementInput.x + _rotationPivot.forward * _movementInput.y;
        _characterController.Move(moveDirection * _moveSpeed * Time.deltaTime);
    }

    private void HandleCamera()
    {
        // Rotate the Camera (Vertical)
        _xRotation -= _lookInput.y * _ySensitivity * Time.deltaTime;
        _xRotation = Mathf.Clamp(_xRotation, -90.0f, 90.0f);

        _camera.transform.localRotation = Quaternion.Euler(_xRotation, 0.0f, 0.0f);


        // Rotate the Rotation Pivot (Horizontal).
        _rotationPivot.Rotate(Vector3.up * _lookInput.x * _xSensitivity * Time.deltaTime);
    }

    private void HandleGravity()
    {
        _velocity.y += _gravity * Time.deltaTime;

        if (_isGrounded && _velocity.y < 0.0f)
        {
            _velocity.y = -2.0f;
        }


        _characterController.Move(_velocity * Time.deltaTime);
    }
}
