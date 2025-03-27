namespace Saving.LevelData
{
    [System.Serializable]
    public class DoorSaveInformation : ObjectSaveInformation
    {
        public bool IsOpen { get => this.BoolValues[0]; set => this.BoolValues[0] = value; }


        public DoorSaveInformation() : base(boolValuesCount: 1)
        { }
    }
}