using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.FilePathAttribute;

// Temp.
public class BreakerPuzzleRotator : MonoBehaviour
{
    [SerializeField] private Vector3 _rotationOn = new Vector3(-60.0f, 0.0f, 0.0f);
    [SerializeField] private Vector3 _rotationOff = new Vector3(60.0f, 0.0f, 0.0f);
    [SerializeField] private float _rotationRate = 720.0f;

    private bool _isEnabled = false;
    private Coroutine _rotationCoroutine;


    public void Toggle() => SetValue(!_isEnabled);
    public void SetValue(bool newValue)
    {
        if (_rotationCoroutine != null)
            StopCoroutine(_rotationCoroutine);

        _isEnabled = newValue;
        _rotationCoroutine = StartCoroutine(RotateTowards());
    }
    public void SetValueInstant(bool newValue)
    {
        if (_rotationCoroutine != null)
            StopCoroutine(_rotationCoroutine);

        _isEnabled = newValue;
        transform.localRotation = Quaternion.Euler(_isEnabled ? _rotationOn : _rotationOff);
    }

    private IEnumerator RotateTowards()
    {
        Quaternion targetRotation = Quaternion.Euler(_isEnabled ? _rotationOn : _rotationOff);
        while (transform.localRotation != targetRotation)
        {
            transform.localRotation = Quaternion.RotateTowards(transform.localRotation, targetRotation, _rotationRate * Time.deltaTime);
            yield return null;
        }
    }
}
