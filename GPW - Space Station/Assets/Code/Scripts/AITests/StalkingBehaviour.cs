using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace GPW.Tests.AI.Stalking
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class StalkingBehaviour : MonoBehaviour
    {
        private NavMeshAgent _agent = null;
        private HidingSpot _currentHidingSpot = null;


        private void Awake()
        {
            _agent = GetComponent<NavMeshAgent>();
            _currentHidingSpot = null;
        }

        private void Start() => FindAvailableHidingSpot();
        

        private void FindAvailableHidingSpot()
        {
            if (HidingSpotManager.Instance.TryGetAvailableHidingSpot(out HidingSpot hidingSpot))
            {
                // We found a new hiding spot.

                SetHidingSpot(hidingSpot);
                _agent.SetDestination(_currentHidingSpot.transform.position);
            }
        }

        private void SetHidingSpot(HidingSpot newHidingSpot)
        {
            if (_currentHidingSpot != null)
            {
                // Unsubscribe from the old HidingSpot's events.
                _currentHidingSpot.OnSpotInvalidated -= HidingSpot_OnSpotInvalidated;
                _currentHidingSpot.OnSpotBecameStale -= HidingSpot_OnSpotBecameStale;

                // Exit the HidingSpot node.
                _currentHidingSpot.ExitNode();
            }

            // Set the value of '_currentHidingSpot'.
            _currentHidingSpot = newHidingSpot;


            // Subscribe to the new HidingSpot's events
            _currentHidingSpot.OnSpotInvalidated += HidingSpot_OnSpotInvalidated;
            _currentHidingSpot.OnSpotBecameStale += HidingSpot_OnSpotBecameStale;

            // Enter the new HidingSpot node.
            _currentHidingSpot.EnterNode();
        }


        private void HidingSpot_OnSpotInvalidated() => FindAvailableHidingSpot();
        private void HidingSpot_OnSpotBecameStale() => FindAvailableHidingSpot();
    }
}