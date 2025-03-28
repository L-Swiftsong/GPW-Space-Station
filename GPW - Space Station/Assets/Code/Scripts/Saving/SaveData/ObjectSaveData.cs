using UnityEngine;

namespace Saving.LevelData
{
    [System.Serializable]
    public class ObjectSaveData
    {
        [field: SerializeField] public SerializableInstanceGuid ID { get; set; }
        [ReadOnly] public bool Exists = false;
        public bool WasDestroyed = false;
        [Space(5)]

        public bool[] BoolValues;
        public int[] IntValues;
    }
}