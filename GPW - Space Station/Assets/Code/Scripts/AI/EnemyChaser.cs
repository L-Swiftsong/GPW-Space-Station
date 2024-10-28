using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyChaser : MonoBehaviour
{
    [Header("Player Settings")]
    [SerializeField]
    private float detectionRange = 15f;

    [Header("Movement Settings")]
    [SerializeField]
    private float chaseSpeed = 5f;
    [SerializeField]
    private float rotationSpeed = 5f;

    [Header("Obstacle Detection")]
    [SerializeField]
    private LayerMask obstacleMask;

    [Header("Audio Settings")]
    [SerializeField]
    private AudioClip chaseSFX;
    [SerializeField]
    private AudioClip breakDoorSFX;
    [SerializeField]
    private AudioClip finalDoorSFX;

    private AudioSource audioSource;
    private bool isChasing = false;
    private bool playerDetected = false;

    private void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
    }

    private void Update()
    {
        if (isChasing && playerDetected)
        {
            ChasePlayer();
        }
        else
        {
            playerDetected = HasLineOfSightToPlayer();
            if (playerDetected)
            {
                StartChasing();
            }
        }
    }

    private bool HasLineOfSightToPlayer()
    {
        Vector3 directionToPlayer = PlayerManager.Instance.Player.position - transform.position;
        float distanceToPlayer = directionToPlayer.magnitude;

        if (distanceToPlayer <= detectionRange)
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, directionToPlayer.normalized, out hit, detectionRange, ~obstacleMask))
            {
                if (hit.transform == PlayerManager.Instance.Player)
                {
                    Debug.Log("Player detected!");
                    return true;
                }
            }
        }
        return false;
    }

    private void ChasePlayer()
    {
        Vector3 direction = (PlayerManager.Instance.Player.position - transform.position).normalized;
        direction.y = 0;

        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);

        transform.position += direction * chaseSpeed * Time.deltaTime;
    }

    public void StartChasing()
    {
        isChasing = true;
        Debug.Log("Chase started!");

        if (chaseSFX != null && audioSource != null)
        {
            audioSource.clip = chaseSFX;
            audioSource.Play();
        }
    }

    public void StopChasing()
    {
        isChasing = false;
        Debug.Log("Chase stopped!");
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("BreakDoor"))
        {
            Destroy(collision.gameObject);
            Debug.Log("BreakDoor destroyed!");

            if (breakDoorSFX != null && audioSource != null)
            {
                audioSource.PlayOneShot(breakDoorSFX);
            }
        }

        if (collision.gameObject.CompareTag("FinalDoor"))
        {
            if (finalDoorSFX != null && audioSource != null)
            {
                audioSource.PlayOneShot(finalDoorSFX);
            }

            Destroy(gameObject);
            Debug.Log("Enemy destroyed at FinalDoor!");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("BreakDoor"))
        {
            Destroy(other.gameObject);
            Debug.Log("Trigger BreakDoor destroyed!");

            if (breakDoorSFX != null && audioSource != null)
            {
                audioSource.PlayOneShot(breakDoorSFX);
            }
        }

        if (other.CompareTag("FinalDoor"))
        {
            if (finalDoorSFX != null && audioSource != null)
            {
                audioSource.PlayOneShot(finalDoorSFX);
            }

            Destroy(gameObject);
            Debug.Log("Enemy destroyed at FinalDoor via trigger!");
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        if (PlayerManager.Exists && PlayerManager.Instance.Player != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, PlayerManager.Instance.Player.position);
        }
    }
}
