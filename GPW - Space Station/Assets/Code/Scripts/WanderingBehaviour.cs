using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem.HID;


namespace GPW.Tests.AI
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class WanderingBehaviour : MonoBehaviour
    {
        [Header("General Settings")]
        private NavMeshAgent _agent;


        [Header("Wander Settings (General)")]
        [SerializeField] private Vector3 _wanderCentre;
        [SerializeField] private float _wanderRadius;

        [Space(5)]
        [SerializeField] private float _minDistanceToNewTarget;

        [Space(5)]
        [SerializeField] private float _reachedDestinationDistance;
        private bool _waitingForNewDestination;

        
        [Header("Wander Settings (Pausing)")]
        [SerializeField] private bool _canPause = true;
        [SerializeField] [Range(0.0f, 1.0f)] private float _wanderPauseChance;

        [Space(5)]
        [SerializeField] private float _minWanderContinueDelay;
        [SerializeField] private float _maxWanderContinueDelay;
        private float _pauseCompleteTime;

        [Space(5)]
        [SerializeField] private float _minTimeBetweenPauses;
        private float _nextPauseAvailableTime;



        private void Awake()
        {
            _agent = GetComponent<NavMeshAgent>();

            DetermineNewDestination();
        }

        private void Update()
        {
            if (_waitingForNewDestination)
            {
                if (_pauseCompleteTime <= Time.time || !_canPause)
                {
                    DetermineNewDestination();
                }

                return;
            }

            if (HasReachedDestination())
            {
                if (_canPause)
                {
                    _agent.isStopped = true;
                    TrySetWaitingTime();
                }

                _waitingForNewDestination = true;
            }
        }


        private bool HasReachedDestination() => Vector3.Distance(transform.position, _agent.pathEndPosition) <= _reachedDestinationDistance;
        private void TrySetWaitingTime()
        {
            if (_nextPauseAvailableTime > Time.time)
            {
                // It hasn't been long enough since our last setting of the waiting delay.
                return;
            }
            if (Random.value >= _wanderPauseChance)
            {
                // We didn't roll low enough to pause.
                return;
            }

            _pauseCompleteTime = Time.time + Mathf.Lerp(_minWanderContinueDelay, _maxWanderContinueDelay, Random.value);
            _nextPauseAvailableTime = Time.time + _minTimeBetweenPauses;
        }


        private void DetermineNewDestination()
        {
            int maxIterations = 5;
            for(int i = maxIterations - 1; i >= 0; i--)
            {
                Vector3 testPosition = _wanderCentre + RandomWithinAnnulus(0.0f, _wanderRadius);

                if (NavMesh.SamplePosition(testPosition, out NavMeshHit hit, float.MaxValue, 1))
                {
                    if (i != 0 && Vector3.Distance(transform.position, hit.position) <= maxIterations)
                    {
                        continue;
                    }

                    _waitingForNewDestination = false;
                    _pauseCompleteTime = 0.0f;

                    _agent.isStopped = false;
                    _agent.SetDestination(hit.position);
                    return;
                }
            }

            Debug.LogError("Error: Failed to find wander position");
        }

        /// <summary> Get a random position within an annulus on the X-Z axis.</summary>
        private Vector3 RandomWithinAnnulus(float innerRadius, float outerRadius) => GetRandomDirectionXZ() * Random.Range(innerRadius, outerRadius);

        /// <summary> Get a random direction along a plane on the XZ axis.</summary>
        private Vector3 GetRandomDirectionXZ()
        {
            // Determine the direction of randomness.
            Vector3 randomDirection = Vector3.zero;
            while (randomDirection == Vector3.zero)
            {
                randomDirection = new Vector3(Random.Range(-1.0f, 1.0f), 0.0f, Random.Range(-1.0f, 1.0f));
            }

            // Return our randomized direction.
            return randomDirection.normalized;
        }


        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(_wanderCentre, _wanderRadius);
            Gizmos.DrawSphere(_wanderCentre, 0.1f);


            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, _minDistanceToNewTarget);


            if (_agent != null)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawSphere(_agent.pathEndPosition, 0.1f);

                Gizmos.color = Color.red;
                Gizmos.DrawSphere(_agent.destination, 0.25f);
            }
        }
    }
}
