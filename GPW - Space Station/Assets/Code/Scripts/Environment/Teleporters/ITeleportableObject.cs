using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Teleporters
{
    public interface ITeleportableObject
    {
        public Vector3 Position { get; }
        public Vector3 Forward { get; }

        public void Teleport(Vector3 newPosition, Quaternion newRotation);
    }
}