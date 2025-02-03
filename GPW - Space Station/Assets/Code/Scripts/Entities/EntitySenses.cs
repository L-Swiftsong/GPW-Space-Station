using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Entities.Player;
using Audio;

namespace Entities
{
    public class EntitySenses : MonoBehaviour
    {
        private Transform _player;
        private TargetableObject _playerTargetableObject;
        private bool _canSeePlayer;

        
        [Header("Sight")]
        [SerializeField] private Transform _headTransform;
        [SerializeField] private float _maxSightRange;
        [SerializeField] private AnimationCurve _sightDetectionTimeCurve;

        [Space(5)]
        [SerializeField] private float _viewAngle;

        [Space(5)]
        [SerializeField] private LayerMask _detectableLayers;
        [SerializeField] private LayerMask _obstructionLayers;


        [Header("Sight Retention")]
        [SerializeField] private float _sightRetentionTime = 0.1f; // How long after losing sight of the player till the EntitySenses registers the player as out of view.
        private float _previousVisibleTime;

        [Space(5)]
        [SerializeField] private bool _ignoreViewAngleForSightLoss = true; // Should we ignore the viewing angle for when we're forgetting a target.


        [Header("Hearing")]
        [SerializeField] private float _hearingSensitivityMultiplier;
        private NavMeshAgent _agent;
        private Vector3? _pointOfInterest = null;


        [Header("Debug")]
        [SerializeField] private bool _drawViewconeGizmos = false;
        [SerializeField] private bool _displayHearingDebug = false;


        #region Properties

        public bool HasTarget => _canSeePlayer;
        public Vector3 TargetPosition => _player != null ? _player.position : Vector3.zero;

        public Vector3? CurrentPointOfInterest => _pointOfInterest;

        #endregion



        private void Awake()
        {
            _agent = GetComponent<NavMeshAgent>();

            // Ensure that we don't accidentally start being able to see the player.
            _previousVisibleTime = -(_sightRetentionTime + 0.1f);
        }
        private void Start()
        {
            _player = PlayerManager.Exists ? PlayerManager.Instance.Player : FindObjectOfType<PlayerController>().transform;
            _playerTargetableObject = _player.GetComponent<TargetableObject>();
        }
        private void OnEnable() => SFXManager.OnDetectableSoundTriggered += SFXManager_OnDetectableSoundTriggered;
        private void OnDisable() => SFXManager.OnDetectableSoundTriggered -= SFXManager_OnDetectableSoundTriggered;


        private void Update()
        {
            if (_player == null)
            {
                if (PlayerManager.Exists)
                    _player = PlayerManager.Instance.Player;
                else
                    return;
            }
            

            if (TryFindTarget() != null)
            {
                // The player is within our sight.
                _canSeePlayer = true;
                _previousVisibleTime = Time.time;
            }
            else if (_canSeePlayer && (Time.time - _previousVisibleTime) > _sightRetentionTime)
            {
                // We can no longer see the player, and our sight retention time has elapsed.
                _canSeePlayer = false;
            }
        }



        #region Sight

        private Transform TryFindTarget()
        {
            if ((_player.position - _headTransform.position).sqrMagnitude > (_maxSightRange * _maxSightRange))
            {
                // The player is out of our max sight range.
                // We don't need to perform any further checks.
                return null;
            }

            if (_playerTargetableObject.IsHidden)
            {
                // The player is hidden. We cannot see them.
                return null;
            }


            // Sight Strength Calculation.
            for (int i = 0; i < _playerTargetableObject.DetectionTargets.Count; i++)
            {
                Vector3 detectionTargetPosition = _playerTargetableObject.DetectionTargets[i].position;
                Vector3 directionToTargetableObject = (detectionTargetPosition - _headTransform.position).normalized;
                if (Physics.Linecast(_headTransform.position, detectionTargetPosition, _obstructionLayers))
                {
                    // Point Obstructed.
                    Debug.DrawRay(_headTransform.position, directionToTargetableObject, Color.red);
                    continue;
                }

                if (!_ignoreViewAngleForSightLoss || !_canSeePlayer)
                {
                    // We can't currently see the player OR we aren't ignoring angles when determining sight loss.
                    if (Vector3.Angle(_headTransform.forward, directionToTargetableObject) > (_viewAngle / 2.0f))
                    {
                        // Point outwith viewcone.
                        Debug.DrawRay(_headTransform.position, directionToTargetableObject, Color.yellow);
                        continue;
                    }
                }


                // To-do: Add detection time based on the number of seen Detection Targets.
                return _player;
            }

            return null;
        }

        #endregion


        #region Sound Detection

        private void SFXManager_OnDetectableSoundTriggered(Vector3 origin, float radius)
        {
            if (TryDetectSound(origin, radius))
            {
                _pointOfInterest = origin;
            }
        }
        private bool TryDetectSound(Vector3 soundOrigin, float detectionRadius)
        {
            detectionRadius *= _hearingSensitivityMultiplier;
            float sqrDetectionRadius = detectionRadius * detectionRadius;
            if ((transform.position - soundOrigin).sqrMagnitude > sqrDetectionRadius)
            {
                // Too far away.
                if (_displayHearingDebug)
                    Debug.Log($"The sound originating at position {soundOrigin} was too far away to be heard (Distance: {(transform.position - soundOrigin).magnitude})");
                return false;
            }


            NavMeshPath path = new NavMeshPath();
            if (!NavMesh.CalculatePath(soundOrigin, _agent.transform.position, NavMesh.AllAreas, path))
            {
                // No possible path to this object.
                if (_displayHearingDebug)
                    Debug.Log("No path to sound originating at position " + soundOrigin);
                return false;
            }

            float sqrPathLength = CalculatePathSqrLength(path);
            if (sqrPathLength > sqrDetectionRadius)
            {
                // Sound is too far away.
                if (_displayHearingDebug)
                    Debug.Log($"The sound originating at position {soundOrigin} was too far away to be heard (Path Distance: {Mathf.Sqrt(sqrPathLength)})");
                return false;
            }

            // Sound is within detection range.
            if (_displayHearingDebug)
                Debug.Log("A sound originating at position " + soundOrigin + " was detected");
            return true;
        }

        private float CalculatePathSqrLength(NavMeshPath path)
        {
            if (path == null || path.corners.Length == 0)
            {
                // Invalid path for calculation.
                return 0.0f;
            }

            float sqrDistance = 0.0f;
            Vector3 previousPosition = path.corners[0];
            for (int i = 1; i < path.corners.Length; i++)
            {
                sqrDistance += (path.corners[i] - previousPosition).sqrMagnitude;
                previousPosition = path.corners[i];
            }

            return sqrDistance;
        }
        private float CalculatePathLength(NavMeshPath path) => Mathf.Sqrt(CalculatePathSqrLength(path));

        #endregion


        public void ClearPointOfInterest() => _pointOfInterest = null;


        private void OnDrawGizmos()
        {
            if (_drawViewconeGizmos)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(_headTransform.position, _maxSightRange);

                Vector3 leftAngle = Quaternion.AngleAxis(_viewAngle / 2.0f, Vector3.up) * _headTransform.forward;
                Vector3 rightAngle = Quaternion.AngleAxis(-_viewAngle / 2.0f, Vector3.up) * _headTransform.forward;
                Gizmos.DrawRay(_headTransform.position, leftAngle * _maxSightRange);
                Gizmos.DrawRay(_headTransform.position, rightAngle * _maxSightRange);
            }
        }
    }
}