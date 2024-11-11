using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SceneManagement;
using JSONSerialisation;
using System.IO;

namespace Saving
{
    public class SaveManager : MonoBehaviour
    {
        [System.Serializable]
        private struct SaveData
        {
            public float SaveTime;
            public int[] LoadedSceneIndices;

            public PlayerManager.PlayerSetupData PlayerData;

            public static SaveData Default = new SaveData() {
                SaveTime = 0.0f,
                LoadedSceneIndices = null,
                PlayerData = PlayerManager.PlayerSetupData.Default,
            };
        }
        [System.Serializable]
        private struct SaveDataBundle
        {
            public SaveData CheckpointSaveData;
            public SaveData HubSaveData;
            public SaveData CurrentSaveData;

            public static SaveDataBundle Default = new SaveDataBundle() {
                CheckpointSaveData = SaveData.Default,
                HubSaveData = SaveData.Default,
                CurrentSaveData = SaveData.Default,
            };
        }

        // Save Data.
        private static SaveData s_checkpointSaveData = SaveData.Default; // The save data from when the player last entered a checkpoint.
        private static SaveData s_hubEnterSaveData = SaveData.Default; // The save data from when the player last entered the Hub.
        private static SaveData s_currentSaveData = SaveData.Default; // The save data from the previous performed save.


        // Autosave Parameters.
        private const float AUTOSAVE_DELAY = 0.5f; // Time between autosaves (In minutes).
        private static Coroutine s_autosaveCoroutine;
        private static float s_previousAutosaveTime;
        private static WaitForSeconds _waitForAutosaveDelay = new WaitForSeconds(AUTOSAVE_DELAY * 60.0f);


        private void Awake()
        {
            if (s_autosaveCoroutine == null)
            {
                s_previousAutosaveTime = 0.0f;
                s_autosaveCoroutine = StartCoroutine(HandleAutosaves());
            }
        }


        private static IEnumerator HandleAutosaves()
        {
            // Wait until the game has actually started (E.g. We're not in the Main Menu).
            bool updateAutosaveTime = !SceneLoader.s_HasGameStarted;
            yield return new WaitUntil(() => SceneLoader.s_HasGameStarted == true);

            if (updateAutosaveTime)
            {
                // We're to update the previous autosave time as when we loaded the game wasn't ready.
                s_previousAutosaveTime = Time.time;
            }

            float initialDelay = (AUTOSAVE_DELAY * 60.0f) - (Time.time - s_previousAutosaveTime);
            Debug.Log("Initial Delay: " + initialDelay.ToString() + " seconds");
            yield return new WaitForSeconds(initialDelay);

            while (true)
            {
                // Save the game data.
                SaveGameState(ref s_currentSaveData);

                // Save to an external file.
                PerformAutosave();

                // Wait until we should perform the next autosave.
                s_previousAutosaveTime = Time.time;
                yield return _waitForAutosaveDelay;
            }
        }
        // Save to file (From Autosave).
        private static void PerformAutosave() => SaveJSONAutosave();
        


        
        private static SaveDataBundle GetSaveDataBundle() => new SaveDataBundle() {
            CheckpointSaveData = s_checkpointSaveData,
            HubSaveData = s_hubEnterSaveData,
            CurrentSaveData = s_currentSaveData,
        };
        private static void OverrideFromSaveDataBundle(SaveDataBundle bundle)
        {
            s_checkpointSaveData = bundle.CheckpointSaveData;
            s_hubEnterSaveData = bundle.HubSaveData;
            s_currentSaveData = bundle.CurrentSaveData;
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



        private void SceneLoader_OnLoadFinished() => SaveCheckpoint();
        private void SceneLoader_OnHubLoadFinished() => SaveHub();

        private void SceneLoader_OnReloadFinished() => LoadGameState(ref s_checkpointSaveData);
        private void SceneLoader_OnReloadToHubFinished() => LoadGameState(ref s_hubEnterSaveData);


        /// <summary> Save to the CheckpointSaveData struct.</summary>
        public static void SaveCheckpoint() => SaveGameState(ref s_checkpointSaveData);
        /// <summary> Save to the HubSaveData struct.</summary>
        public static void SaveHub() { Debug.Log("Hub Save"); SaveGameState(ref s_hubEnterSaveData); }
        
        /// <summary> Save to the ManualSaveData struct and perform a manual save.</summary>
        public static void ManualSave()
        {
            // Update saved game data structs.
            SaveGameState(ref s_currentSaveData);

            // Save to file (From Manual).
            SaveJSONManual();
        }


        private static void SaveGameState(ref SaveData saveData)
        {
            // Clear current save data.
            saveData = new SaveData();

            // General Save Data.
            saveData.SaveTime = Time.time;
            saveData.LoadedSceneIndices = SceneLoader.GetActiveSceneBuildIndices(ignorePersistents: true);
            

            // Save Player Data.
            saveData.PlayerData = PlayerManager.Instance.GetCurrentPlayerData();

            // Save Level Data.
            #region Level Data

            // Save Door States (Open, Unlocked).


            #endregion
        }

        public static void StartLoadFromFileInfo(FileInfo fileInfo)
        {
            // Load the data from the file.
            SaveDataBundle saveDataBundle = JsonDataService.LoadDataRelative<SaveDataBundle>(fileInfo.Name);
            OverrideFromSaveDataBundle(saveDataBundle);

            // Load the current save data from the retrieved data.
            StartLoadGameState(s_currentSaveData.SaveTime > s_checkpointSaveData.SaveTime ? s_currentSaveData : s_checkpointSaveData);
        }
        private static void StartLoadGameState(SaveData saveData) // Note: Cannot pass 'saveData' as a reference due to using it in a lambda expression. We need to let it be copied instead.
        {
            // Load active scenes. Then, once complete, load the rest of the save data.
            SceneLoader.Instance.LoadFromSave(saveData.LoadedSceneIndices, () => LoadGameState(ref saveData));
        }
        private static void LoadGameState(ref SaveData saveData)
        {
            // Load Player Data.
            PlayerManager.Instance.LoadFromPlayerData(saveData.PlayerData);

            // Load Level Data.
        }


        private static void SaveJSONManual()
        {
            string slashlessTime = System.DateTime.Now.ToString().Replace('/', '_').Replace(':', '_');
            string fileName = "ManualSaveData (" + slashlessTime + ").json";

            JsonDataService.SaveDataRelative(fileName, GetSaveDataBundle(), true);
        }
        private static void SaveJSONAutosave() => JsonDataService.SaveDataRelative(relativePath: "Autosave_0.json", GetSaveDataBundle(), true);
        



        [ContextMenu("Continue Test")]
        private static void LoadMostRecentSave() => StartLoadFromFileInfo(GetMostRecentSaveFile());

        public static FileInfo[] GetAllSaveFiles(bool ordered = false)
        {
            // Get all '.json' files in our save directory.
            string path = Application.persistentDataPath + "/";
            DirectoryInfo directoryInfo = new System.IO.DirectoryInfo(path);
            FileInfo[] fileInfoArray = directoryInfo.GetFiles("*.json");

            if (ordered)
            {
                // Order the files by their creation time in descending order (Index 0 is the newest file).
                // Courtesy of 'Henrik'. Link: 'https://stackoverflow.com/a/23627452'.
                System.Array.Sort(fileInfoArray, delegate(FileInfo f1, FileInfo f2)
                {
                    return f2.CreationTime.CompareTo(f1.CreationTime);
                });
            }

            return fileInfoArray;
        }
        private static FileInfo GetMostRecentSaveFile()
        {
            FileInfo[] fileInfoArray = GetAllSaveFiles(ordered: true);

            if (fileInfoArray.Length <= 0)
            {
                Debug.LogError("ERROR: No save data to read.");
                return null;
            }

            // Return the most recently saved file.
            return fileInfoArray[0];
        }
    }
}