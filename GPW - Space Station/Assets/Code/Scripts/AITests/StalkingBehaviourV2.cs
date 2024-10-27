using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace GPW.Tests.AI.Stalking
{
    public class StalkingBehaviourV2 : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private NavMeshAgent _agent;
        [SerializeField] private Transform _target;


        [Header("Settings")]
        [SerializeField] private float _hidingSpotCheckRadius;
        [SerializeField] private LayerMask _hideableLayers;

        [Space(5)]
        [SerializeField] private float _hidingSensitivity = 0.0f; // Lower values are better hiding spots.
        [SerializeField] private float _secondTestOffsetDistance = 2.0f;


        private void Start()
        {
            StartCoroutine(StalkTarget());
        }


        private IEnumerator StalkTarget()
        {
            // Run until cancelled.
            while(true)
            {
                // Find a hiding spot near the target.
                // 
            }
        }
    }
}