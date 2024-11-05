using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Saving
{
    public class SaveManager : MonoBehaviour
    {
        [System.Serializable]
        private struct TempSaveData
        {
            public PlayerManager.PlayerSetupData PlayerData;
        }
        private static TempSaveData s_saveData;


        private void OnEnable() => SceneManagement.SceneLoader.OnReloadFinished += SceneLoader_OnReloadFinished;
        private void OnDisable() => SceneManagement.SceneLoader.OnReloadFinished -= SceneLoader_OnReloadFinished;
        



        private void SceneLoader_OnReloadFinished()
        {
            LoadGameState();
        }
        public static void SaveGameState()
        {
            Debug.Log("Saving Game");

            // Clear current save data.
            s_saveData = new TempSaveData();

            // Save Player Data.
            s_saveData.PlayerData = PlayerManager.Instance.GetCurrentPlayerData();

            // Save Level Data.
            #region Level Data

            // Save Door States (Open, Unlocked).


            #endregion
        }
        public static void LoadGameState()
        {
            // Load Player Data.
            PlayerManager.Instance.LoadFromPlayerData(s_saveData.PlayerData);

            // Load Level Data.
        }
    }
}