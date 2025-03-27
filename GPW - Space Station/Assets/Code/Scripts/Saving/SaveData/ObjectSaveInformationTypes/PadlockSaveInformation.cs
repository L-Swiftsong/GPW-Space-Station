namespace Saving.LevelData
{
    [System.Serializable]
    public class PadlockSaveInformation : ObjectSaveInformation
    {
        public bool IsUnlocked { get => this.ObjectSaveData.BoolValues[0]; set => this.ObjectSaveData.BoolValues[0] = value; }
        public int[] CurrentSetValues { get => this.ObjectSaveData.IntValues; set => this.ObjectSaveData.IntValues = value; }


        public PadlockSaveInformation(ObjectSaveData objectSaveData) : base(objectSaveData) { }
        public PadlockSaveInformation(SerializableGuid id, bool isUnlocked, int[] intValues) : base(id, boolCount: 1, intCount: intValues.Length)
        {
            this.IsUnlocked = isUnlocked;
            this.CurrentSetValues = intValues;
        }
    }
}