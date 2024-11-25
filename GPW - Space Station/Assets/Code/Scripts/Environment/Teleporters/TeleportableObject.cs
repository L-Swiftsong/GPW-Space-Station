using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Environment.Teleporters
{
    public class TeleportableObject : MonoBehaviour, ITeleportableObject
    {
        public Vector3 Position => transform.position;
        public Vector3 Forward => transform.forward;
        
        public void Teleport(Vector3 newPosition, Quaternion newRotation)
        {
            // Update position.
            transform.position = newPosition;
            Physics.SyncTransforms(); // Allows for the teleportation of Physics Objects and CharacterControllers.

            // Update rotation.
            transform.rotation = newRotation;
        }
    }
}