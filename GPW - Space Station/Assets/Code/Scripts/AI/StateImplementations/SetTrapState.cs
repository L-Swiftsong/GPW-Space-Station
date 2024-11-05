using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace AI.States
{
    public class SetTrapState : State
    {
        public override string Name => "Set Trap";


        [Header("References")]
        [SerializeField] private NavMeshAgent _agent;


        [Header("Trap Detection Settings")]
        [SerializeField] private float _trapDetectionRadius = 10.0f; // How close the agent must be to a potential trap spot to consider laying a trap.
        private const float REACHED_TRAP_POINT_SQR_DISTANCE = 0.25f;


        [Space(5)]
        [SerializeField] private float _maxTrapTime = 10.0f; // The maximum time that the agent can be preparing a trap.
        private float _currentTrapTime;
        private TrapPoint _targetTrapPoint;
        private bool _hasReachedTargetTrapPoint;


        [Header("Trap Cooldown Settings")]
        [SerializeField] private float _minTimeBetweenEntries = 10.0f; // The minimum time after the agent exits this state before they can re-enter it.
        private float _setTrapReadyTime;


        private bool _stateEntryFailed;
        public bool ShouldExitState() => _stateEntryFailed || _currentTrapTime >= _maxTrapTime;
        public bool CanEnter() => _setTrapReadyTime <= Time.time;


        public override void OnEnter()
        {
            // Reset previous values.
            _targetTrapPoint = null;
            _hasReachedTargetTrapPoint = false;
            _currentTrapTime = 0.0f;
            _stateEntryFailed = false;

            // Attempt to find a Trap Point within our detection range.
            FindTrapPoint();
        }
        public override void OnLogic()
        {
            if (_stateEntryFailed)
                return;


            if (_hasReachedTargetTrapPoint)
            {
                WaitInTrapPoint();
            }
            else
            {
                MoveToTrapPoint();
            }
        }
        public override void OnExit()
        {
            _agent.isStopped = false;
            _setTrapReadyTime = Time.time + _minTimeBetweenEntries;
        }


        private void FindTrapPoint()
        {
            Debug.Log("Attempt Enter Trap State");

            // Find all nearby TrapPoints.
            List<TrapPoint> nearbyTrapPoints = TrapManager.GetTrapPointsWithinRange(transform.position, _trapDetectionRadius);

            if (nearbyTrapPoints.Count <= 0)
            {
                // There were no trap points in range. We cannot enter the SetTrap state.
                Debug.Log("Trap State Failure");
                _stateEntryFailed = true;
                return;
            }

            // Choose a random trapPoint for our target.
            _targetTrapPoint = nearbyTrapPoints[Random.Range(0, nearbyTrapPoints.Count)];
            _agent.SetDestination(_targetTrapPoint.transform.position);
            Debug.Log("Trap State Success");
        }

        private void MoveToTrapPoint()
        {
            if ((_agent.transform.position - _targetTrapPoint.transform.position).sqrMagnitude > REACHED_TRAP_POINT_SQR_DISTANCE)
            {
                // We haven't reached the trap point yet.
                return;
            }

            // We have reached the target trap point.
            // Ensure we are at the trap's position.
            _agent.isStopped = true;
            transform.position = _targetTrapPoint.transform.position;
            _hasReachedTargetTrapPoint = true;
        }

        private void WaitInTrapPoint()
        {
            // Rotate to face the trap's forward.
            transform.rotation = Quaternion.RotateTowards(_agent.transform.rotation, Quaternion.LookRotation(_targetTrapPoint.transform.forward, transform.up), _agent.angularSpeed * Time.deltaTime);

            // Increase the time that we have spent waiting.
            _currentTrapTime += Time.deltaTime;
        }
    }
}