using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Environment.Teleporters
{
    /// <summary> A teleporter which acts within a single scene.</summary>
    public class IntraSceneTeleporter : BaseTeleporter
    {
        private HashSet<ITeleportableObject> _currentTeleportationTargets = new HashSet<ITeleportableObject>();


        [Header("Intra-Scene Teleporter Settings")]
        [SerializeField] private IntraSceneTeleporter _linkedTeleporter;

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

                StartTeleportation();
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
                    StopTeleportation();
                }
            }
        }

        protected override void PerformTeleportation()
        {
            // Notify the linked teleporter that we are teleporting to them.
            _linkedTeleporter.PrepareToReceiveTarget();

            // Teleport each teleportable object currently within the teleporter's trigger volume.
            foreach(ITeleportableObject teleportableObject in _currentTeleportationTargets)
            {
                // Preserve local positioning within the teleporter.
                Vector3 relativeOffset = this.transform.InverseTransformPoint(teleportableObject.Position - _teleportPosition);
                Vector3 destinationPosition = _linkedTeleporter.GetRelativeOffset(relativeOffset);

                // Preserve relative forward direction within the teleporter.
                Vector3 teleportableObjectRelativeForwardToThis = transform.InverseTransformDirection(teleportableObject.Forward);
                Vector3 teleportableObjectRelativeForwardToLinked = _linkedTeleporter.transform.TransformDirection(teleportableObjectRelativeForwardToThis);

                // Teleport the teleporation target.
                teleportableObject.Teleport(destinationPosition, teleportableObjectRelativeForwardToLinked);
            }

            _teleporterReadyTime = Time.time + _teleporterCooldown;
        }
        public void PrepareToReceiveTarget() => _teleporterReadyTime = Time.time + 1.0f;


        public Vector3 GetRelativeOffset(Vector3 offset)
        {
            return this.transform.TransformPoint(offset) + this._teleportPosition;
        }


        private void OnDrawGizmosSelected()
        {
            Gizmos.DrawWireSphere(TeleportPosition, 0.25f);
        }
    }
}