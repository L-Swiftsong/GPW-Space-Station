namespace Saving.LevelData
{
    [System.Serializable]
    public class KeycardReaderSaveInformation : ObjectSaveInformation
    {
        public bool IsUnlocked { get => this.ObjectSaveData.BoolValues[0]; set => this.ObjectSaveData.BoolValues[0] = value; }


        public KeycardReaderSaveInformation(ObjectSaveData objectSaveData) : base(objectSaveData) { }
        public KeycardReaderSaveInformation(SerializableGuid id, bool isUnlocked) : base(id, boolCount: 1)
        {
            this.IsUnlocked = isUnlocked;
        }
    }
}