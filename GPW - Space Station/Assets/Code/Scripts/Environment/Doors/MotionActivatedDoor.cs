using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Environment.Doors
{
    public class MotionActivatedDoor : Door
    {
        [SerializeField] private LayerMask _detectedLayers;
        private int _openCount = 0;


        [SerializeField] private float _openDuration = 2.0f;
        private float _openTimeRemaining = 0.0f;

        public bool _isLocked = false;


        private void Update()
        {
            if (_openCount <= 0)
            {
                _openTimeRemaining -= Time.deltaTime;
                if (_openTimeRemaining <= 0)
                {
                    Close();
                }
            }

            if (_openCount < 0)
            {
                _openCount = 0;
            }
        }

        public void LockDoor()
        {
            _isLocked = true;
            _openCount = 0;
            _openTimeRemaining = 0f;
            Close();
        }

        public void CloseDoor()
        {
            _openCount = 0;
            _openTimeRemaining = 0f;
            Close();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!_detectedLayers.Contains(other.gameObject.layer))
            {
                Debug.Log(other.name + " was invalid");
                return;
            }

            if (_isLocked)
            {
                Debug.Log(other.name + " was valid but door is locked");
                return;
            }

            Debug.Log(other.name + " was valid");

            _openCount++;
            if (_openCount == 1)
            {
                _openTimeRemaining = _openDuration;
                Open();
            }
        }
        private void OnTriggerExit(Collider other)
        {
            if (!_detectedLayers.Contains(other.gameObject.layer))
            {
                return;
            }

            _openCount--;
        }
    }
}