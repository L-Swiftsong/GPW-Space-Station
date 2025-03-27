using UnityEngine;

namespace Saving.LevelData
{
    [System.Serializable]
    public class ObjectSaveInformation
    {
        public ObjectSaveData ObjectSaveData;
        public SerializableGuid ID { get => ObjectSaveData.ID; set => ObjectSaveData.ID = value; }
        public bool Exists { get => ObjectSaveData.Exists; }
        public bool WasDestroyed { get => ObjectSaveData.WasDestroyed; set => ObjectSaveData.WasDestroyed = value; }


        public ObjectSaveInformation(ObjectSaveData objectSaveData)
        {
            this.ObjectSaveData = objectSaveData;
        }
        public ObjectSaveInformation(SerializableGuid id, int boolCount = 0, int intCount = 0)
        {
            this.ObjectSaveData = new ObjectSaveData()
            {
                ID = id,
                Exists = true,
                BoolValues = new bool[boolCount],
                IntValues = new int[intCount],
            };
        }
    }
}