namespace Saving.LevelData
{
    [System.Serializable]
    public class DoorSaveInformation : ObjectSaveInformation
    {
        public bool IsOpen { get => this.ObjectSaveData.BoolValues[0]; set => this.ObjectSaveData.BoolValues[0] = value; }


        public DoorSaveInformation(ObjectSaveData objectSaveData, DisabledState disabledState) : base(objectSaveData, disabledState) { }
        public DoorSaveInformation(SerializableGuid id, DisabledState disabledState, bool isOpen) : base(id, disabledState, boolCount: 1)
        {
            this.IsOpen = isOpen;
        }
    }
}