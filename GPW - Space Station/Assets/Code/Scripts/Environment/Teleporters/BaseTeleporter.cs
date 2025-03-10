using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Environment.Teleporters
{
    public abstract class BaseTeleporter : MonoBehaviour
    {
        [Header("General (Warmup)")]
        [SerializeField] private float _teleporterWarmupTime;
        private Coroutine _teleporterWarmupCoroutine;


        [Header("General (Teleportation Effects)")]
        [SerializeField] private ParticleSystem _teleporterWarmupParticleSystem;

        [Space(5)]
        [SerializeField] private AudioSource _audioSource;
        [SerializeField] private AudioClip _teleportSFXClip;


        protected IEnumerator TeleporterWarmup()
        {
            _teleporterWarmupParticleSystem.Play();

            yield return new WaitForSeconds(_teleporterWarmupTime);

            _teleporterWarmupParticleSystem.Stop();
            PerformTeleportation();
        }
        protected abstract void PerformTeleportation();


        protected void StartTeleportation()
        {
            if (_teleporterWarmupCoroutine != null)
            {
                // The teleporter is already warming up.
                return;
            }

            _audioSource.clip = _teleportSFXClip;
            _audioSource.Play();
            
            _teleporterWarmupCoroutine = StartCoroutine(TeleporterWarmup());
        }
        protected void StopTeleportation()
        {
            if (_teleporterWarmupCoroutine != null)
            {
                StopCoroutine(_teleporterWarmupCoroutine);
                _teleporterWarmupCoroutine = null;
            }

            _teleporterWarmupParticleSystem.Stop();
        }
    }
}