namespace Saving.LevelData
{
    [System.Serializable]
    public class PadlockSaveInformation : ObjectSaveInformation
    {
        // We are using try...catch as a fallback as during the period between a scene finishing loading and the data being bound there is a period where we can get a nullreferenceexception from trying to fetch/set values.
        public bool IsUnlocked
        {
            get
            {
                try     { return this.ObjectSaveData.BoolValues[0]; }
                catch   { return false; }
            }
            set 
            {
                try     { this.ObjectSaveData.BoolValues[0] = value; }
                catch   { this.ObjectSaveData.BoolValues = new bool[1] { value }; }
            }
        }
        public int[] CurrentSetValues
        {
            get => this.ObjectSaveData.IntValues;
            set => this.ObjectSaveData.IntValues = value;
        }


        public PadlockSaveInformation(ObjectSaveData objectSaveData) : base(objectSaveData) { }
        public PadlockSaveInformation(SerializableGuid id, bool isUnlocked, int[] intValues) : base(id, boolCount: 1, intCount: intValues.Length)
        {
            this.IsUnlocked = isUnlocked;
            this.CurrentSetValues = intValues;
        }
    }
}