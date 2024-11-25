using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Chase;
using Entities.Player;


namespace Entities.Mimic
{
    public class ChaseMimic : MonoBehaviour
    {
        [Header("Movement Settings")]
        [SerializeField] private float _chaseSpeed = 5f;

        [Header("Audio Settings")]
        [SerializeField] private AudioClip _chaseSFX;
        [SerializeField] private AudioClip _breakDoorClip;
        [SerializeField] private AudioClip _chaseEndClip;

        private AudioSource audioSource;
        private bool isChasing = false;
        private NavMeshAgent navMeshAgent;


        private static System.Action OnChaseStart;
        private static System.Action OnChaseEnd;


        private void Start()
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;

            navMeshAgent = GetComponent<NavMeshAgent>();
            if (navMeshAgent != null)
            {
                navMeshAgent.speed = _chaseSpeed;
                navMeshAgent.updateRotation = true;
            }
            else
            {
                Debug.LogError("NavMeshAgent component is missing on EnemyChaser.");
            }
        }
        private void OnEnable()
        {
            ChaseMimic.OnChaseStart += ActivateChaser;
            ChaseMimic.OnChaseEnd += DeactivateChaser;
        }
        private void OnDisable()
        {
            ChaseMimic.OnChaseStart -= ActivateChaser;
            ChaseMimic.OnChaseEnd -= DeactivateChaser;
        }

        private void Update()
        {
            if (isChasing && PlayerManager.Instance.Player != null)
            {
                navMeshAgent.SetDestination(PlayerManager.Instance.Player.position);
            }
        }


        public static void StartChase() => OnChaseStart?.Invoke();
        public static void EndChase() => OnChaseEnd?.Invoke();
        public void ActivateChaser()
        {
            if (!isChasing)
            {
                isChasing = true;
                Debug.Log("Chase activated!");

                if (_chaseSFX != null && audioSource != null)
                {
                    audioSource.clip = _chaseSFX;
                    audioSource.Play();
                }
            }
        }
        private void DeactivateChaser()
        {
            if (_chaseEndClip != null && audioSource != null)
            {
                audioSource.PlayOneShot(_chaseEndClip);
            }

            Debug.Log("Chase Ended");
            Destroy(gameObject);
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.CompareTag("BreakDoor"))
            {
                Destroy(collision.gameObject);
                Debug.Log("BreakDoor destroyed!");

                if (_breakDoorClip != null && audioSource != null)
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

                if (_breakDoorClip != null && audioSource != null)
                {
                    audioSource.PlayOneShot(_breakDoorClip);
                }
            }
        }


        private void OnDrawGizmosSelected()
        {
            if (PlayerManager.Exists && PlayerManager.Instance.Player != null)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawLine(transform.position, PlayerManager.Instance.Player.position);
            }
        }
    }
}