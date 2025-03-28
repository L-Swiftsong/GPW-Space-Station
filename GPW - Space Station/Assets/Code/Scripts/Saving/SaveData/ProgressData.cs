using Items.Collectables;

namespace Saving
{
    [System.Serializable]
    public class ProgressData
    {
        public bool[] JournalsObtainedArray;

        public bool[] KeyItemsObtainedArray;
        public bool[] RepairSpotStates;


        public void LoadData()
        {
            CollectableManager.PrepareForLoad();
            CollectableManager.LoadObtainedCollectables(CollectableDataType.Codex, JournalsObtainedArray);

            CollectableManager.LoadObtainedCollectables(CollectableDataType.KeyItem, KeyItemsObtainedArray);
            RepairSpotManager.LoadRepairStates(RepairSpotStates);
        }
        public static ProgressData FromCurrent()
        {
            return new ProgressData()
            {
                JournalsObtainedArray = CollectableManager.GetObtainedStateArrayForType(CollectableDataType.Codex.ToSystemType()),

                KeyItemsObtainedArray = CollectableManager.GetObtainedStateArrayForType(CollectableDataType.KeyItem.ToSystemType()),
                RepairSpotStates = RepairSpotManager.GetRepairStates(),
            };
        }
    }
}
