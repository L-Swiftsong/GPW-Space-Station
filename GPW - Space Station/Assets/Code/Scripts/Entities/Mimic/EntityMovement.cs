using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Unity.AI.Navigation;

namespace Entities
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class EntityMovement : MonoBehaviour
    {
        private NavMeshAgent _agent;
        private bool _onNavMeshLink;

        enum MovementState { Walking, Crouching, Crawling }
        private MovementState _movementState;
        private NavMeshLayers _currentLayers;


        [Header("Pathing Settings")]
        [SerializeField] private float _reachedDestinationDistance = 0.5f;


        [Header("Movement Settings")]
        [SerializeField] private float _defaultSpeed = 2.0f;
        [SerializeField] private float _defaultAcceleration = 120.0f;

        [SerializeField, ReadOnly] private float _speedOverride = -1.0f;
        [SerializeField, ReadOnly] private float _accelerationOverride = -1.0f;


        private float _baseSpeed => _speedOverride >= 0.0f ? _speedOverride : _defaultSpeed;
        private float _baseAcceleration => _speedOverride >= 0.0f ? _accelerationOverride : _defaultAcceleration;


        [Header("Crouching")]
        [SerializeField] private NavMeshLayers _crouchingLayers = NavMeshLayers.None; // To-Implement (E.g. Doors).
        [SerializeField] private float _crouchingSpeedMultiplier = 0.8f;


        [Header("Crawling")]
        [SerializeField] private NavMeshLayers _crawlingLayers = NavMeshLayers.Crawlable;
        [SerializeField] private float _crawlingSpeedMultiplier = 0.5f;


        [Header("(Temp) Entering Vent Behaviour")]
        [SerializeField] private AnimationCurve _ventEnterCurve;
        private float _ventEnterDuration => _ventEnterCurve.keys[_ventEnterCurve.length - 1].time;

        [SerializeField] private AnimationCurve _ventExitCurve;
        private float _ventExitDuration => _ventExitCurve.keys[_ventExitCurve.length - 1].time;



        [Header("(Temp) GFX")]
        [SerializeField] private Transform _head;
        [SerializeField] private float _defaultHeadHeight = 1.6f;
        [SerializeField] private float _crouchedHeadHeight = 0.8f;

        [Space(5)]
        [SerializeField] private Transform[] _gfxContainers;
        [SerializeField] private float _defaultGFXYScale = 1f;
        [SerializeField] private float _crouchedGFXYScale = 0.5f;

        [Space(10)]
        [SerializeField] private AnimationCurve _crouchHeightChangeCurve;


        private void Awake()
        {
            _agent = GetComponent<NavMeshAgent>();
            _agent.autoTraverseOffMeshLink = false;
            _defaultSpeed = _agent.speed;

            _movementState = MovementState.Walking;
        }

        private void Update()
        {
            if (_agent.isOnOffMeshLink && !_onNavMeshLink)
            {
                // We've just entered a NavMeshLink.
                StartNavMeshLinkMovement();
            }

            UpdateMovementState();
            UpdateSpeed();
            Temp_UpdateHeight();
        }


        private void UpdateMovementState()
        {
            // Determine the agent's currently occupied NavMeshLayers.
            DetermineOccupiedLayers();
            
            if ((_currentLayers & _crawlingLayers) != 0)
            {
                _movementState = MovementState.Crawling;
            }
            else
            {
                _movementState = MovementState.Walking;
            }
        }
        private void UpdateSpeed()
        {
            // Reset Speed.
            _agent.speed = _baseSpeed;
            _agent.acceleration = _baseAcceleration;

            // Apply MovementState Speed Multipliers.
            _agent.speed *= _movementState switch
            {
                MovementState.Crouching => _crouchingSpeedMultiplier,
                MovementState.Crawling => _crawlingSpeedMultiplier,
                _ => 1.0f,
            };
        }
        private void Temp_UpdateHeight()
        {
            float headHeight = _defaultHeadHeight;
            float yScale = _defaultGFXYScale;
            switch (_movementState)
            {
                case MovementState.Crawling:
                    headHeight = _crouchedHeadHeight;
                    yScale = _crouchedGFXYScale;
                    break;
            }


            _head.localPosition = new Vector3(0.0f, headHeight, 0.0f);
            for(int i = 0; i < _gfxContainers.Length; ++i)
            {
                _gfxContainers[i].localScale = new Vector3(1.0f, yScale, 1.0f);
            }
        }


        private void DetermineOccupiedLayers()
        {
            if (!_agent.SamplePathPosition(_agent.areaMask, 1.0f, out NavMeshHit hit))
            {
                // SamplePathPosition() call succeeded.
                _currentLayers = (NavMeshLayers)hit.mask;
            }
            else
            {
                // SamplePathPosition() failed.
                //throw new System.Exception($"{this.name}'s 'EntityMovement's SamplePathPosition call failed.");
            }
        }


        #region Vent NavMeshLink Movement
        // Ref: 'https://github.com/SunnyValleyStudio/Diablo-Like-Movement-in-Unity-using-AI-Navigation-package/blob/main/AgentMover.cs'.

        private void StartNavMeshLinkMovement()
        {
            _onNavMeshLink = true;

            NavMeshLink link = (NavMeshLink)_agent.navMeshOwner;
            PerformLinkMovement(link);
        }

        private void PerformLinkMovement(NavMeshLink link)
        {
            bool reverseDirection = CheckIfExitingVent(link);
            Vector3 targetPos = reverseDirection ? link.gameObject.transform.TransformPoint(link.startPoint) : link.gameObject.transform.TransformPoint(link.endPoint);

            Debug.Log("Exiting?: " + reverseDirection);
            StartCoroutine(MoveOnOffMeshLink(targetPos, reverseDirection));
        }
        /// <summary>
        ///     Determine whether the agent is closer to the start or end of a NavMeshLink, and therefore whether we are travelling from the Start to End or vice versa.
        /// </summary>
        /// <param name="link">The NavMeshLink that we are testing against.</param>
        /// <returns> True if we are moving from the End to the Start of the NavMeshLink. False if otherwise.</returns>
        private bool CheckIfExitingVent(NavMeshLink link)
        {
            Vector3 startPos = link.gameObject.transform.TransformPoint(link.startPoint);
            Vector3 endPos = link.gameObject.transform.TransformPoint(link.endPoint);

            float distanceAgentToStart = Vector3.Distance(_agent.transform.position, startPos);
            float distanceAgentToEnd = Vector3.Distance(_agent.transform.position, endPos);

            return distanceAgentToStart > distanceAgentToEnd;
        }

        private IEnumerator MoveOnOffMeshLink(Vector3 targetPos, bool reverseDirection)
        {
            float currentTime = 0.0f;
            float duration = reverseDirection ? _ventExitDuration : _ventEnterDuration;

            Vector3 agentStartPosition = _agent.transform.position;

            while (currentTime < duration)
            {
                // Calculate our lerpTime.
                currentTime += Time.deltaTime;
                float lerpTime = Mathf.Clamp01(currentTime / _ventEnterDuration);

                // Handle our position change.
                float positionLerpValue = reverseDirection ? _ventExitCurve.Evaluate(lerpTime) : _ventEnterCurve.Evaluate(lerpTime);
                _agent.transform.position = Vector3.Lerp(agentStartPosition, targetPos, positionLerpValue);

                // Ensure that our Y-position is always at the desired level.
                _agent.transform.position = new Vector3(_agent.transform.position.x, agentStartPosition.y, _agent.transform.position.z);


                // Handle our GFX Changes (Would be in our animation for the Mimic proper).
                float gfxLerpValue = _crouchHeightChangeCurve.Evaluate(reverseDirection ? 1.0f - lerpTime : lerpTime);
                Vector3 gfxLerpScale = new Vector3(1.0f, Mathf.Lerp(_defaultGFXYScale, _crouchedGFXYScale, gfxLerpValue), 1.0f);
                for(int i = 0; i < _gfxContainers.Length; ++i)
                {
                    _gfxContainers[i].localScale = gfxLerpScale;
                }

                yield return null;
            }

            // Finish our movement.
            _agent.CompleteOffMeshLink();

            // Ensure that our scale successfully reached our desired values.
            for (int i = 0; i < _gfxContainers.Length; ++i)
            {
                _gfxContainers[i].localScale = new Vector3(1.0f, reverseDirection ? _defaultGFXYScale : _crouchedGFXYScale, 1.0f);
            }

            // Allow ourself to enter a new NavMeshLink after a short delay.
            yield return new WaitForSeconds(0.1f);
            _onNavMeshLink = false;
        }

        #endregion



        public void SetSpeed(float newValue) => _defaultSpeed = newValue;
        public void SetAcceleration(float newValue) => _defaultAcceleration = newValue;


        // Overrides.
        public void SetSpeedOverride(float overrideValue) => _speedOverride = overrideValue;
        public void SetAccelerationOverride(float overrideValue) => _accelerationOverride = overrideValue;

        public void ResetSpeedOverride() => _speedOverride = -1.0f;
        public void ResetAccelerationOverride() => _accelerationOverride = -1.0f;
        public void ResetAllOverrides()
        {
            ResetSpeedOverride();
            ResetAccelerationOverride();
        }


        public bool SetDestination(Vector3 destination) => _agent.SetDestination(destination);
        public void SetIsStopped(bool newValue) => _agent.isStopped = newValue;

        public bool HasReachedDestination() => _agent.remainingDistance <= _reachedDestinationDistance;
        public bool TryFindRandomPointInBounds(Vector3 centre, Vector3 extents, out Vector3 result, int areaMask = -1, int maxAttempts = 5) => _agent.TryFindRandomPoint(centre, extents, out result, areaMask: areaMask, maxAttempts: maxAttempts);


        public void RotateToDirection(Vector3 direction) => RotateToDirection(direction, _agent.angularSpeed);
        public void RotateToDirection(Vector3 direction, float angularSpeed) => _agent.transform.rotation = Quaternion.RotateTowards(from: _agent.transform.rotation, to: Quaternion.LookRotation(direction, _agent.transform.up), maxDegreesDelta: angularSpeed * Time.deltaTime);
    }
}
