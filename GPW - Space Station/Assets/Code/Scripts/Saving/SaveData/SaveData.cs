using UnityEngine;
using Entities.Player;
using Items;
using Items.Collectables;
using Saving.LevelData;

namespace Saving
{
    [System.Serializable]
    public class SaveData
    {
        public bool Exists;
        public int SaveTime;
        public int SaveID;

        public int[] LoadedSceneIndices;
        public int ActiveSceneIndex;

        public PlayerData PlayerData;
        public InventorySaveData ItemSaveData;
        public ProgressData ProgressData;
        public LevelSaveData[] LevelSaveDatas;


        public void LoadData()
        {
            PlayerData.LoadToPlayer(PlayerManager.Instance.Player.GetComponent<PlayerController>());
            ItemSaveData.LoadToInventory(PlayerManager.Instance.Player.GetComponent<Items.PlayerInventory>());
            ProgressData.LoadData();
            LevelDataManager.LoadLevelSaves(LevelSaveDatas);
        }
        public static void PrepareForNewGame()
        {
            // Ensure that we're not using old data.
            CollectableManager.ResetObtainedCollectables();
            LevelDataManager.ClearSaveDataForNewGame();
        }
        public static SaveData FromCurrent(int saveID)
        {
            return new SaveData()
            {
                Exists = true,
                SaveTime = Mathf.FloorToInt(Time.time),
                SaveID = saveID,

                LoadedSceneIndices = SceneManagement.SceneLoader.GetLoadedSceneBuildIndices(),
                ActiveSceneIndex = SceneManagement.SceneLoader.GetActiveSceneBuildIndex(),

                PlayerData = PlayerData.CreateFromPlayer(PlayerManager.Instance.Player.GetComponent<PlayerController>()),
                ItemSaveData = InventorySaveData.CreateFromInventory(PlayerManager.Instance.Player.GetComponent<PlayerInventory>()),
                ProgressData = ProgressData.FromCurrent(),
                LevelSaveDatas = LevelDataManager.GetAllExistingSaveData(),
            };
        }
    }
}
