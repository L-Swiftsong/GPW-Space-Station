namespace Saving.LevelData
{
    [System.Serializable]
    public class PadlockSaveInformation : ObjectSaveInformation
    {
        public bool IsUnlocked { get => BoolValues[0]; set => BoolValues[0] = value; }
        public int[] CurrentSetValues { get => IntValues; set => IntValues = value; }


        public PadlockSaveInformation(int intValuesCount) : base(boolValuesCount: 1, intValuesCount: intValuesCount)
        { }
    }
}