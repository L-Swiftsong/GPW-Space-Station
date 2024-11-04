using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using UnityEditor.VersionControl;
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
        private PassiveMimicryController _passiveMimicryController;


        [System.Serializable] private enum State { Wander, Search, Chase, SetTrap, Vent, Stunned };
        private State _currentState = State.Wander;



        [Header("Default Settings")]
        [SerializeField] private float _defaultMovementSpeed = 2.0f;
        [SerializeField] private float _defaultAcceleration = 8.0f;
        
        
        [Header("Wander Settings")]
        [SerializeField] private Vector3 _mapCentre;
        [SerializeField] private Vector3 _mapExtents;

        [Space(5)]
        [SerializeField] private float _minWanderDecisionTime = 1.0f;
        [SerializeField] private float _maxWanderDecisionTime = 15.0f;
        private float _wanderDecisionTimeRemaining = 0.0f;


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
        private float _currentTrapTime;
        private TrapPoint _targetTrapPoint;


        [Header("Venting Settings")]
        [SerializeField] private float _maxVentDetectionRadius = 5.0f;
        [SerializeField] private float _minTimeBetweenVents = 10.0f; // The minimum time between the agent entering the vents.
        private float _ventCooldownRemaining;

        private Vent _targetVent;
        private VentEntrance _targetEntrance;
        private bool _isInVent = false;



        private void Awake()
        {
            _agent = GetComponent<NavMeshAgent>();
            _entitySenses = GetComponent<EntitySenses>();
            _flashlightStunnableScript = GetComponent<FlashlightStunnable>();
            _passiveMimicryController = GetComponent<PassiveMimicryController>();


            // Start in the wander state.
            _currentState = State.Wander;
            _wanderDecisionTimeRemaining = Random.Range(_minWanderDecisionTime, _maxWanderDecisionTime);
        }


        private void Update()
        {
            DetermineActiveState();
            SetAgentVariables();
            HandleActiveState();
            UpdateMimicryRenderer();


            _ventCooldownRemaining -= Time.deltaTime;
            _wanderDecisionTimeRemaining -= Time.deltaTime;
            _currentTrapTime += Time.deltaTime;
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
                if (_currentState != State.Chase || _currentState != State.SetTrap)
                {
                    // We just entered the Chase State from a state where we didn't expect to see the player.
                    // Add a delay before we start properly chasing the player.
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

            if (_currentState == State.SetTrap)
            {
                return;
            }
            if (_currentState == State.Vent)
            {
                return;
            }


            if (_wanderDecisionTimeRemaining <= 0.0f)
            {
                float _rndBehaviourDecision = Random.Range(0.0f, 1.0f);

                float ventChance = 0.2f;
                float setTrapChance = 0.2f;
                if (_rndBehaviourDecision <= ventChance && _ventCooldownRemaining <= 0.0f)
                {
                    TryEnterVentState();
                    return;
                }
                else if (_rndBehaviourDecision <= (ventChance + setTrapChance))
                {
                    TryEnterTrapState();
                    return;
                }

                _currentState = State.Wander;
                _wanderDecisionTimeRemaining = Random.Range(_minWanderDecisionTime, _maxWanderDecisionTime);
                return;
            }
            else
            {
                _currentState = State.Wander;
            }
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
        private void UpdateMimicryRenderer()
        {
            float mimicryStrength = _currentState switch
            {
                State.Chase => 0.0f,
                _ => 1.0f,
            };

            _passiveMimicryController.SetMimicryStrengthTarget(mimicryStrength);
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
            if (_agent.TryFindRandomPoint(_mapCentre, _mapExtents, out Vector3 result))
            {
                _agent.SetDestination(result);
            }
        }
        /// <summary> Makes the agent move to a hidden position until the player moves past or it gets bored.</summary>
        private void HandlePrepareTrapBehaviour()
        {
            if (_agent.remainingDistance > 0.2f)
            {
                // We haven't reached the target trap.
                _currentTrapTime = 0.0f;
                return;
            }

            // We have reached the target trap.
            // Ensure we are at the trap's position.
            _agent.isStopped = true;
            transform.position = _targetTrapPoint.transform.position;

            // Rotate to face the trap's forward.
            transform.rotation = Quaternion.RotateTowards(_agent.transform.rotation, Quaternion.LookRotation(_targetTrapPoint.transform.forward, transform.up), _agent.angularSpeed * Time.deltaTime);

            if (_currentTrapTime >= _maxTrapTime)
            {
                // We have exceeded our maximum trap time. Stop waiting for the player.
                _currentState = State.Wander;
            }
        }
        /// <summary> Makes the agent move to the nearest vent and enter it. If in a vent, it instead moves around the level.</summary>
        private void HandleVentBehaviour()
        {
            if ((transform.position - _targetEntrance.transform.position).sqrMagnitude <= 0.5f)
            {
                if (_isInVent)
                {
                    // We have reached the vent exit.
                    // Exit the vent.
                    _isInVent = false;
                    _currentState = State.Wander;
                }
                else
                {
                    // We have reached our target vent.
                    _isInVent = true;

                    // Choose a random vent entrance (Which is not our current entrance, unless said entrance is Omnidirectional).
                    int maxIndex = _targetVent.VentEntrances.Count - (_targetEntrance.IsOmnidirectional ? 0 : 1);
                    int randomIndex = Random.Range(0, maxIndex);

                    VentEntrance newTarget = _targetVent.VentEntrances[randomIndex];
                    _targetEntrance = ((!_targetEntrance.IsOmnidirectional && newTarget != _targetEntrance) ? newTarget : (_targetVent.VentEntrances[randomIndex == 0 ? 1 : randomIndex - 1]));


                    // (Temp) Move towards the vent exit.
                    _agent.enabled = false;
                    transform.position = _targetEntrance.transform.position;
                    _agent.enabled = true;
                    _ventCooldownRemaining = _minTimeBetweenVents;
                }
            }
        }


        [ContextMenu(itemName: "Test Trap State")]
        private void TryEnterTrapState()
        {
            // Reset previous references.
            _targetTrapPoint = null;

            Debug.Log("Attempt Enter Trap State");

            // Find all nearby TrapPoints.
            List<TrapPoint> nearbyTrapPoints = TrapManager.GetTrapPointsWithinRange(transform.position, _trapDetectionRadius);

            if (nearbyTrapPoints.Count <= 0)
            {
                // There were no trap points in range. We cannot enter the SetTrap state.
                Debug.Log("Trap State Failure");
                return;
            }

            // Choose a random trapPoint for our target.
            _targetTrapPoint = nearbyTrapPoints[Random.Range(0, nearbyTrapPoints.Count)];
            _agent.SetDestination(_targetTrapPoint.transform.position);
            _currentTrapTime = 0.0f;
            _currentState = State.SetTrap;
            Debug.Log("Trap State Success");
        }


        private void TryEnterVentState()
        {
            // Reset previous references.
            _targetVent = null;
            _targetEntrance = null;
            _isInVent = false;

            Debug.Log("Attempt Enter Vent State");


            // Get the closest Vent Entrance (Based on NavMesh distance, not Euclidean distance).
            List<VentEntrance> ventEntrances = new List<VentEntrance>();
            foreach(Collider collider in Physics.OverlapSphere(transform.position, _maxVentDetectionRadius))
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
                if (!_agent.TryCalculateSqrDistanceToPoint(ventEntrances[i].transform.position, out float sqrDistance, ref path))
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
                // We were unable to find a path to a valid entrance.
                _currentState = State.Wander;
                Debug.Log("Vent State Failure");
                return;
            }

            _targetVent = _targetEntrance.GetComponentInParent<Vent>();
            _agent.SetDestination(_targetEntrance.transform.position);
            _currentState = State.Vent;
            Debug.Log("Vent State Success");
        }

        #endregion


        private void OnDrawGizmosSelected()
        {
            Gizmos.DrawWireCube(_mapCentre, _mapExtents);
        }
    }
}