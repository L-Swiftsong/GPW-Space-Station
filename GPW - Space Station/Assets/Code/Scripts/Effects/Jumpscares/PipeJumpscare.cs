using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Effects.Jumpscares
{
    public class PipeJumpscare : MonoBehaviour
    {
        public ParticleSystem SteamParticleSystem;
        public float steamDuration = 10f;
        public float fadeDuration = 2f;

        private bool isActive = false;

        [SerializeField] private AudioClip steamAudio;
        [SerializeField] private float maxVolume = 0.035f;
        [SerializeField] private float fadeOutVolumeDuration = 2f;

        [SerializeField] private AudioSource _audioSource;

        private void Start()
        {
            _audioSource.clip = steamAudio;
            _audioSource.loop = true;
            _audioSource.playOnAwake = false;
            _audioSource.volume = maxVolume;
        }


        public void StartJumpScare()
        {
            if (SteamParticleSystem != null)
            {
                isActive = true;
                StartCoroutine(EnableSteam());
            }
        }

        IEnumerator EnableSteam()
        {
            _audioSource.Play();

            yield return new WaitForSeconds(0.5f); // This is just to sync better with audio

            SteamParticleSystem.Play();

            yield return new WaitForSeconds(steamDuration);

            var mainModule = SteamParticleSystem.main;
            float startLifetime = mainModule.startLifetime.constant;
            float elapsedTime = 0f;

            while (elapsedTime < fadeDuration)
            {
                elapsedTime += Time.deltaTime;
                float progress = elapsedTime / fadeDuration;

                mainModule.startLifetime = Mathf.Lerp(startLifetime, 0f, progress);

                _audioSource.volume = Mathf.Lerp(maxVolume, 0f, progress);

                yield return null;
            }

            SteamParticleSystem.Stop();

            _audioSource.loop = false;
            _audioSource.Stop();
        }
    }
}