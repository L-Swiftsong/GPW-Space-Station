using System.Collections.Generic;
using Items.Collectables;

namespace Saving
{
    [System.Serializable]
    public class CollectableSaveData
    {
        public CollectableDataType CollectableType;
        public bool[] CollectablesObtained;
        


        public static CollectableSaveData[] GetCurrentSaveData()
        {
            return new CollectableSaveData[]
            {
                GetCurrentForType(CollectableDataType.Codex),
            };
        }
        private static CollectableSaveData GetCurrentForType(CollectableDataType collectableDataType)
        {
            return new CollectableSaveData
            {
                CollectableType = collectableDataType,
                CollectablesObtained = CollectableManager.GetObtainedStateArrayForType(collectableDataType.ToSystemType())
            };
        }

        public static CollectableSaveData[] GetExampleData()
        {
            return new CollectableSaveData[]
            {
                new CollectableSaveData
                {
                    CollectableType = CollectableDataType.Codex,
                    CollectablesObtained = new bool[] { false, false, true },
                },
            };
        }
    }
}
