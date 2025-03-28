namespace Saving.LevelData
{
    [System.Serializable]
    public class DoorSaveInformation : ObjectSaveInformation
    {
        public bool IsOpen { get => this.ObjectSaveData.BoolValues[0]; set => this.ObjectSaveData.BoolValues[0] = value; }


        public DoorSaveInformation(ObjectSaveData objectSaveData) : base(objectSaveData) { }
        public DoorSaveInformation(SerializableGuid id, bool isOpen) : base(id, boolCount: 1)
        {
            this.IsOpen = isOpen;
        }
    }
}