using UnityEngine;
using Entities.Player;
using Items;
using Saving.LevelData;

namespace Saving
{
    [System.Serializable]
    public class SaveData
    {
        public bool Exists;
        public float SaveTime;

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
        public static SaveData FromCurrent()
        {
            return new SaveData()
            {
                Exists = true,
                SaveTime = Time.time,

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
