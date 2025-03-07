using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockedLocker : MonoBehaviour
{
    public AudioClip interactionSound;
    private AudioSource audioSource;

    void Start()
    {
        // Add an AudioSource component if not already present
        audioSource = gameObject.GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        audioSource.playOnAwake = false; // Ensure it doesn't play automatically
    }

    public void Interact()
    {
        if (interactionSound != null)
        {
            audioSource.PlayOneShot(interactionSound);
        }
    }
}
