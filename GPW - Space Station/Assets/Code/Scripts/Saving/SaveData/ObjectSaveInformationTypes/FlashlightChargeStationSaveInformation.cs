namespace Saving.LevelData
{
    [System.Serializable]
    public class FlashlightChargeStationSaveInformation : ObjectSaveInformation
    {
        public bool HasFlashlight { get => this.ObjectSaveData.BoolValues[0]; set => this.ObjectSaveData.BoolValues[0] = value; }


        public FlashlightChargeStationSaveInformation(ObjectSaveData objectSaveData) : base(objectSaveData, boolCount: 1, intCount: 0) { }
        public FlashlightChargeStationSaveInformation(SerializableGuid id, DisabledState disabledState, bool hasFlashlight) : base(id, disabledState, boolCount: 1, intCount: 0)
        {
            this.HasFlashlight = hasFlashlight;
        }
    }
}