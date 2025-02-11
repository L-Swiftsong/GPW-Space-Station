using UnityEngine;
using System.Collections;

public class LockerDoor : MonoBehaviour
{
    public float openAngle = 90f;
    public float speed = 2f;
    private bool isOpen = false;

    private Vector3 hingePoint;
    private float currentAngle = 0f;

    void Start()
    {
        hingePoint = transform.position + new Vector3(-0.5f, 0, 0);
    }

    public void ToggleDoor()
    {
        StartCoroutine(RotateDoor(isOpen ? -openAngle : openAngle));
        isOpen = !isOpen;
    }


    IEnumerator RotateDoor(float angle)
    {
        float rotatedAmount = 0f;
        float rotationStep = speed * Time.deltaTime * angle;

        while (Mathf.Abs(rotatedAmount) < Mathf.Abs(angle))
        {
            float step = Mathf.Sign(angle) * Mathf.Min(Mathf.Abs(rotationStep), Mathf.Abs(angle - rotatedAmount));
            transform.RotateAround(hingePoint, Vector3.up, step);
            rotatedAmount += step;
            yield return null;
        }
    }
}
