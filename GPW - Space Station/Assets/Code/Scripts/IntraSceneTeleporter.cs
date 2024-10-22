using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Teleporters
{
    /// <summary> A teleporter which acts within a single scene.</summary>
    public class IntraSceneTeleporter : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private IntraSceneTeleporter _linkedTeleporter;
        [SerializeField] private ParticleSystem _teleporterEffect;


        [Header("Teleportation Parameters")]
        [SerializeField] private float _teleporterWarmupTime;
        private Coroutine _teleporterWarmupCoroutine;
        private Transform _currentTeleportTarget;

        [Space(5)]
        [SerializeField] private float _teleporterCooldown = 3.0f;
        private float _teleporterReadyTime;
        private bool _canTeleport;

        [Space(5)]
        [SerializeField] public Vector3 _teleportPosition;
        public Vector3 TeleportPosition => transform.position + _teleportPosition;


        private void Awake()
        {
            _canTeleport = true;
            _teleporterReadyTime = 0.0f;
        }


        private void OnTriggerEnter(Collider other)
        {
            if (!_canTeleport)
            {
                // We cannot teleport.
                return;
            }
            if (_teleporterReadyTime > Time.time)
            {
                // The teleporter is still in cooldown.
                return;
            }
            if (_currentTeleportTarget != null)
            {
                // We are already teleporting a target.
                return;
            }


            if (_teleporterWarmupCoroutine != null)
                StopCoroutine(_teleporterWarmupCoroutine);
            _teleporterWarmupCoroutine = StartCoroutine(TeleporterWarmup(other.transform));
        }
        private void OnTriggerExit(Collider other)
        {
            if (other.transform == _currentTeleportTarget)
            {
                if (_teleporterWarmupCoroutine != null)
                    StopCoroutine(_teleporterWarmupCoroutine);

                _currentTeleportTarget = null;

                _teleporterEffect.Stop();
            }
        }

        private IEnumerator TeleporterWarmup(Transform target)
        {
            _teleporterEffect.Play();

            _currentTeleportTarget = target;
            yield return new WaitForSeconds(_teleporterWarmupTime);
            _currentTeleportTarget = null;

            _teleporterEffect.Stop();
            TeleportTarget(target);
        }
        private void TeleportTarget(Transform target)
        {
            _linkedTeleporter.PrepareToReceiveTarget();
            
            if (target.TryGetComponent<CharacterController>(out CharacterController controller))
            {
                controller.Move(_linkedTeleporter.TeleportPosition - target.position);
            }
            else
            {
                target.position = _linkedTeleporter.TeleportPosition;
            }


            _teleporterReadyTime = Time.time + _teleporterCooldown;
        }

        
        public void PrepareToReceiveTarget() => _teleporterReadyTime = Time.time + 1.0f;


        private void OnDrawGizmosSelected()
        {
            Gizmos.DrawWireSphere(TeleportPosition, 0.25f);
        }
    }
}