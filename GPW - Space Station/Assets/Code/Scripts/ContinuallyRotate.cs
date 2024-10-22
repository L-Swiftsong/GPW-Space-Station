using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContinuallyRotate : MonoBehaviour
{
    [SerializeField] private float _rotationSpeed = 45.0f; // Rotation Speed in deg/s.
    [SerializeField] private Vector3 _rotationAxis = Vector3.up; // The axis we rotate around.
    

    [SerializeField] private bool _rotateAroundSelf = true;
    [SerializeField] private Vector3 _rotationCentre;


    private void Update()
    {
        transform.RotateAround(_rotateAroundSelf ? transform.position : _rotationCentre, _rotationAxis, _rotationSpeed * Time.deltaTime);
    }
}
