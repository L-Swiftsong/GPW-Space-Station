using UnityEngine;

namespace Saving.LevelData
{
    public abstract class ObjectSaveInformation
    {
        public ObjectSaveData ObjectSaveData;
        public SerializableGuid ID { get => ObjectSaveData.ID; set => ObjectSaveData.ID = value; }
        public bool Exists { get => ObjectSaveData.Exists; }

        public Vector3 Position {get => ObjectSaveData.Position; set => ObjectSaveData.Position = value; }
        public Quaternion Rotation {get => ObjectSaveData.Rotation; set => ObjectSaveData.Rotation = value; }

        public DisabledState DisabledState { get => ObjectSaveData.DisabledState; set => ObjectSaveData.DisabledState = value; }


        public ObjectSaveInformation(ObjectSaveData objectSaveData, int boolCount, int intCount)
        {
            this.ObjectSaveData = objectSaveData;

            if (objectSaveData.BoolValues.Length != boolCount)
                objectSaveData.BoolValues = new bool[boolCount];
            if (objectSaveData.IntValues.Length != intCount)
                objectSaveData.IntValues = new int[intCount];
        }
        public ObjectSaveInformation(SerializableGuid id, DisabledState disabledState, int boolCount, int intCount )
        {
            this.ObjectSaveData = new ObjectSaveData()
            {
                ID = id,
                Exists = true,
                DisabledState = disabledState,
                BoolValues = new bool[boolCount],
                IntValues = new int[intCount],
            };
        }
    }


    [System.Serializable] [System.Flags]
    public enum DisabledState
    {
        None = 0,
        EntityDisabled = 1 << 0,
        ComponentDisabled = 1 << 1,
        Destroyed = 1 << 2,
    }
}