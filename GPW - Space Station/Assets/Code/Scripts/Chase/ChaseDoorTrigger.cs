using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Environment.Doors;


namespace Chase
{
    public class ChaseDoorTrigger : MonoBehaviour
    {
        [SerializeField] private ExternalInputDoor[] _doors;
        private bool _isTriggered = false;


        private void Awake()
        {
            for(int i = 0; i < _doors.Length; i++)
            {
                _doors[i].Activate();
            }
        }
        private void OnTriggerEnter(Collider other)
        {
            if (!_isTriggered && other.CompareTag("Player"))
            {
                _isTriggered = true;
                Debug.Log("DOOR CLOSING");

                // begin moving the doors
                for (int i = 0; i < _doors.Length; i++)
                {
                    _doors[i].Deactivate();
                }
            }
        }
    }
}