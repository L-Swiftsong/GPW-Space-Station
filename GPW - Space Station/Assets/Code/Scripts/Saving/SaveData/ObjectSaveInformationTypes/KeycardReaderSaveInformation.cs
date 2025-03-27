namespace Saving.LevelData
{
    [System.Serializable]
    public class KeycardReaderSaveInformation : ObjectSaveInformation
    {
        public bool IsLocked { get => this.BoolValues[0]; set => this.BoolValues[0] = value; }


        public KeycardReaderSaveInformation() : base(boolValuesCount: 1)
        { }
    }
}