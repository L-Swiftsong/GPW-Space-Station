using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Interaction;
using UI;
using Environment.Doors;
using Saving.LevelData;

public class Lock : MonoBehaviour, IInteractable, ISaveableObject
{
    #region Saving Properties

    [field: SerializeField] public SerializableInstanceGuid ID { get; set; } = SerializableInstanceGuid.NewUnlinkedGuid();
    [SerializeField] private PadlockSaveInformation _saveData;

    #endregion


    private Vector3 originPosition;
    private Quaternion originRotation;

    private Vector3 targetPosition;
    private Quaternion targetRotation;

    private bool isMoving = false;
    private float moveSpeed = 3f;

    public bool lockInteraction = false;

    [SerializeField] private float _cameraOffsetDistance = 0.75f;


    [Header("Wheels")]
    [SerializeField] private LockWheel[] _lockWheels;
    [SerializeField, ReadOnly] private int _selectedWheelIndex;

    [Space(5)]
    [SerializeField] private float _interactionMinDelay = 0.1f;
    private float _interactionReadyTime;


    [Header("Code Settings")]
    [SerializeField] private int[] _correctDigits;

    public ExternalInputDoor connectedDoor;


    #region IInteractable Properties & Events

    private int _previousLayer;

    public event System.Action OnSuccessfulInteraction;
    public event System.Action OnFailedInteraction;

    #endregion


    void Start()
    {
        // stores locks orignial position so that it can return when not interacting
        originPosition = transform.position;
        originRotation = transform.rotation;
    }
    

    private void PlayerInput_OnUILeftClickPerformed()
    {
        Debug.Log("Left Click Performed");

        // Determine if the left click falls on one of our padlock wheels.
        // If not, stop interacting with the lock?
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (!Physics.Raycast(ray, out hit))
        {
            // Our ray didn't hit anything.
            Debug.Log("Ray Miss");
            return;
        }
        
        
        if (!hit.transform.TryGetComponent<LockWheel>(out LockWheel lockWheel))
        {
            // We didn't click a wheel.
            Debug.Log($"Hit Object ({hit.transform.name}) isn't a LockWheel");
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


        if (selectedWheelIndex != _selectedWheelIndex)
        {
            // We've selected a new wheel.
            // Update our selected wheel.
            _selectedWheelIndex = selectedWheelIndex;
            UpdateSelectedWheel();
            return;
        }
        else
        {
            // We haven't selected a new wheel.
            // Rotate the wheel.
            bool positiveIncrement = Vector3.Dot(hit.point - hit.transform.position, this.transform.right) < 0;
            lockWheel.IncrementWheel(positiveIncrement);
        }
    }


    void Update()
    {
        if (isMoving && lockInteraction)
        {
            // Update the target position.
            SetTargetPosition();

            // Move the lock towards the camera.
            transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * moveSpeed);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * moveSpeed);
        }
    }

    public void Interact(PlayerInteraction player)
    {
        StartInteraction();
        OnSuccessfulInteraction?.Invoke();
    }
    public void Highlight() => IInteractable.StartHighlight(this.gameObject, ref _previousLayer);
    public void StopHighlighting() => IInteractable.StopHighlight(this.gameObject, _previousLayer);


    public void SetTargetPosition()
    {
        Transform playerCameraTransform = Entities.Player.PlayerManager.Instance.GetPlayerCameraTransform();

        // Offsets the lock infront of the camera.
        targetPosition = GetLockPositionForWheelIndex(playerCameraTransform, _selectedWheelIndex);

        // Lock faces camera.
        targetRotation = Quaternion.LookRotation(-playerCameraTransform.forward);
    }
    private Vector3 GetLockPositionForWheelIndex(Transform playerCamTransform, int wheelIndex)
    {
        Vector3 wheelLocalPosition = transform.InverseTransformPoint(_lockWheels[wheelIndex].transform.position);

        Vector3 desiredLockPosition = playerCamTransform.position + (playerCamTransform.forward * _cameraOffsetDistance);
        desiredLockPosition += (playerCamTransform.right * wheelLocalPosition.x) + (playerCamTransform.up * -wheelLocalPosition.y);

        return desiredLockPosition;
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
        bool allCorrect = true;
        for(int i = 0; i < _lockWheels.Length; ++i)
        {
            _saveData.CurrentSetValues[i] = _lockWheels[i].GetWheelDigit();
            if (_lockWheels[i].GetWheelDigit() != _correctDigits[i])
            {
                // This LockWheel is set to an incorrect value.
                allCorrect = false;
            }
        }

        if (allCorrect)
        {
            // All of our LockWheels were set to the correct digits.
            LockDigitsCorrect();
        }
    }
    private void LockDigitsCorrect()
    {
        StopInteraction();
        connectedDoor.Activate();
        Destroy(this.gameObject);
    }


    private void StartInteraction()
    {
        // Input.
        PlayerInput.OnUILeftClickPerformed += PlayerInput_OnUILeftClickPerformed;
        PlayerInput.OnUICancelPerformed += StopInteraction;
        PlayerInput.OnUINavigateCancelled += ResetInteractTime;

        PlayerInput.PreventAllActions(typeof(Lock), disableGlobalMaps: true);


        // gets position infront of camera for lock to move to
        SetTargetPosition();

        isMoving = true;
        lockInteraction = true;
        UpdateSelectedWheel();

        PlayerUIManager.Instance.HideInteractionUI();
        Cursor.lockState = CursorLockMode.Confined;
    }
    private void StopInteraction()
    {
        // Input.
        PlayerInput.OnUILeftClickPerformed -= PlayerInput_OnUILeftClickPerformed;
        PlayerInput.OnUICancelPerformed -= StopInteraction;
        PlayerInput.OnUINavigateCancelled -= ResetInteractTime;

        PlayerInput.RemoveAllActionPrevention(typeof(Lock));


        // Reset the lock's position.
        ResetLockPosition();

        PlayerUIManager.Instance.ShowInteractionUI();
        Cursor.lockState = CursorLockMode.Locked;
    }


    public bool CanInteract() => _interactionReadyTime < Time.time;
    public void UpdateInteractTime() => _interactionReadyTime = Time.time + _interactionMinDelay;
    private void ResetInteractTime() => _interactionReadyTime = 0.0f;

    public int GetSelectedWheelIndex() => _selectedWheelIndex;



    #region Saving Functions

    public void BindExisting(ObjectSaveData saveData)
    {
        this._saveData = new PadlockSaveInformation(saveData);
        _saveData.ID = ID;

        ISaveableObject.PerformBindingChecks(this._saveData.ObjectSaveData, this);

        for (int i = 0; i < _lockWheels.Length; ++i)
        {
            _lockWheels[i].SetWheelDigit(_saveData.CurrentSetValues[i]);
        }
    }
    public ObjectSaveData BindNew()
    {
        if (this._saveData == null || !this._saveData.Exists)
        {
            int[] currentDigits = new int[this._lockWheels.Length];
            for(int i = 0; i < this._lockWheels.Length; ++i)
            {
                currentDigits[i] = _lockWheels[i].GetWheelDigit();
            }

            this._saveData = new PadlockSaveInformation(this.ID, false, currentDigits);
        }

        return this._saveData.ObjectSaveData;
    }

    private void OnEnable() => ISaveableObject.DefaultOnEnableSetting(this._saveData.ObjectSaveData, this);
    private void OnDestroy() => _saveData.DisabledState = DisabledState.Destroyed;
    private void OnDisable() => ISaveableObject.DefaultOnDisableSetting(this._saveData.ObjectSaveData, this);
    public void InitialiseID() => ID.LinkGuidToGameObject(this.gameObject);

    #endregion


#if UNITY_EDITOR

    private void OnValidate()
    {
        if (_lockWheels == null || _correctDigits == null)
        {
            return;
        }

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


        // Saving - Initialise our Guid ID.
        if (ID.IsUnlinked())
        {
            InitialiseID();
        }
    }

#endif
}
