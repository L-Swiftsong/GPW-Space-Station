using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SceneManagement;
using System.IO;

namespace Saving
{
    public class SaveManager : MonoBehaviour
    {
        [System.Serializable]
        private struct SaveData
        {
            public float SaveTime;

            public PlayerManager.PlayerSetupData PlayerData;

            public static SaveData Default = new SaveData() {
                PlayerData = PlayerManager.PlayerSetupData.Default,
                SaveTime = 0.0f,
            };
        }
        private static SaveData s_checkpointSaveData = SaveData.Default;
        private static SaveData s_hubEnterSaveData = SaveData.Default;
        private static SaveData s_manualSaveData = SaveData.Default;


        [System.Serializable]
        private struct SaveDataBundle
        {
            public SaveData CheckpointSaveData;
            public SaveData HubSaveData;
            public SaveData ManualSaveData;
        }
        private SaveDataBundle GetSaveDataBundle() => new SaveDataBundle() {
            CheckpointSaveData = s_checkpointSaveData,
            HubSaveData = s_hubEnterSaveData,
            ManualSaveData = s_manualSaveData,
        };
        private void OverrideFromSaveDataBundle(SaveDataBundle bundle)
        {
            s_checkpointSaveData = bundle.CheckpointSaveData;
            s_hubEnterSaveData = bundle.HubSaveData;
            s_manualSaveData = bundle.ManualSaveData;
        }



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
        public static void ManualSave() => SaveGameState(ref s_manualSaveData);


        private static void SaveGameState(ref SaveData saveData)
        {
            // Clear current save data.
            saveData = new SaveData();
            saveData.SaveTime = Time.time;


            // Save Player Data.
            saveData.PlayerData = PlayerManager.Instance.GetCurrentPlayerData();

            // Save Level Data.
            #region Level Data

            // Save Door States (Open, Unlocked).


            #endregion
        }
        private static void LoadGameState(ref SaveData saveData)
        {
            // Load Player Data.
            PlayerManager.Instance.LoadFromPlayerData(saveData.PlayerData);

            // Load Level Data.
        }


        [ContextMenu("Manual Save")]
        private void SaveIntoJSON()
        {
            string saveDataBundle = JsonUtility.ToJson(GetSaveDataBundle(), true);

            string slashlessTime = System.DateTime.Now.ToString().Replace('/', '_').Replace(':', '_');
            string fileName = "ManualSaveData (" + slashlessTime + ").json";

            File.WriteAllText(Application.persistentDataPath + "/" + fileName, saveDataBundle);
        }
        [ContextMenu("Autosave")]
        private void SaveAutosaveToJSON()
        {
            string saveDataBundle = JsonUtility.ToJson(GetSaveDataBundle(), true);

            string fileName = "Autosave_0.json";

            File.WriteAllText(Application.persistentDataPath + "/" + fileName, saveDataBundle);
        }



        [ContextMenu("Manual Load")]
        private void LoadFromJSON()
        {
            string path = Application.persistentDataPath + "/";
            DirectoryInfo directoryInfo = new DirectoryInfo(path);
            FileInfo[] fileInfoArray = directoryInfo.GetFiles("ManualSaveData*.json");

            if (fileInfoArray.Length <= 0)
            {
                Debug.LogError("ERROR: No save data to read.");
            }

            // Find the most recently saved file and retreive its data.
            int mostRecentSaveIndex = 0;
            Debug.Log("Initial File: '" + fileInfoArray[mostRecentSaveIndex].Name + "'. Created: " + fileInfoArray[mostRecentSaveIndex].CreationTimeUtc);
            
            for (int i = 1; i < fileInfoArray.Length; i++)
            {
                Debug.Log("Check File: '" + fileInfoArray[i].Name + "'. Created: " + fileInfoArray[i].CreationTimeUtc);
                if (fileInfoArray[i].CreationTimeUtc > fileInfoArray[mostRecentSaveIndex].CreationTimeUtc)
                {
                    mostRecentSaveIndex = i;
                }
            }
            Debug.Log("Most Recent File: '" + fileInfoArray[mostRecentSaveIndex].Name + "'. Created: " + fileInfoArray[mostRecentSaveIndex].CreationTimeUtc);


            // Load from the file.
            string saveDataText = File.ReadAllText(fileInfoArray[mostRecentSaveIndex].FullName);
            SaveDataBundle saveDataBundle = JsonUtility.FromJson<SaveDataBundle>(saveDataText);
            OverrideFromSaveDataBundle(saveDataBundle);

            // Load the most recently saved item in the loaded bundle.
            if (s_checkpointSaveData.SaveTime >= s_hubEnterSaveData.SaveTime)
            {
                if (s_manualSaveData.SaveTime >= s_checkpointSaveData.SaveTime)
                {
                    LoadGameState(ref s_manualSaveData);
                }
                else
                {
                    LoadGameState(ref s_checkpointSaveData);
                }
            }
            else
            {
                if (s_manualSaveData.SaveTime >= s_hubEnterSaveData.SaveTime)
                {
                    LoadGameState(ref s_manualSaveData);
                }
                else
                {
                    LoadGameState(ref s_hubEnterSaveData);
                }
            }
        }
    }
}