using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Entities.Player;


namespace Entities.Mimic
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class ChaseMimic : MonoBehaviour
    {
        private NavMeshAgent _navMeshAgent;
        private bool isChasing = false;
        private bool _hasStartedChase = false;


        [Header("Movement Settings")]
        [SerializeField] private AnimationCurve _chaseSpeedCurve = AnimationCurve.Linear(5.0f, 5.0f, 10.0f, 7.0f);


        [Header("Audio Settings")]
        [SerializeField] private AudioClip _chaseSFX;
        [SerializeField] private AudioClip _breakDoorClip;
        [SerializeField] private AudioClip _chaseEndClip;

        [Header("Footstep Sounds")]
        [SerializeField] private AudioClip _footstepSound1;
        [SerializeField] private AudioClip _footstepSound2;
        [SerializeField] private float _footstepInterval = 0.5f;
        private float _footstepTimer = 0.0f;
        private bool _useFirstFootstep = true; // Toggle flag

        private AudioSource audioSource;
        private AudioSource audioSource2;


        private static System.Action<bool> OnPauseAllChases;
        private static System.Action OnResumeAllChases;
        private static System.Action OnEndAllChases;


        private void Start()
        {
            audioSource2 = gameObject.GetComponent<AudioSource>();
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;

            _navMeshAgent = GetComponent<NavMeshAgent>();
            _navMeshAgent.speed = _chaseSpeedCurve.Evaluate(0.0f);
            _navMeshAgent.updateRotation = true;


            if (_chaseSFX != null)
            {
                audioSource.clip = _chaseSFX;
            }

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
            if (isChasing && PlayerManager.Instance.Player != null)
            {
                _navMeshAgent.SetDestination(PlayerManager.Instance.Player.position);
                
                float distanceToPlayer = Vector3.Distance(transform.position, PlayerManager.Instance.Player.position);
                _navMeshAgent.speed = _chaseSpeedCurve.Evaluate(distanceToPlayer);

                HandleFootsteps();
            }
        }


        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.CompareTag("BreakDoor"))
            {
                Destroy(collision.gameObject);
                Debug.Log("BreakDoor destroyed!");

                if (_breakDoorClip != null)
                {
                    audioSource.PlayOneShot(_breakDoorClip);
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
                    audioSource.PlayOneShot(_breakDoorClip);
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

            audioSource.Play();
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
                audioSource.Pause();
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

            if (!audioSource.isPlaying)
                audioSource.Play();
            _navMeshAgent.isStopped = false;
        }
        public void EndChase()
        {
            if (!_hasStartedChase)
            {
                // This mimic hasn't started chasing yet.
                return;
            }

            audioSource.Stop();
            if (_chaseEndClip != null)
            {
                audioSource.PlayOneShot(_chaseEndClip);
            }

            Debug.Log("Chase Ended");
            Destroy(gameObject);
        }

        #endregion



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

        private void HandleFootsteps()
        {
            if (_navMeshAgent.velocity.magnitude > 0.1f)
            {
                _footstepTimer -= Time.deltaTime;

                if (_footstepTimer <= 0f)
                {
                    _footstepTimer = _footstepInterval;

                    // Alternate between the two footstep sounds
                    AudioClip footstepToPlay = _useFirstFootstep ? _footstepSound1 : _footstepSound2;
                    _useFirstFootstep = !_useFirstFootstep;

                    audioSource2.PlayOneShot(footstepToPlay);
                }
            }
        }
    }
}