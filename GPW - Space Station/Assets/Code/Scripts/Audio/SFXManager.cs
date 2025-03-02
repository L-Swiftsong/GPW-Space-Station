using Entities.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.Audio;

namespace Audio
{
    public class SFXManager : MonoBehaviour
    {
        private static SFXManager s_instance;
        public static SFXManager Instance
        {
            get => s_instance;
            set
            {
                if (s_instance != null)
                {
                    Debug.LogError($"Error: A PlayerManager instance already exists: {Instance.name}. \nDestroying {value.name} ");
                    Destroy(value.gameObject);
                    return;
                }

                s_instance = value;
            }
        }


        private ObjectPool<AudioSource> _audioSourcePool;
        private const int AUDIO_SOURCE_POOL_CAPACITY = 10;
        private const int AUDIO_SOURCE_POOL_MAX_SIZE = 30;

        [SerializeField] private AudioMixerGroup _sfxMixerGroup;


        public static event System.Action<Vector3, float> OnDetectableSoundTriggered;


        private void Awake()
        {
            Instance = this;
            _audioSourcePool = new ObjectPool<AudioSource>(createFunc: CreateNewSource, actionOnGet: OnGetSource, actionOnRelease: OnReleaseSource, defaultCapacity: AUDIO_SOURCE_POOL_CAPACITY, maxSize: AUDIO_SOURCE_POOL_MAX_SIZE);
        }


        private AudioSource CreateNewSource()
        {
            // Create the AudioSource.
            GameObject sourceGameObject = new GameObject("AudioSource");
            sourceGameObject.transform.parent = transform;
            AudioSource audioSource = sourceGameObject.AddComponent<AudioSource>();

            // Set default values for the AudioSource.
            audioSource.volume = 1.0f;
            audioSource.pitch = 1.0f;

            audioSource.spatialBlend = 1.0f;

            audioSource.dopplerLevel = 1.0f;
            audioSource.spread = 0.0f;
            audioSource.minDistance = 500.0f;
            audioSource.maxDistance = 1.0f;

            audioSource.outputAudioMixerGroup = _sfxMixerGroup;


            return audioSource;
        }
        private void OnGetSource(AudioSource audioSource)
        {
            audioSource.enabled = true;
        }
        private void OnReleaseSource(AudioSource audioSource)
        {
            audioSource.enabled = false;
        }

        public void PlayClipAtPosition(AudioClip clip, Vector3 position, float minPitch = 1.0f, float maxPitch = 1.0f, float volume = 1.0f,
            float dopplerLevel = 1.0f, float spread = 0.0f, float minDistance = 1.0f, float maxDistance = 500.0f, AnimationCurve falloffCurve = null)
        {
            AudioSource audioSource = _audioSourcePool.Get();

            
            // Setup the AudioSource.
            audioSource.transform.position = position;
            audioSource.clip = clip;
            
            audioSource.volume = volume;
            audioSource.pitch = Random.Range(minPitch, maxPitch);
            
            audioSource.dopplerLevel = dopplerLevel;
            audioSource.spread = spread;

            audioSource.minDistance = minDistance;
            audioSource.maxDistance = maxDistance;
            if (falloffCurve != null)
            {
                audioSource.rolloffMode = AudioRolloffMode.Custom;
                audioSource.SetCustomCurve(AudioSourceCurveType.CustomRolloff, falloffCurve);
            }
            else
                audioSource.rolloffMode = AudioRolloffMode.Logarithmic;


            // Start the Clip Playing.
            audioSource.Play();

            // Release the AudioSource once the clip has finished.
            StartCoroutine(ReleaseOnAudioFinish(audioSource));
        }
        public void PlayDetectableClipAtPosition(AudioClip clip, Vector3 position, float detectionRadius, float minPitch = 1.0f, float maxPitch = 1.0f, float volume = 1.0f,
            float dopplerLevel = 1.0f, float spread = 0.0f, float minDistance = 1.0f, float maxDistance = 500.0f, AnimationCurve falloffCurve = null)
        {
            PlayClipAtPosition(clip, position, minPitch: minPitch, maxPitch: maxPitch, volume: volume, dopplerLevel: dopplerLevel, spread: spread, minDistance: minDistance, maxDistance: maxDistance);
            TriggerDetectableSoundAtPoint(position, detectionRadius);
        }
        public void TriggerDetectableSoundAtPoint(Vector3 origin, float detectionRadius) => OnDetectableSoundTriggered?.Invoke(origin, detectionRadius);

        private IEnumerator ReleaseOnAudioFinish(AudioSource audioSource)
        {
            // Wait until we start playing.
            yield return new WaitUntil(() => audioSource.isPlaying);

            // Wait until we stop playing.
            yield return new WaitUntil(() => !audioSource.isPlaying);

            // Release the audio source.
            _audioSourcePool.Release(audioSource);
        }


        public struct DetectableClipParameters
        {
            public Vector3 Origin;
            public float DetectabilityRadius;
        }
    }
}