using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Teleporters
{
    public class TeleportableObject : MonoBehaviour, ITeleportableObject
    {
        public void Teleport(Vector3 destinationPosition, Quaternion destinationRotation, Vector3 originForward)
        {
            // Update position.
            transform.position = destinationPosition;
            Physics.SyncTransforms(); // Allows for the teleportation of PhysicsObjects and CharacterControllers.

            // Update rotation (Maintaining relative rotation compared to each teleporter's forward vector).
            Quaternion initialTeleporterRotation = Quaternion.FromToRotation(originForward, transform.forward);
            transform.rotation = initialTeleporterRotation * destinationRotation;
        }
    }
}