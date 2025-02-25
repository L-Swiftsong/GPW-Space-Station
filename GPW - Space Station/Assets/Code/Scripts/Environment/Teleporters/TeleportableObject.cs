using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Environment.Teleporters
{
    public class TeleportableObject : MonoBehaviour, ITeleportableObject
    {
        [SerializeField] private Transform _positionTransform;
        [SerializeField] private Transform _rotationTransform;

        public Vector3 Position => _positionTransform.position;
        public Vector3 Forward => _rotationTransform.forward;
        
        public void Teleport(Vector3 newPosition, Vector3 newForward)
        {
            // Update position.
            _positionTransform.position = newPosition;
            Physics.SyncTransforms(); // Allows for the teleportation of Physics Objects and CharacterControllers.

            // Update rotation.
            _rotationTransform.rotation = Quaternion.LookRotation(newForward, _rotationTransform.up);
        }


#if UNITY_EDITOR

        private void Reset()
        {
            _positionTransform = this.transform;
            _rotationTransform = this.transform;
        }

#endif
    }
}