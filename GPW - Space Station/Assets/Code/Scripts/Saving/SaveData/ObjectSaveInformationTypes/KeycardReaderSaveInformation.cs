namespace Saving.LevelData
{
    [System.Serializable]
    public class KeycardReaderSaveInformation : ObjectSaveInformation
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


        public KeycardReaderSaveInformation(ObjectSaveData objectSaveData) : base(objectSaveData) { }
        public KeycardReaderSaveInformation(SerializableGuid id, bool isUnlocked) : base(id, boolCount: 1)
        {
            this.IsUnlocked = isUnlocked;
        }
    }
}