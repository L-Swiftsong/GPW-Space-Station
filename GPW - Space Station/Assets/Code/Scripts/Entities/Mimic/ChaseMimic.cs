using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Entities.Player;
using Audio;


namespace Entities.Mimic
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class ChaseMimic : MonoBehaviour, IStunnable
    {
        private NavMeshAgent _navMeshAgent;
        private bool isChasing = false;
        private bool _hasStartedChase = false;


        [Header("Movement Settings")]
        [SerializeField, ReadOnly] private float _targetSpeed;
        [SerializeField] private float _movementLerpRate = 3.0f;

        [Space(5)]
        [SerializeField] private AnimationCurve _chaseSpeedCurve = AnimationCurve.Linear(5.0f, 5.0f, 10.0f, 7.0f);
        [SerializeField] [Range(0.0f, 1.0f)] private float _stunnedMovementMultiplier = 0.7f;
        private bool _isBeingStunned = false;


        [Header("Audio Settings")]
        [SerializeField] private Audio.BackgroundMusicManager.AudioClipSettings _chaseSFX;
        [SerializeField] private float _defaultBGMResumeDelay = 2.0f;

        [Space(5)]
        [SerializeField] private AudioClip _breakDoorClip;
        [SerializeField] private AudioClip _chaseEndClip;


        private static System.Action<bool> OnPauseAllChases;
        private static System.Action OnResumeAllChases;
        private static System.Action OnEndAllChases;


        private void Start()
        {
            _navMeshAgent = GetComponent<NavMeshAgent>();
            _navMeshAgent.speed = _chaseSpeedCurve.Evaluate(0.0f);
            _navMeshAgent.updateRotation = true;
        }
        private void OnEnable()
        {
            ChaseMimic.OnPauseAllChases += PauseChase;
            ChaseMimic.OnResumeAllChases += ResumeChase;
            ChaseMimic.OnEndAllChases += EndChase;
        }
        private void OnDisable()
        {
            ChaseMimic.OnPauseAllChases -= PauseChase;
            ChaseMimic.OnResumeAllChases -= ResumeChase;
            ChaseMimic.OnEndAllChases -= EndChase;
        }

        private void Update()
        {
            if (!isChasing || PlayerManager.Instance.Player == null)
            {
                return;
            }

            _navMeshAgent.SetDestination(PlayerManager.Instance.Player.position);
                
            float distanceToPlayer = Vector3.Distance(transform.position, PlayerManager.Instance.Player.position);
            _targetSpeed = _chaseSpeedCurve.Evaluate(distanceToPlayer);
            if (_isBeingStunned)
                _targetSpeed *= _stunnedMovementMultiplier;

            _navMeshAgent.speed = Mathf.MoveTowards(_navMeshAgent.speed, _targetSpeed, _movementLerpRate * Time.deltaTime);
        }
        private void LateUpdate() => _isBeingStunned = false;


        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.CompareTag("BreakDoor"))
            {
                Destroy(collision.gameObject);
                Debug.Log("BreakDoor destroyed!");

                if (_breakDoorClip != null)
                {
                    SFXManager.Instance.PlayClipAtPosition(_breakDoorClip, transform.position);
                }
            }
        }
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("BreakDoor"))
            {
                Destroy(other.gameObject);
                Debug.Log("Trigger BreakDoor destroyed!");

                if (_breakDoorClip != null)
                {
                    SFXManager.Instance.PlayClipAtPosition(_breakDoorClip, transform.position);
                }
            }
        }



        #region Chase Starting, Pausing, & Ending

        public static void PauseAllChases(bool pauseMusic) => OnPauseAllChases?.Invoke(pauseMusic);
        public static void ResumeAllChases() => OnResumeAllChases?.Invoke();
        public static void EndAllChases() => OnEndAllChases?.Invoke();


        public void StartChase()
        {
            if (isChasing)
            {
                // This mimic has already started chasing the player.
                return;
            }

            isChasing = true;
            _hasStartedChase = true;
            Debug.Log("Chase Activated!");

            BackgroundMusicManager.OverrideBackgroundMusic(_chaseSFX);
        }
        public void PauseChase(bool pauseMusic)
        {
            if (!_hasStartedChase)
            {
                // This mimic hasn't started chasing yet.
                return;
            }

            isChasing = false;
            Debug.Log("Chase Paused");

            if (pauseMusic)
            {
                BackgroundMusicManager.PauseBackgroundMusic();
            }
            _navMeshAgent.isStopped = true;
        }
        public void ResumeChase()
        {
            if (!_hasStartedChase)
            {
                // This mimic hasn't started chasing yet.
                return;
            }

            isChasing = true;
            Debug.Log("Chase Resumed");

            BackgroundMusicManager.ResumeBackgroundMusic();
            _navMeshAgent.isStopped = false;
        }
        public void EndChase()
        {
            if (!_hasStartedChase)
            {
                // This mimic hasn't started chasing yet.
                return;
            }

            if (_chaseEndClip != null)
            {
                BackgroundMusicManager.RemoveBackgroundMusicOverride();
                BackgroundMusicManager.PlaySingleClip(_chaseEndClip);
            }
            else
            {
                BackgroundMusicManager.RemoveBackgroundMusicOverride(initialTransitionDelay: _defaultBGMResumeDelay);
            }

            Debug.Log("Chase Ended");
            Destroy(gameObject);
        }

        #endregion


        public void ApplyStun(float stunStrengthDelta) => _isBeingStunned = true;


        private void OnDrawGizmosSelected()
        {
            if (PlayerManager.Exists && PlayerManager.Instance.Player != null)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawLine(transform.position, PlayerManager.Instance.Player.position);
            }

            Gizmos.color = Color.red;
            foreach (Keyframe key in _chaseSpeedCurve.keys)
            {
                Gizmos.DrawWireSphere(transform.position, key.time);
            }
        }
    }
}