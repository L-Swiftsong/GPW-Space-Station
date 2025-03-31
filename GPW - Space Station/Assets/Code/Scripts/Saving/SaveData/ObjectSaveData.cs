using UnityEngine;

namespace Saving.LevelData
{
    [System.Serializable]
    public class ObjectSaveData : ISerializationCallbackReceiver
    {
        [field: ReadOnly] [field: SerializeField] public SerializableInstanceGuid ID { get; set; }
        
        [ReadOnly] public Vector3 Position;
        [System.NonSerialized] private Vector3 _tempPosition;
        [ReadOnly] public Quaternion Rotation;


        [ReadOnly] public bool Exists = false;
        [ReadOnly] public DisabledState DisabledState = DisabledState.None;
        [Space(5)]

        [ReadOnly] public bool[] BoolValues;
        [ReadOnly] public int[] IntValues;


        private float ROUNDING_FACTOR = 1000.0f;
        public void OnBeforeSerialize()
        {
            Position = new Vector3(Round(Position.x), Round(Position.y), Round(Position.z));
        }
        public void OnAfterDeserialize() => Position = Position / ROUNDING_FACTOR;
        private int Round(float value) => Mathf.RoundToInt(value * ROUNDING_FACTOR);
    }
}