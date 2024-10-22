using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Teleporters
{
    public interface ITeleportableObject
    {
        public void Teleport(Vector3 destinationPosition, Quaternion destinationRotation, Vector3 originForward);
    }
}