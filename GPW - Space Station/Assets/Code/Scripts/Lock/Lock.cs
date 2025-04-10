using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Interaction;
using UI;
using Environment.Doors;
using Saving.LevelData;


[RequireComponent(typeof(FocusableObject))]
public class Lock : MonoBehaviour, IInteractable, ISaveableObject
{
    #region Saving Properties

    [field: SerializeField] public SerializableGuid ID { get; set; }
    [SerializeField] private PadlockSaveInformation _saveData;

    #endregion

    #region IInteractable Properties & Events

    [field: SerializeField] public bool IsInteractable { get; set; } = true;

    private int _previousLayer;

    public event System.Action OnSuccessfulInteraction;
    public event System.Action OnFailedInteraction;

    #endregion


    private FocusableObject _focusableObjectScript;
    public bool IsFocused { get; private set; }


    [Header("Wheels")]
    [SerializeField] private LockWheel[] _lockWheels;
    [SerializeField, ReadOnly] private int _selectedWheelIndex;

    [Space(5)]
    [SerializeField] private float _interactionMinDelay = 0.1f;
    private float _interactionReadyTime;


    [Header("Code Settings")]
    [SerializeField] private int[] _correctDigits;

    public ExternalInputDoor connectedDoor;


    private void Awake()
    {
        _focusableObjectScript ??= GetComponent<FocusableObject>();
        IsFocused = false;
    }
    protected virtual void OnDestroy()
    {
        // Remove input preventions if we were still in use when we were destroyed.
        if (IsFocused)
            PlayerInput.RemoveAllActionPrevention(typeof(Lock));

        // Saving.
        _saveData.DisabledState = DisabledState.Destroyed;
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

    public void Interact(PlayerInteraction player)
    {
        StartInteraction();
        OnSuccessfulInteraction?.Invoke();
    }
    public void Highlight() => IInteractable.StartHighlight(this.gameObject, ref _previousLayer);
    public void StopHighlighting() => IInteractable.StopHighlight(this.gameObject, _previousLayer);



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

        // Focus on our selected wheel.
        FocusOnSelectedWheel();
    }

    private void FocusOnSelectedWheel() => _focusableObjectScript.SetPositionOffset(transform.InverseTransformPoint(_lockWheels[_selectedWheelIndex].transform.position));


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
        IsFocused = true;

        // Subscribe to Input Events.
        PlayerInput.OnUILeftClickPerformed += PlayerInput_OnUILeftClickPerformed;
        PlayerInput.OnUICancelPerformed += StopInteraction;
        PlayerInput.OnUINavigateCancelled += ResetInteractTime;

        // Prevent non-UI Input.
        PlayerInput.PreventAllActions(typeof(Lock), disableGlobalMaps: true);

        // Set our selected wheel.
        UpdateSelectedWheel();

        // Start the lock focus.
        FocusOnSelectedWheel();
        _focusableObjectScript.StartFocus();

        // Hide the interaction UI and unlock our cursor.
        PlayerUIManager.Instance.HideInteractionUI();
        Cursor.lockState = CursorLockMode.Confined;
    }
    private void StopInteraction()
    {
        IsFocused = false;

        // Unsubscribe to Input Events.
        PlayerInput.OnUILeftClickPerformed -= PlayerInput_OnUILeftClickPerformed;
        PlayerInput.OnUICancelPerformed -= StopInteraction;
        PlayerInput.OnUINavigateCancelled -= ResetInteractTime;

        // Remove input prevention.
        PlayerInput.RemoveAllActionPrevention(typeof(Lock));


        // Stop the lock focus.
        _focusableObjectScript.StopFocus();

        // Show the interaction UI and lock our cursor.
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
        this._saveData = new PadlockSaveInformation(saveData, ISaveableObject.DetermineDisabledState(this), _lockWheels.Length);
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

            this._saveData = new PadlockSaveInformation(this.ID, ISaveableObject.DetermineDisabledState(this), false, currentDigits);
        }

        return this._saveData.ObjectSaveData;
    }

    protected virtual void OnEnable() => ISaveableObject.DefaultOnEnableSetting(this._saveData.ObjectSaveData, this);
    protected virtual void OnDisable() => ISaveableObject.DefaultOnDisableSetting(this._saveData.ObjectSaveData, this);
    protected virtual void LateUpdate() => ISaveableObject.UpdatePositionAndRotationInformation(this._saveData.ObjectSaveData, this);

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
    }

#endif
}
