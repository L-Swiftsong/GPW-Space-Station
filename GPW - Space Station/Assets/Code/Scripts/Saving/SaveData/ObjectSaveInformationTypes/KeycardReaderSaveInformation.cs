namespace Saving.LevelData
{
    [System.Serializable]
    public class KeycardReaderSaveInformation : ObjectSaveInformation
    {
        public bool IsUnlocked { get => this.ObjectSaveData.BoolValues[0]; set => this.ObjectSaveData.BoolValues[0] = value; }


        public KeycardReaderSaveInformation(ObjectSaveData objectSaveData, DisabledState disabledState) : base(objectSaveData, disabledState) { }
        public KeycardReaderSaveInformation(SerializableGuid id, DisabledState disabledState, bool isUnlocked) : base(id, disabledState, boolCount: 1)
        {
            this.IsUnlocked = isUnlocked;
        }
    }
}