using UnityEngine;

namespace Saving.LevelData
{
    [System.Serializable]
    public class ObjectSaveData
    {
        [field: SerializeField] public SerializableInstanceGuid ID { get; set; }
        [ReadOnly] public bool Exists = false;
        public DisabledState DisabledState = DisabledState.None;
        [Space(5)]

        public bool[] BoolValues;
        public int[] IntValues;
    }
}