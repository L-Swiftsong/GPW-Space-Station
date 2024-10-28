using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField]
    private Transform pointA;

    [SerializeField]
    private Transform pointB;

    [SerializeField]
    private float speed = 2f;

    [SerializeField]
    private float rotationSpeed = 5f;

    [SerializeField]
    private Transform player;

    [SerializeField]
    private float soundDistance = 10f;

    private AudioSource[] audioSources;
    private Transform target;
    private bool isAudioPlaying = false;

    void Start()
    {
        target = pointA;
        audioSources = GetComponents<AudioSource>();

        if (audioSources.Length >= 2)
        {
            audioSources[0].Stop();
            audioSources[1].Stop();
        }
        else
        {
            Debug.LogWarning("Not enough AudioSources attached to the enemy!");
        }
    }

    void Update()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= soundDistance && !isAudioPlaying)
        {
            isAudioPlaying = true;
            if (audioSources.Length >= 2)
            {
                audioSources[0].Play();
                audioSources[1].Play();
            }
        }
        else if (distanceToPlayer > soundDistance && isAudioPlaying)
        {
            isAudioPlaying = false;
            if (audioSources.Length >= 2)
            {
                audioSources[0].Stop();
                audioSources[1].Stop();
            }
        }

        transform.position = Vector3.MoveTowards(transform.position, target.position, speed * Time.deltaTime);

        Vector3 direction = (target.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, rotationSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, target.position) < 0.1f)
        {
            if (target == pointA)
            {
                target = pointB;
            }
            else
            {
                target = pointA;
            }
        }
    }
}
