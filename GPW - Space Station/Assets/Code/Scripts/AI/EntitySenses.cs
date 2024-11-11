using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace AI
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


        [Header("Hearing")]
        [SerializeField] private float _hearingSensitivityMultiplier;
        private NavMeshAgent _agent;
        private Vector3? _pointOfInterest = null;

        private static event System.Action<Vector3, float> OnSoundTriggered;


        #region Properties

        public bool HasTarget => _canSeePlayer;
        public Vector3 TargetPosition => _player != null ? _player.position : Vector3.zero;

        public Vector3? CurrentPointOfInterest => _pointOfInterest;

        #endregion



        private void Start()
        {
            _player = PlayerManager.Exists ? PlayerManager.Instance.Player : FindObjectOfType<PlayerController>().transform;
            _playerTargetableObject = _player.GetComponent<TargetableObject>();

            _agent = GetComponent<NavMeshAgent>();
        }
        private void OnEnable() => OnSoundTriggered += EntitySenses_OnSoundTriggered;
        private void OnDisable() => OnSoundTriggered -= EntitySenses_OnSoundTriggered;


        private void Update()
        {
            if (_player == null)
            {
                if (PlayerManager.Exists)
                    _player = PlayerManager.Instance.Player;
                else
                    return;
            }
            
            _canSeePlayer = (TryFindTarget() != null);
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

                if (Vector3.Angle(_headTransform.forward, directionToTargetableObject) > (_viewAngle / 2.0f))
                {
                    // Point outwith viewcone.
                    Debug.DrawRay(_headTransform.position, directionToTargetableObject, Color.yellow);
                    continue;
                }


                // To-do: Add detection time based on the number of seen Detection Targets.
                return _player;
            }

            return null;
        }

        #endregion


        #region Sound Detection

        private void EntitySenses_OnSoundTriggered(Vector3 soundOrigin, float soundVolume)
        {
            if (TryDetectSound(soundOrigin, soundVolume))
            {
                _pointOfInterest = soundOrigin;
            }
        }
        private bool TryDetectSound(Vector3 soundOrigin, float volume)
        {
            NavMeshPath path = new NavMeshPath();
            if (!NavMesh.CalculatePath(soundOrigin, _agent.transform.position, _agent.areaMask, path))
            {
                // No possible path to this object.
                Debug.Log("No path to sound originating at position " + soundOrigin);
                return false;
            }

            volume *= _hearingSensitivityMultiplier;
            float sqrMaxHearingDistance = volume * volume;
            if (CalculatePathSqrLength(path) > sqrMaxHearingDistance)
            {
                // Sound is too far away.
                Debug.Log("The sound originating at position " + soundOrigin + " was too far away to be heard");
                return false;
            }

            // Sound is within detection range.
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


        public static void SoundTriggered(Vector3 origin, float volume) => OnSoundTriggered?.Invoke(origin, volume);

        #endregion


        public void ClearPointOfInterest() => _pointOfInterest = null;


        private void OnDrawGizmos()
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