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
        }


        private void OnTriggerEnter(Collider other)
        {
            if (!_detectedLayers.Contains(other.gameObject))
            {
                return;
            }

            _openCount++;
            if (_openCount == 1)
            {
                _openTimeRemaining = _openDuration;
                Open();
            }
        }
        private void OnTriggerExit(Collider other)
        {
            if (!_detectedLayers.Contains(other.gameObject))
            {
                return;
            }

            _openCount--;
        }
    }
}