using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SceneManagement;

namespace Saving
{
    public class SaveManager : MonoBehaviour
    {
        /// <summary> Replace this with something like PlayerPrefs or a .json file, as this doesn't actually save between sessions.</summary>
        [System.Serializable]
        private struct TempSaveData
        {
            public PlayerManager.PlayerSetupData PlayerData;

            public static TempSaveData Default = new TempSaveData() {
                PlayerData = PlayerManager.PlayerSetupData.Default,
            };
        }
        private static TempSaveData s_checkpointSaveData = TempSaveData.Default;
        private static TempSaveData s_hubEnterSaveData = TempSaveData.Default;


        private void OnEnable()
        {
            SceneLoader.OnLoadFinished += SceneLoader_OnLoadFinished;
            SceneLoader.OnHubLoadFinished += SceneLoader_OnHubLoadFinished;

            SceneLoader.OnReloadFinished += SceneLoader_OnReloadFinished;
            SceneLoader.OnReloadToHubFinished += SceneLoader_OnReloadToHubFinished;
        }
        private void OnDisable()
        {
            SceneLoader.OnLoadFinished -= SceneLoader_OnLoadFinished;
            SceneLoader.OnHubLoadFinished -= SceneLoader_OnHubLoadFinished;

            SceneLoader.OnReloadFinished -= SceneLoader_OnReloadFinished;
            SceneLoader.OnReloadToHubFinished -= SceneLoader_OnReloadToHubFinished;
        }



        private void SceneLoader_OnLoadFinished() => SaveGameState(ref s_checkpointSaveData);
        private void SceneLoader_OnHubLoadFinished() => SaveGameState(ref s_hubEnterSaveData);

        private void SceneLoader_OnReloadFinished() => LoadGameState(ref s_checkpointSaveData);
        private void SceneLoader_OnReloadToHubFinished() => LoadGameState(ref s_hubEnterSaveData);


        public static void SaveCheckpoint() => SaveGameState(ref s_checkpointSaveData);
        public static void SaveHub() => SaveGameState(ref s_hubEnterSaveData);


        private static void SaveGameState(ref TempSaveData saveData)
        {
            // Clear current save data.
            saveData = new TempSaveData();

            // Save Player Data.
            saveData.PlayerData = PlayerManager.Instance.GetCurrentPlayerData();

            // Save Level Data.
            #region Level Data

            // Save Door States (Open, Unlocked).


            #endregion
        }
        private static void LoadGameState(ref TempSaveData saveData)
        {
            // Load Player Data.
            PlayerManager.Instance.LoadFromPlayerData(saveData.PlayerData);

            // Load Level Data.
        }
    }
}