namespace Saving.LevelData
{
    public abstract class ObjectSaveInformation
    {
        public ObjectSaveData ObjectSaveData;
        public SerializableInstanceGuid ID { get => ObjectSaveData.ID; set => ObjectSaveData.ID = value; }
        public bool Exists { get => ObjectSaveData.Exists; }
        public DisabledState DisabledState { get => ObjectSaveData.DisabledState; set => ObjectSaveData.DisabledState = value; }


        public ObjectSaveInformation(ObjectSaveData objectSaveData)
        {
            this.ObjectSaveData = objectSaveData;
        }
        public ObjectSaveInformation(SerializableInstanceGuid id, int boolCount = 0, int intCount = 0)
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


    [System.Serializable] [System.Flags]
    public enum DisabledState
    {
        None = 0,
        EntityDisabled = 1 << 0,
        ComponentDisabled = 1 << 1,
        Destroyed = 1 << 2,
    }
}