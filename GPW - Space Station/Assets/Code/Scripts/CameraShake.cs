using UnityEngine;

public class CameraShake : MonoBehaviour
{
    private float shakeIntensity;
    private float shakeSpeed;

    [Header("Walk Values")]
    public float walkShakeIntensity;
    public float walkShakeSpeed;

    [Header("Run Values")]
    public float runShakeIntensity;
    public float runShakeSpeed;

    private Vector3 originalPosition;
    private float timeCounter;

    public bool IsShaking { get; private set; }

    void Start()
    {
        originalPosition = transform.localPosition;
        timeCounter = 0f;
    }

    void Update()
    {
        if (IsShaking)
            ApplyShake();
        else
            ResetPosition();
    }

    public void SetShakeState(bool isWalking, bool isRunning)
    {
        if (isWalking)
        {
            shakeIntensity = walkShakeIntensity;
            shakeSpeed = walkShakeSpeed;
            IsShaking = true;
        }
        else if (isRunning)
        {
            shakeIntensity = runShakeIntensity;
            shakeSpeed = runShakeSpeed;
            IsShaking = true;
        }
        else
        {
            IsShaking = false;
        }
    }

    private void ApplyShake()
    {
        timeCounter += Time.deltaTime * shakeSpeed;
        float shakeX = Mathf.Sin(timeCounter) * shakeIntensity;
        float shakeY = Mathf.Cos(timeCounter * 1.5f) * shakeIntensity;

        transform.localPosition = originalPosition + new Vector3(shakeX, shakeY, 0);
    }

    private void ResetPosition()
    {
        transform.localPosition = Vector3.Lerp(transform.localPosition, originalPosition, Time.deltaTime * 5f);
    }
}




