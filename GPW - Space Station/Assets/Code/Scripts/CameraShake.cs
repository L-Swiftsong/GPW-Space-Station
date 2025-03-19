using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CameraShake : MonoBehaviour
{
    private float shakeIntensity; //Store current intensity.
    private float shakeSpeed; //Store current speed.

    [Header("Walk Values")]
    [SerializeField] private float walkShakeIntensity;
    [SerializeField] private float walkShakeSpeed;

    [Header("Run Values")]
    [SerializeField] private float runShakeIntensity;
    [SerializeField] private float runShakeSpeed;

    private Vector3 originalPosition;
    private float timeCounter;

    public bool IsShaking { get; private set; } //Is walking/running essentially.

    //toggles
    private bool cameraSwayToggled = true;
    private bool eventSwayToggled = true;

    private bool eventInProgress = false;

    void Start()
    {
        originalPosition = transform.localPosition;
        timeCounter = 0f;
    }

    void Update()
    {
        if (IsShaking && cameraSwayToggled && !eventInProgress) //If walking/running, camera sway is toggled & no event occuring, apply camera sway.
            ApplyShake(shakeIntensity, shakeSpeed);
        else if (!eventInProgress)
            ResetPosition();
    }


    public void SetShakeState(bool isWalking, bool isRunning) //Called from Player controller, depending on movement state, change values.
    {
        if (cameraSwayToggled)
        {
            if (isWalking) //lower values
            {
                shakeIntensity = walkShakeIntensity;
                shakeSpeed = walkShakeSpeed;
                IsShaking = true;
            }
            else if (isRunning) //higher values
            {
                shakeIntensity = runShakeIntensity;
                shakeSpeed = runShakeSpeed;
                IsShaking = true;
            }
            else //Stop camera shake
            {
                IsShaking = false;
            }
        }
    }


    private void ApplyShake(float intensity, float speed) //whats moving da camera.
    {
        timeCounter += Time.deltaTime * speed;
        float shakeX = Mathf.Sin(timeCounter) * intensity;
        float shakeY = Mathf.Cos(timeCounter * 1.5f) * intensity;

        transform.localPosition = originalPosition + new Vector3(shakeX, shakeY, 0);
    }


    private void ResetPosition() //Return to normal after shake.
    {
        transform.localPosition = Vector3.Lerp(transform.localPosition, originalPosition, Time.deltaTime * 5f);
    }


    #region Event

    public static void StartEventShake(float intensity, float speed, float duration)
    {
        var playerInstance = Entities.Player.PlayerManager.Instance;

        if (playerInstance.CameraShake)
        {
            playerInstance.CameraShake.StartShakeCoroutine(intensity, speed, duration);
        }
    }


    private void StartShakeCoroutine(float intensity, float speed, float duration)
    {
        if (!eventInProgress)
        {
            StartCoroutine(ShakeCoroutine(intensity, speed, duration));
        }
    }


    public IEnumerator ShakeCoroutine(float intensity, float speed, float duration)
    {
        if (eventSwayToggled)
        {
            eventInProgress = true;
            shakeIntensity = intensity;
            shakeSpeed = speed;

            float elapsedTime = 0f;

            while (elapsedTime < duration)
            {
                ApplyShake(intensity, speed);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            eventInProgress = false;
            ResetPosition();
        }
    }

    #endregion


    #region Toggles

    public void ToggleCameraSway() //Call in settings to enable/disable general camera sway.
    {
        if (cameraSwayToggled)
        {
            cameraSwayToggled = false;
        }
        else
        {
            cameraSwayToggled = true;
        }
    }


    public void ToggleEventSway() //Call in settings to enable/disable event camera sway.
    {
        if (eventSwayToggled)
        {
            eventSwayToggled = false;
        }
        else
        {
            eventSwayToggled = true;
        }
    }

    #endregion
}




