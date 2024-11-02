using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


namespace AI.Mimic
{
    [RequireComponent(typeof(NavMeshAgent), typeof(EntitySenses))]
    public class MimicController : MonoBehaviour
    {
        // References.
        private NavMeshAgent _agent;
        private EntitySenses _entitySenses;
        private FlashlightStunnable _flashlightStunnableScript;


        [System.Serializable] private enum State { Wander, Search, Chase, SetTrap, Vent, Stunned };
        private State _currentState = State.Wander;



        [Header("Default Settings")]
        [SerializeField] private float _defaultMovementSpeed = 2.0f;
        [SerializeField] private float _defaultAcceleration = 8.0f;
        
        
        [Header("Wander Settings")]
        [SerializeField] private Vector3 _mapCentre;
        [SerializeField] private Vector3 _mapExtents;


        [Header("Searching Settings")]



        [Header("Chase Settings")]
        [SerializeField] private float _chaseStartTime = 0.75f;
        private float _chaseReadyTime;

        [Space(5)]
        [SerializeField] private float _chaseMovementSpeed = 4.5f;
        [SerializeField] private float _chaseAcceleration = 8.0f;


        [Header("Lay In Wait Settings")]
        [SerializeField] private float _trapDetectionRadius = 10.0f; // How close the agent must be to a potential trap spot to consider laying a trap.
        [SerializeField] private float _maxTrapTime = 0.0f; // The maximum time that the agent can be preparing a trap.


        [Header("Venting Settings")]
        [SerializeField] private float _minTimeBetweenVents = 10.0f; // The minimum time between the agent entering the vents.
        private float _lastVentExitTime;



        private void Awake()
        {
            _agent = GetComponent<NavMeshAgent>();
            _entitySenses = GetComponent<EntitySenses>();
            _flashlightStunnableScript = GetComponent<FlashlightStunnable>();
        }


        private void Update()
        {
            DetermineActiveState();
            SetAgentVariables();
            HandleActiveState();
        }


        private void DetermineActiveState()
        {
            if (_flashlightStunnableScript != null && _flashlightStunnableScript.IsStunned)
            {
                // We are currently stunned.
                _currentState = State.Stunned;
                return;
            }
            
            if (_entitySenses.HasTarget || (_currentState == State.Chase && _agent.remainingDistance > 1.0f))
            {
                // We can see the player.
                if (_currentState != State.Chase)
                {
                    _chaseReadyTime = Time.time + _chaseStartTime;
                    _agent.isStopped = true;
                }
                    
                _currentState = State.Chase;
                return;
            }
            
            _agent.isStopped = false;

            // We cannot see the player.

            if (_entitySenses.CurrentPointOfInterest.HasValue)
            {
                // We have a current Point of Interest.
                _currentState = State.Search;
                return;
            }

            // Trap | Vent | Wander.
            _currentState = State.Wander;
        }
        private void HandleActiveState()
        {
            switch (_currentState)
            {
                case State.Wander:
                    HandleWanderBehaviour();
                    break;
                case State.Search:
                    HandleSearchBehaviour();
                    break;
                case State.Chase:
                    HandleChaseBehaviour();
                    break;
                case State.SetTrap:
                    HandlePrepareTrapBehaviour();
                    break;
                case State.Vent:
                    HandleVentBehaviour();
                    break;
            }
        }
        private void SetAgentVariables()
        {
            switch (_currentState)
            {
                case State.Chase:
                    _agent.speed = _chaseMovementSpeed;
                    _agent.acceleration = _chaseAcceleration;
                    break;
                default:
                    _agent.speed = _defaultMovementSpeed;
                    _agent.acceleration = _defaultAcceleration;
                    break;
            }
        }


        #region Handle 'State' Functions

        /// <summary> Makes the Agent chase the player.</summary>
        private void HandleChaseBehaviour()
        {
            if (_chaseReadyTime > Time.time)
            {
                _agent.isStopped = true;
            }
            else
            {
                _agent.isStopped = false;
            }
            

            // Move towards the player.
            _agent.SetDestination(_entitySenses.TargetPosition);
        }
        /// <summary> Makes the Agent investigate the current Point of Interest.</summary>
        private void HandleSearchBehaviour()
        {
            _agent.SetDestination(_entitySenses.CurrentPointOfInterest.Value);

            if (_agent.remainingDistance < 0.5f)
            {
                // Don't keep pathing to the same POI if we've discovered it.
                _entitySenses.ClearPointOfInterest();
            }
        }
        /// <summary> Makes the Agent randomly wander the level.</summary>
        private void HandleWanderBehaviour()
        {
            if (_agent.remainingDistance >= 2.0f)
            {
                // We haven't yet reached our desired wander destination.
                return;
            }

            // We've reached our desired wander destination.
            // Pick a new destination.
            if (TryFindRandomPoint(_mapCentre, _mapExtents, out Vector3 result))
            {
                _agent.SetDestination(result);
            }
        }
        /// <summary> Makes the agent move to a hidden position until the player moves past or it gets bored.</summary>
        private void HandlePrepareTrapBehaviour()
        {

        }
        /// <summary> Makes the agent move to the nearest vent and enter it. If in a vent, it instead moves around the level.</summary>
        private void HandleVentBehaviour()
        {

        }

        #endregion


        private bool TryFindRandomPoint(Vector3 centre, float maxRadius, out Vector3 result, int maxAttempts = 5)
        {
            Vector3 randomPoint;
            NavMeshHit hit;
            for (int i = 0; i < maxAttempts; i++)
            {
                randomPoint = centre + (Random.insideUnitSphere * maxRadius);
                if (NavMesh.SamplePosition(randomPoint, out hit, 1.0f, _agent.areaMask))
                {
                    // We found a suitable point on the navmesh.
                    result = hit.position;
                    return true;
                }
            }

            // We couldn't find a suitable point on the navmesh.
            result = Vector3.zero;
            return false;
        }
        private bool TryFindRandomPoint(Vector3 centre, Vector3 extents, out Vector3 result, int maxAttempts = 5)
        {
            Vector3 randomPoint;
            NavMeshHit hit;
            for(int i = 0; i < maxAttempts; i++)
            {
                randomPoint = centre + new Vector3(
                    x: Random.Range(-extents.x, extents.x),
                    y: Random.Range(-extents.y, extents.y),
                    z: Random.Range(-extents.z, extents.z));

                if (NavMesh.SamplePosition(randomPoint, out hit, 1.0f, _agent.areaMask))
                {
                    // We found a suitable point on the navmesh.
                    result = hit.position;
                    return true;
                }
            }

            // We couldn't find a suitable point on the navmesh.
            result = Vector3.zero;
            return false;
        }


        private void OnDrawGizmosSelected()
        {
            Gizmos.DrawWireCube(_mapCentre, _mapExtents);
        }
    }
}