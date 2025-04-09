using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Entities.Player;

public class FocusableObject : MonoBehaviour
{
    public bool IsFocused { get; private set; }


    private Transform _playerCameraTransform;
    private CameraShake _playerCameraShake;


    private Vector3 _originPosition;
    private Quaternion _originRotation;
    private Vector3 _originScale;

    private Vector3 _targetPosition;
    private Quaternion _targetRotation;
    private Vector3 _targetScale;

    private const float MOVEMENT_TIME = 0.3f;

    [SerializeField] private float _cameraOffsetDistance = 1.25f;
    [SerializeField] private float _cameraOffsetMultiplier = 0.25f; // Applied to scale & offset position so that our map is pushed into the player.
    [SerializeField] private AnimationCurve _movementAndScaleCurve = AnimationCurve.Linear(0.0f, 0.0f, 1.0f, 1.0f);
    private Coroutine _movementCoroutine;


    private Vector3 _currentPositionOffset;
    [SerializeField] private Vector3 _eulerRotationOffset = Vector3.zero;


    public float CameraOffsetMultiplier => _cameraOffsetMultiplier;
    public Vector3 TargetPosition => _targetPosition;


    private void Awake()
    {
        _originPosition = transform.position;
        _originRotation = transform.rotation;
        _originScale = transform.localScale;

        _currentPositionOffset = Vector3.zero;
    }
    private void Update()
    {
        if (!IsFocused)
            return;

        Vector3 targetWithOffset = _targetPosition - transform.TransformVector(_currentPositionOffset);
        const float APPROXIMATE_SQR_DISTANCE = 0.1f * 01f;
        if ((transform.position - targetWithOffset).sqrMagnitude < APPROXIMATE_SQR_DISTANCE)
        {
            transform.position = Vector3.Lerp(transform.position, targetWithOffset, 5.0f * Time.deltaTime);
        }
        else
        {
            transform.position = targetWithOffset;
        }
    }


    public void StartFocus()
    {
        // Start Movement to TargetPos.
        if (_movementCoroutine != null)
            StopCoroutine(_movementCoroutine);
        _movementCoroutine = StartCoroutine(MoveToTarget());
    }
    public void StopFocus()
    {
        // Reset Position, Rotation, & Scale.
        if (_movementCoroutine != null)
            StopCoroutine(_movementCoroutine);
        _movementCoroutine = StartCoroutine(MoveToOrigin());
    }


    private void SetTargetPosition()
    {
        _playerCameraTransform ??= PlayerManager.Instance.GetPlayerCamera().transform;
        _playerCameraShake ??= _playerCameraTransform.GetComponentInParent<CameraShake>();

        // Position.
        _targetPosition = _playerCameraShake.CameraOriginalPosition + (_playerCameraTransform.forward * _cameraOffsetDistance * _cameraOffsetMultiplier);

        // Rotation.
        _targetRotation = Quaternion.LookRotation(-_playerCameraTransform.forward) * Quaternion.Euler(_eulerRotationOffset);

        // Scale.
        _targetScale = _originScale * _cameraOffsetMultiplier;
    }

    private IEnumerator MoveToTarget()
    {
        SetTargetPosition();

        Vector3 targetPosition = _targetPosition;
        if (_currentPositionOffset != Vector3.zero)
        {
            // Equivalent of 'targetPosition - transform.TransformVector(_currentPositionOffset)' when the object is at the target rotation & scale.
            targetPosition = targetPosition - (_targetRotation * (new Vector3(_currentPositionOffset.x * _targetScale.x, _currentPositionOffset.y * _targetScale.y, _currentPositionOffset.z * _targetScale.z)));
        }

        float transitionTime = 0.0f;
        while (transitionTime <= 1.0f)
        {
            float t = _movementAndScaleCurve.Evaluate(transitionTime);

            // Alter Position, Rotation, and Scale.
            transform.position = Vector3.Lerp(_originPosition, targetPosition, t);
            transform.rotation = Quaternion.Lerp(_originRotation, _targetRotation, t);
            transform.localScale = Vector3.Lerp(_originScale, _targetScale, t);

            yield return null;
            transitionTime += Time.deltaTime / MOVEMENT_TIME;
        }

        // Ensure correct Position, Rotation, and Scale.
        transform.position = targetPosition;
        transform.rotation = _targetRotation;
        transform.localScale = _targetScale;


        IsFocused = true;
    }
    private IEnumerator MoveToOrigin()
    {
        IsFocused = false;
        _currentPositionOffset = Vector3.zero;

        // Ensure that our current target values are correct.
        Vector3 targetPosition = transform.position;
        Quaternion targetRotation = transform.rotation;
        Vector3 targetScale = transform.localScale;

        // Interpolate from our starting values to the original values.
        float transitionTime = 0.0f;
        while (transitionTime <= 1.0f)
        {
            float t = _movementAndScaleCurve.Evaluate(transitionTime);

            // Alter Position, Rotation, and Scale.
            transform.position = Vector3.Lerp(targetPosition, _originPosition, t);
            transform.rotation = Quaternion.Lerp(targetRotation, _originRotation, t);
            transform.localScale = Vector3.Lerp(targetScale, _originScale, t);

            yield return null;
            transitionTime += Time.deltaTime / MOVEMENT_TIME;
        }

        // Ensure correct Position, Rotation, and Scale.
        transform.position = _originPosition;
        transform.rotation = _originRotation;
        transform.localScale = _originScale;
    }


    public void SetPositionOffset(Vector3 newPositionOffset) => _currentPositionOffset = newPositionOffset;
    public Vector3 GetPositionOffset() => _currentPositionOffset;
}
