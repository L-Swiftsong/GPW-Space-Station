namespace Saving.LevelData
{
    [System.Serializable]
    public class PadlockSaveInformation : ObjectSaveInformation
    {
        public bool IsUnlocked { get => this.ObjectSaveData.BoolValues[0]; set => this.ObjectSaveData.BoolValues[0] = value; }
        public int[] CurrentSetValues { get => this.ObjectSaveData.IntValues; set => this.ObjectSaveData.IntValues = value; }


        public PadlockSaveInformation(ObjectSaveData objectSaveData, DisabledState disabledState) : base(objectSaveData, disabledState) { }
        public PadlockSaveInformation(SerializableInstanceGuid id, DisabledState disabledState, bool isUnlocked, int[] intValues) : base(id, disabledState, boolCount: 1, intCount: intValues.Length)
        {
            this.IsUnlocked = isUnlocked;
            this.CurrentSetValues = intValues;
        }
    }
}