using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Interaction;
using UnityEngine.InputSystem.Interactions;

public class Lock : MonoBehaviour, IInteractable
{
    public event System.Action OnSuccessfulInteraction;
    public event System.Action OnFailedInteraction;

    private Vector3 originPosition;
    private Quaternion originRotation;

    private Vector3 targetPosition;
    private Quaternion targetRotation;

    private bool isMoving = false;
    private float moveSpeed = 3f;

    public bool lockInteraction = false;

    [Header("Wheels")]
    [SerializeField] private LockWheel[] _lockWheels;
    [SerializeField, ReadOnly] private int _selectedWheelIndex;

    [Space(5)]
    [SerializeField] private float _interactionMinDelay = 0.1f;
    private float _interactionReadyTime;


    [Header("Code Settings")]
    [SerializeField] private int[] _correctDigits;

    

    public GameObject connectedDoor;

    void Start()
    {
        // stores locks orignial position so that it can return when not interacting
        originPosition = transform.position;
        originRotation = transform.rotation;
    }
    

    private void PlayerInput_OnUILeftClickPerformed()
    {
        // Determine if the left click falls on one of our padlock wheels.
        // If not, stop interacting with the lock?
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (!Physics.Raycast(ray, out hit))
        {
            // Our ray didn't hit anything.
            return;
        }
        
        
        if (!hit.transform.TryGetComponent<LockWheel>(out LockWheel lockWheel))
        {
            // We didn't click a wheel.
            return;
        }

        // Determine the clicked wheel's index.
        int selectedWheelIndex = -1;
        for(int i = 0; i < _lockWheels.Length; ++i)
        {
            if (_lockWheels[i] == lockWheel)
            {
                selectedWheelIndex = i;
                break;
            }
        }

        if (selectedWheelIndex == -1)
        {
            // The selected wheel wasn't on this Padlock.
            return;
        }


        // Select the wheel.
        _selectedWheelIndex = selectedWheelIndex;
        UpdateSelectedWheel();

        // Rotate the wheel.
        lockWheel.IncrementWheel();
    }


    void Update()
    {
        if (isMoving && lockInteraction)
        {
            // moves lock towards camera
            transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * moveSpeed);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * moveSpeed);

            // Stop moving when close enough to camera
            if (Vector3.Distance(transform.position, targetPosition) < 0.01f)
            {
                transform.position = targetPosition;
                transform.rotation = targetRotation;
                isMoving = false;
            }
        }
    }

    public void Interact(PlayerInteraction player)
    {
        StartInteraction();
        OnSuccessfulInteraction?.Invoke();
    }

    public void SetTargetPosition()
    {
        Camera mainCamera = Camera.main;
        if (mainCamera != null)
        {
            // offsets lock infront of camera
            targetPosition = mainCamera.transform.position + mainCamera.transform.forward * 0.75f;

            // little offsets as lock wasnt in centre of camera
            targetPosition += mainCamera.transform.right * 0.1f;
            targetPosition += mainCamera.transform.up * -0.1f;

            // lock faces camera
            targetRotation = Quaternion.LookRotation(-mainCamera.transform.forward);
        }
    }

    public void ResetLockPosition()
    {
        lockInteraction = false;

        transform.position = originPosition;
        transform.rotation = originRotation;
    }



    public void SelectNextWheel()
    {
        _selectedWheelIndex++;
        if (_selectedWheelIndex > _lockWheels.Length - 1)
        {
            _selectedWheelIndex = 0;
        }

        UpdateSelectedWheel();
    }
    public void SelectPreviousWheel()
    {
        _selectedWheelIndex--;
        if (_selectedWheelIndex < 0)
        {
            _selectedWheelIndex = _lockWheels.Length - 1;
        }

        UpdateSelectedWheel();
    }
    private void UpdateSelectedWheel()
    {
        // Deselect all wheels.
        for(int i = 0; i < _lockWheels.Length; ++i)
        {
            _lockWheels[i].Deselect();
        }

        // Select our selected wheel;
        _lockWheels[_selectedWheelIndex].Select();
    }


    // Checks if wheel digits match the correct digits.
    public void DetectCompletion()
    {
        for(int i = 0; i < _lockWheels.Length; ++i)
        {
            if (_lockWheels[i].GetWheelDigit() != _correctDigits[i])
            {
                // This LockWheel is set to an incorrect value.
                return;
            }
        }

        // None of our LockWheels were set to incorrect digits.
        StopInteraction();
        connectedDoor.GetComponent<LockerDoor>().ToggleDoor();
        Destroy(this.gameObject);
    }


    private void StartInteraction()
    {
        // Input.
        PlayerInput.OnUILeftClickPerformed += PlayerInput_OnUILeftClickPerformed;
        PlayerInput.OnUICancelPerformed += StopInteraction;

        PlayerInput.PreventAllActions(typeof(Lock));


        // gets position infront of camera for lock to move to
        SetTargetPosition();

        isMoving = true;
        lockInteraction = true;
        UpdateSelectedWheel();

        Cursor.lockState = CursorLockMode.Confined;
    }
    private void StopInteraction()
    {
        // Input.
        PlayerInput.OnUILeftClickPerformed -= PlayerInput_OnUILeftClickPerformed;
        PlayerInput.OnUICancelPerformed -= StopInteraction;

        PlayerInput.RemoveAllActionPrevention(typeof(Lock));


        // Reset the lock's position.
        ResetLockPosition();
        Cursor.lockState = CursorLockMode.Locked;
    }


    public bool CanInteract() => _interactionReadyTime < Time.time;
    public void UpdateInteractTime() => _interactionReadyTime = Time.time + _interactionMinDelay;

    public int GetSelectedWheelIndex() => _selectedWheelIndex;


#if UNITY_EDITOR

    private void OnValidate()
    {
        if (_lockWheels.Length != _correctDigits.Length)
        {
            // Alter size of Correct Digits array.
            int[] newValues = new int[_lockWheels.Length];
            int correctDigitsLength = _correctDigits.Length;
            for (int i = 0; i < _lockWheels.Length; ++i)
            {
                if (i < correctDigitsLength)
                {
                    newValues[i] = _correctDigits[i];
                }
                else
                {
                    newValues[i] = 0;
                }
            }

            _correctDigits = newValues;
        }
    }

#endif
}
