using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioPlay : MonoBehaviour
{
    public AudioSource audioSource; 

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Mimic")) 
        {
            if (!audioSource.isPlaying) 
            {
                audioSource.PlayOneShot(audioSource.clip);
            }
        }
    }
}
