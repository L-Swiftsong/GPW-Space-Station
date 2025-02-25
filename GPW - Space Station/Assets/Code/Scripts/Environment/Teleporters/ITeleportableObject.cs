using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Environment.Teleporters
{
    public interface ITeleportableObject
    {
        public Vector3 Position { get; }
        public Vector3 Forward { get; }

        public void Teleport(Vector3 newPosition, Vector3 newForward);
    }
}