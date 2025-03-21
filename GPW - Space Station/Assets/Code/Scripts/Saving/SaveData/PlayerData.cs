using UnityEngine;

namespace Saving
{
    [System.Serializable]
    public class PlayerData : ISaveable
    {
        [field: SerializeField] public SerializableGuid SaveID { get; set; }

        // Position & Rotation Information.
        public Vector3 RootPosition;
        public float YRotation;
        public float CameraXRotation;

        public MovementState MovementState;
    }
}