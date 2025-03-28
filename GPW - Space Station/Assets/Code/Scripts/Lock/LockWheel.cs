using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockWheel : MonoBehaviour
{
    private Lock _lock;


    [SerializeField, ReadOnly] private int _wheelDigit = 0;
    [SerializeField] private int _maxDigit = 9;


    private bool _isSelected = false;
    [SerializeField] private bool _isVerticalInputWheelChange = true; // Does vertical input change the active wheel, or does horizontal input?


    void Start()
    {
        _lock = GetComponentInParent<Lock>();
    }


    public void Select()
    {
        _isSelected = true;
    }
    public void Deselect()
    {
        _isSelected = false;
    }


    void Update()
    {
        if (_isSelected)
        {
            // Register Inputs.
            DetectInput();
        }
    }

    private void DetectInput()
    {
        if (!_lock.CanInteract() || !_lock.lockInteraction)
        {
            return;
        }


        Vector2 navigateInput = PlayerInput.UINavigate.normalized;
        
        // Process Wheel Change Input (Changing to another Wheel).
        float wheelChangeInput = _isVerticalInputWheelChange ? -navigateInput.y : navigateInput.x;
        if (wheelChangeInput != 0)
        {
            _lock.UpdateInteractTime();
            
            if (wheelChangeInput > 0)
            {
                _lock.SelectNextWheel();
            }
            else
            {
                 _lock.SelectPreviousWheel();
            }

            // If we are swapping wheels, ignore further input.
            return;
        }


        // Process Value Change Input (Changing this Wheel's value).
        float valueChangeInput = _isVerticalInputWheelChange ? navigateInput.x : navigateInput.y;
        if (valueChangeInput != 0)
        {
            _lock.UpdateInteractTime();
            IncrementWheel(valueChangeInput > 0);
        }
    }
    public void IncrementWheel(bool positiveIncrement = true)
    {
        // Alter value.
        _wheelDigit += positiveIncrement ? 1 : -1;

        // Clamp.
        if (_wheelDigit > _maxDigit)
        {
            _wheelDigit = 0;
        }
        else if (_wheelDigit < 0)
        {
            _wheelDigit = _maxDigit;
        }

        this.transform.localEulerAngles = new Vector3(0.0f, _wheelDigit * (360f / (_maxDigit + 1)), 0.0f);


        // Determine if the padlock is now complete.
        _lock.DetectCompletion();
    }


    /// <remarks>Does not check for completion after setting the value of '_wheelDigit'.</remarks>
    public void SetWheelDigit(int newValue)
    {
        _wheelDigit = newValue;
        this.transform.localEulerAngles = new Vector3(0.0f, _wheelDigit * (360f / (_maxDigit + 1)), 0.0f);
    }
    public int GetWheelDigit() => _wheelDigit;
}
