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
        private Coroutine _teleporterWarmupCoroutine = null;
        private HashSet<ITeleportableObject> _currentTeleportationTargets = new HashSet<ITeleportableObject>();

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

            if (other.TryGetComponent<ITeleportableObject>(out ITeleportableObject teleportationTarget))
            {
                _currentTeleportationTargets.Add(teleportationTarget);


                if (_teleporterWarmupCoroutine == null)
                {
                    _teleporterWarmupCoroutine = StartCoroutine(TeleporterWarmup());
                }
            }
        }
        private void OnTriggerExit(Collider other)
        {
            if (other.TryGetComponent<ITeleportableObject>(out ITeleportableObject teleportationTarget))
            {
                _currentTeleportationTargets.Remove(teleportationTarget);

                if (_currentTeleportationTargets.Count <= 0)
                {
                    // There are no more teleportation targets within the teleporter.

                    if (_teleporterWarmupCoroutine != null)
                    {
                        StopCoroutine(_teleporterWarmupCoroutine);
                        _teleporterWarmupCoroutine = null;
                    }

                    _teleporterEffect.Stop();
                }
            }
        }

        private IEnumerator TeleporterWarmup()
        {
            _teleporterEffect.Play();

            yield return new WaitForSeconds(_teleporterWarmupTime);

            _teleporterEffect.Stop();
            TeleportTarget();
        }
        private void TeleportTarget()
        {
            _linkedTeleporter.PrepareToReceiveTarget();

            foreach(ITeleportableObject teleportableObject in _currentTeleportationTargets)
            {
                teleportableObject.Teleport(_linkedTeleporter.TeleportPosition, _linkedTeleporter.transform.rotation, transform.forward);
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