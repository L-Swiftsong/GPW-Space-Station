using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockWheel : MonoBehaviour
{
    public int wheelDigit = 0;
    private Lock _lock;

    void Start()
    {
        _lock = GetComponentInParent<Lock>();
    }

    // Finds out what digit each wheel is on based on wheels rotation
    // Sorry this is so messy lol feel free to change this if anyone wants
    void Update()
    {
        float yRotation = transform.localEulerAngles.y;

        if (_lock.lockInteraction)
        {
            if (Mathf.Abs(yRotation - 0f) < 1f)
            {
                wheelDigit = 0;
            }
            else if (Mathf.Abs(yRotation - 36f) < 1f)
            {
                wheelDigit = 1;
            }
            else if (Mathf.Abs(yRotation - 72f) < 1f)
            {
                wheelDigit = 2;
            }
            else if (Mathf.Abs(yRotation - 108f) < 1f)
            {
                wheelDigit = 3;
            }
            else if (Mathf.Abs(yRotation - 144f) < 1f)
            {
                wheelDigit = 4;
            }
            else if (Mathf.Abs(yRotation - 180f) < 1f)
            {
                wheelDigit = 5;
            }
            else if (Mathf.Abs(yRotation - 216f) < 1f)
            {
                wheelDigit = 6;
            }
            else if (Mathf.Abs(yRotation - 252f) < 1f)
            {
                wheelDigit = 7;
            }
            else if (Mathf.Abs(yRotation - 288f) < 1f)
            {
                wheelDigit = 8;
            }
            else if (Mathf.Abs(yRotation - 324f) < 1f)
            {
                wheelDigit = 9;
            }
        }
    }
}
