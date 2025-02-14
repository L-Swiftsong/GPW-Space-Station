namespace Saving
{
    [System.Serializable]
    public struct SaveDataBundle
    {
        public SaveData CheckpointSaveData;
        public SaveData HubSaveData;
        public SaveData CurrentSaveData;

        public static SaveDataBundle Empty = new SaveDataBundle()
        {
            CheckpointSaveData = SaveData.Empty,
            HubSaveData = SaveData.Empty,
            CurrentSaveData = SaveData.Empty,
        };
    }
}
