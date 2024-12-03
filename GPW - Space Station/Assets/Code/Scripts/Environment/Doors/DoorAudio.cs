using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Environment.Doors
{
    [RequireComponent(typeof(Door), typeof(AudioSource))]
    public class DoorAudio : MonoBehaviour
    {
        private Door _door;
        private AudioSource _audioSource;


        [Header("Sound Clips")]
        [SerializeField] private AudioClip _doorOpenClip;
        [SerializeField] private AudioClip _doorCloseClip;

        
#if UNITY_EDITOR
        private void Reset()
        {
            // Create or get access to this door's AudioSource.
            if (!TryGetComponent<AudioSource>(out _audioSource))
            {
                _audioSource = gameObject.AddComponent<AudioSource>();
            }

            // Setup this door's audioSource.
            _audioSource.playOnAwake = false;
            _audioSource.loop = false;
            _audioSource.spatialBlend = 1.0f;
            _audioSource.rolloffMode = AudioRolloffMode.Logarithmic;
            _audioSource.minDistance = 3.0f;
            _audioSource.maxDistance = 40.0f;
        }
#endif


        private void Awake()
        {
            _door = GetComponent<Door>();
            _audioSource = GetComponent<AudioSource>();
        }
        private void OnEnable() => _door.OnOpenStateChanged += Door_OnOpenStateChanged;
        private void OnDisable() => _door.OnOpenStateChanged -= Door_OnOpenStateChanged;


        private void Door_OnOpenStateChanged(bool newValue)
        {
            if (newValue)
            {
                PlayClip(_doorOpenClip);
            }
            else
            {
                PlayClip(_doorCloseClip);
            }
        }

        private void PlayClip(AudioClip clip, bool overridePrevious = true)
        {
            if (clip == null)
            {
                // Invalid clip.
                return;
            }

            if (overridePrevious)
            {   
                // Stop the previous opening/closing audio.
                _audioSource.Stop();
            }

            // Play the opening/closing audio (With a pitch variation to increase variability and reduce audio fatigue).
            _audioSource.clip = clip;
            _audioSource.pitch = Random.Range(0.95f, 1.05f);
            _audioSource.Play();
        }
    }
}