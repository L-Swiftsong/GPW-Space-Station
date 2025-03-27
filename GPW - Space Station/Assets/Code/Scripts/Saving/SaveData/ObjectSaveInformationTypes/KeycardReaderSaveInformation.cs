namespace Saving.LevelData
{
    [System.Serializable]
    public class KeycardReaderSaveInformation : ObjectSaveInformation
    {
        public bool IsLocked { get => this.ObjectSaveData.BoolValues[0]; set => this.ObjectSaveData.BoolValues[0] = value; }


        public KeycardReaderSaveInformation(ObjectSaveData objectSaveData) : base(objectSaveData) { }
        public KeycardReaderSaveInformation(SerializableGuid id, bool isLocked) : base(id, boolCount: 1)
        {
            this.IsLocked = isLocked;
        }
    }
}