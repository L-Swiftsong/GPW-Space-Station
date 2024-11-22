using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Environment.Doors;

public class DoorTriggerController : MonoBehaviour
{
    [SerializeField] private ExternalInputDoor[] _doors;
    private bool isTriggered = false;

    private void Awake()
    {
        for(int i = 0; i < _doors.Length; i++)
        {
            _doors[i].Open();
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (!isTriggered && other.CompareTag("Player"))
        {
            isTriggered = true;
            Debug.Log("DOOR CLOSING");

            // begin moving the doors
            for (int i = 0; i < _doors.Length; i++)
            {
                _doors[i].Close();
            }
        }
    }
}
