using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Audio
{
    public class BackgroundMusic : MonoBehaviour
    {
        public AudioClip soundEffect; 
        private AudioSource audioSource;

        // Start is called before the first frame update
        void Start()
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;  
            audioSource.clip = soundEffect;   
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
           
                audioSource.Play();
            }
        }
    }
}