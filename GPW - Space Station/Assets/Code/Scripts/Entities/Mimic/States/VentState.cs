using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Environment.Vents;

namespace Entities.Mimic.States
{
    public class VentState : State
    {
        public override string Name => "Venting";


        [Header("References")]
        [SerializeField] private NavMeshAgent _agent;


        [Header("Venting Settings")]
        [SerializeField] private float _maxVentDetectionRadius = 5.0f;
        private const float ENTER_VENT_SQR_DISTANCE = 1.0f;

        [Space(5)]
        [SerializeField] private float _minTimeBetweenVents = 10.0f; // The minimum time between the agent entering the vents.
        private float _ventCooldownComplete;

        private Vent _targetVent;
        private VentEntrance _targetEntrance;
        private bool _isInVent = false;


        private bool _shouldExitState;
        public bool ShouldExitState() => _shouldExitState;


        public override void OnEnter()
        {
            _shouldExitState = false;
            FindTargetVent();
        }
        public override void OnLogic()
        {
            if (_shouldExitState)
                return;


            if (_isInVent)
            {
                // We are moving through a vent.
                MoveThroughVent();
            }
            else if ((_agent.transform.position - _targetEntrance.transform.position).sqrMagnitude <= ENTER_VENT_SQR_DISTANCE)
            {
                // We have arrived at our target vent.
                EnterVent();
            }
        }


        public bool CanEnter() => _ventCooldownComplete <= Time.time;


        /// <summary> Attempt to find a Vent Entrance that we can travel to. If we fail to do so, request that we exit this state.</summary>
        private void FindTargetVent()
        {
            // Reset previous references.
            _targetVent = null;
            _targetEntrance = null;
            _isInVent = false;

            Debug.Log("Attempt Enter Vent State");


            // Get the closest Vent Entrance (Based on NavMesh distance, not Euclidean distance).
            List<VentEntrance> ventEntrances = new List<VentEntrance>();
            foreach (Collider collider in Physics.OverlapSphere(transform.position, _maxVentDetectionRadius))
            {
                if (collider.TryGetComponent<VentEntrance>(out VentEntrance ventEntrance))
                {
                    ventEntrances.Add(ventEntrance);
                }
            }

            // Set our target entrance to the closest vent entrance (Based on NavMesh distance, not Euclidean distance).
            float closestSqrDistance = float.PositiveInfinity;
            NavMeshPath path = new NavMeshPath();
            for (int i = 0; i < ventEntrances.Count; i++)
            {
                if (!_agent.TryCalculateSqrDistanceToPoint(ventEntrances[i].transform.position, out float sqrDistance))
                {
                    // We couldn't find a valid path to this Vent Entrance.
                    continue;
                }

                // We found a valid path to this Vent Entrance.

                if (sqrDistance < closestSqrDistance)
                {
                    // This vent entrance is closer than our cached closest vent.
                    _targetEntrance = ventEntrances[i];
                    closestSqrDistance = sqrDistance;
                }
            }

            if (_targetEntrance == null)
            {
                // We were unable to find a path to a valid entrance, and thus shouldn't have entered this state.
                Debug.Log("Vent State Failure");
                _shouldExitState = true;
                return;
            }

            _targetVent = _targetEntrance.GetComponentInParent<Vent>();
            _agent.SetDestination(_targetEntrance.transform.position);
            Debug.Log("Vent State Success");
        }


        /// <summary> Move through the current vent towards our desired exit.</summary>
        private void MoveThroughVent()
        {
            // (Temp) Teleport to the vent exit.
            _agent.enabled = false;
            transform.position = _targetEntrance.transform.position;
            _agent.enabled = true;

            // (Temp) Exit this state.
            ExitVent();
        }

        /// <summary> Enter a vent through our current targetEntrance.</summary>
        private void EnterVent()
        {
            // We have reached our target vent.
            _isInVent = true;

            // Choose a random vent entrance (Which is not our current entrance, unless said entrance is Omnidirectional).
            int maxIndex = _targetVent.VentEntrances.Length - (_targetEntrance.IsOmnidirectional ? 0 : 1);
            int randomIndex = Random.Range(0, maxIndex);

            VentEntrance newTarget = _targetVent.VentEntrances[randomIndex];
            _targetEntrance = ((!_targetEntrance.IsOmnidirectional && newTarget != _targetEntrance) ? newTarget : (_targetVent.VentEntrances[randomIndex == 0 ? 1 : randomIndex - 1]));
        }
        /// <summary> Exit the vent we are currently in, subsequently exiting this state.</summary>
        private void ExitVent()
        {
            // Prevent this agent from entering a vent again until the minimum duration has passed.
            _ventCooldownComplete = Time.time + _minTimeBetweenVents;

            // Notify the Controller that we have exited the vent and should switch states.
            _shouldExitState = true;
        }
    }
}