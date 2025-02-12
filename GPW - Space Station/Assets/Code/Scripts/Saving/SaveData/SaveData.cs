namespace Saving
{
    [System.Serializable]
    public struct SaveData
    {
        public bool Exists;
        public float SaveTime;

        public int[] LoadedSceneIndices;
        public int ActiveSceneIndex;


        public PlayerSaveData PlayerData;
        public InventorySaveData ItemSaveData;


        public static SaveData Empty = new SaveData()
        {
            Exists = false,
            SaveTime = 0.0f,

            LoadedSceneIndices = null,
            ActiveSceneIndex = -1,

            PlayerData = PlayerSaveData.Default,
            ItemSaveData = InventorySaveData.Default,
        };
    }
}
