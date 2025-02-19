using Entities.Player;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

public class CameraFocusLook : MonoBehaviour
{
    [SerializeField] private Camera playerCamera;
    [SerializeField] private GameObject focusLookTarget;

    public float lookStrength = 3f;
    public float resistanceStrength = 3f;

    [SerializeField] private bool isFocusLookActive;
    public float focusLookDuration;
    public float focusLookTimer;

    public float cameraXRotation;
    public float cameraYRotation;


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

        Vector3 directionToTarget = focusLookTarget.transform.position - playerCamera.transform.position;
        Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);

        playerCamera.transform.rotation = Quaternion.Slerp(playerCamera.transform.rotation, targetRotation, Time.deltaTime * lookStrength);

        Vector2 lookInput = PlayerInput.GetLookInputWithSensitivity * Time.deltaTime;

        Vector2 resistance = new Vector2(-lookInput.x, lookInput.y);
        playerCamera.transform.Rotate(resistance * resistanceStrength);

        if (focusLookTimer >= focusLookDuration)
        {
            StopFocusLook();
        }
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

        _preventedActionTypes = preventedActionTypes;
        PlayerInput.PreventActions(this.GetType(), preventedActionTypes);
    }
    private void StopFocusLook()
    {
        cameraXRotation = playerCamera.transform.eulerAngles.x;
        cameraYRotation = playerCamera.transform.eulerAngles.y;

        Debug.Log($"[Focus Look] Stored X Rotation: {cameraXRotation}");
        Debug.Log($"[Focus Look] Stored Y Rotation: {cameraYRotation}");

        isFocusLookActive = false;

        PlayerInput.RemoveActionPrevention(this.GetType(), _preventedActionTypes);
    }

    public bool IsFocusLookActive()
    {
        return isFocusLookActive;
    }
}
