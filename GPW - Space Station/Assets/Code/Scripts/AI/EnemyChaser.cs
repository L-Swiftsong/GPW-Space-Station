using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class EnemyChaser : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField]
    private float chaseSpeed = 5f;

    [Header("Audio Settings")]
    [SerializeField]
    private AudioClip chaseSFX;
    [SerializeField]
    private AudioClip breakDoorSFX;
    [SerializeField]
    private AudioClip finalDoorSFX;

    private AudioSource audioSource;
    private bool isChasing = false;
    private NavMeshAgent navMeshAgent;

    private void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;

        navMeshAgent = GetComponent<NavMeshAgent>();
        if (navMeshAgent != null)
        {
            navMeshAgent.speed = chaseSpeed;
            navMeshAgent.updateRotation = true;
        }
        else
        {
            Debug.LogError("NavMeshAgent component is missing on EnemyChaser.");
        }
    }

    private void Update()
    {
        if (isChasing && PlayerManager.Instance.Player != null)
        {
            navMeshAgent.SetDestination(PlayerManager.Instance.Player.position);
        }
    }

    public void ActivateChase()
    {
        if (!isChasing)
        {
            isChasing = true;
            Debug.Log("Chase activated!");

            if (chaseSFX != null && audioSource != null)
            {
                audioSource.clip = chaseSFX;
                audioSource.Play();
            }
        }
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
        else if (collision.gameObject.CompareTag("FinalDoor"))
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
        if (other.CompareTag("Player"))
        {
            // Restart the scene
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            Debug.Log("Enemy triggered with the player. Scene restarted.");
        }
        else if (other.CompareTag("BreakDoor"))
        {
            Destroy(other.gameObject);
            Debug.Log("Trigger BreakDoor destroyed!");

            if (breakDoorSFX != null && audioSource != null)
            {
                audioSource.PlayOneShot(breakDoorSFX);
            }
        }
        else if (other.CompareTag("FinalDoor"))
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
        if (PlayerManager.Exists && PlayerManager.Instance.Player != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, PlayerManager.Instance.Player.position);
        }
    }
}
