using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFocusLook : MonoBehaviour
{
    [SerializeField] private Transform _rotationPivot;
    [SerializeField] private Camera playerCamera;

    [SerializeField] private GameObject focusLookTarget;


    private float lookStrength = 3f;

    [SerializeField, ReadOnly] private bool isFocusLookActive;
    [SerializeField] private float _cameraInputPreventionDefaultTime = 0.5f;
    private float focusLookDuration;
    private float focusLookTimer;


    // Input Prevention.
    private PlayerInput.ActionTypes _preventedActionTypes;


    private void Start()
    {
        playerCamera = Camera.main;
        isFocusLookActive = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (isFocusLookActive)
        {
            HandleFocusLook();
        }
    }

    private void HandleFocusLook()
    {
        focusLookTimer += Time.deltaTime;

        if (!_preventedActionTypes.HasFlag(PlayerInput.ActionTypes.Camera) && ShouldStopFocusLook(false))
        {
            PlayerInput.RemoveActionPrevention(this.GetType(), PlayerInput.ActionTypes.Camera);
            StopFocusLook();
            return;
        }
        if (ShouldStopFocusLook(true))
        {
            StopFocusLook();
            return;
        }

        Vector3 directionToTarget = focusLookTarget.transform.position - playerCamera.transform.position;
        Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);

        float originalCameraLocalZ = playerCamera.transform.localEulerAngles.z;
        playerCamera.transform.rotation = Quaternion.Slerp(playerCamera.transform.rotation, targetRotation, Time.deltaTime * lookStrength);
        _rotationPivot.rotation = Quaternion.Euler(0.0f, playerCamera.transform.rotation.eulerAngles.y, 0.0f);
        playerCamera.transform.localEulerAngles = new Vector3(playerCamera.transform.localEulerAngles.x, 0.0f, originalCameraLocalZ);
    }

    public static void TriggerFocusLookStatic(GameObject target, float duration = 3f, float strength = 3f, PlayerInput.ActionTypes preventedActionTypes = PlayerInput.ActionTypes.Movement)
    {
        var playerInstance = Entities.Player.PlayerManager.Instance;

        if (playerInstance.CameraFocusLook)
        {
            playerInstance.CameraFocusLook.TriggerFocusLook(target, duration, strength, preventedActionTypes);
        }
    }

    public void TriggerFocusLook(GameObject target, float duration = 3f, float strength = 3f, PlayerInput.ActionTypes preventedActionTypes = PlayerInput.ActionTypes.Movement)
    {
        focusLookTarget = target;
        focusLookDuration = duration;
        lookStrength = strength;
        focusLookTimer = 0f;
        isFocusLookActive = true;


        if (focusLookDuration <= _cameraInputPreventionDefaultTime)
        {
            // Our camera focus duration is less than our default camera prevention time.
            // Treat it as if we wanted to prevent camera input by default.
            preventedActionTypes |= PlayerInput.ActionTypes.Camera;
        }
        _preventedActionTypes = preventedActionTypes;

        // Note: We're including Camera input in the prevention here so that the camera input isn't registered for the first '_cameraInputPreventionDefaultTime' seconds.
        //      This effectively does nothing if we are already disabling camera input.
        PlayerInput.PreventActions(this.GetType(), preventedActionTypes | PlayerInput.ActionTypes.Camera);

    }

    private bool ShouldStopFocusLook(bool allowInputToCancel)
    {
        if (focusLookTimer >= focusLookDuration)
        {
            // Focus look duration elapsed.
            return true;
        }
        if (focusLookTarget == null)
        {
            // Lost reference to our target.
            return true;
        }
        if (allowInputToCancel && PlayerInput.GetLookInputWithSensitivity.sqrMagnitude > 0.0f)
        {
            // We received input.
            return true;
        }

        return false;
    }
    private void StopFocusLook()
    {
        isFocusLookActive = false;

        PlayerInput.RemoveActionPrevention(this.GetType(), _preventedActionTypes);
    }

    public bool IsFocusLookActive() => isFocusLookActive;
}
