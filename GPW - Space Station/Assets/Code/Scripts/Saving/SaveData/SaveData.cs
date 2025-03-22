using UnityEngine;
using Entities.Player;
using Items;

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
            };
        }
    }
}
