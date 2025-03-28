using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using SceneManagement;
using JSONSerialisation;
using System.IO;
using Entities.Player;

namespace Saving
{
    // Source: 'https://www.youtube.com/watch?v=z1sMhGIgfoo'.
    public class SaveManager : Singleton<SaveManager>
    {
        [SerializeField] private SaveData _saveData;


        private const bool USE_PRETTY_PRINT = true;
        #if UNITY_EDITOR
        private const bool ALLOW_AUTOSAVE_IN_EDITOR = true;
        #endif


        protected override void Awake()
        {
            base.Awake();
            NewGame();
        }


        public void NewGame()
        {
            Debug.Log("Starting New Game");
        }

        #region Saving

        public void SaveGameManual()
        {
            string slashlessTime = System.DateTime.Now.ToString().Replace('/', '_').Replace(':', '_').Replace(' ', '-');
            SaveGame(string.Concat("ManualSave-", slashlessTime));
        }
        public void SaveGameAutosave() => SaveGame("Autosave");
        public void SaveGameDebug() => SaveGame("DebugSave");
        private void SaveGame(string saveDataName) => JsonDataService.Save<SaveData>(saveDataName, CreateSaveData(), USE_PRETTY_PRINT);

        private SaveData CreateSaveData() => SaveData.FromCurrent();

        #endregion

        #region Loading

        public void LoadMostRecentSave() => LoadGame(GetMostRecentSaveFile());
        public void LoadGame(FileInfo fileInfo) => LoadGame(Path.GetFileNameWithoutExtension(fileInfo.Name));
        public void LoadGame(string fileName)
        {
            _saveData = JsonDataService.Load<SaveData>(fileName);
            SceneLoader.Instance.LoadFromSave(_saveData.LoadedSceneIndices, _saveData.ActiveSceneIndex, onScenesLoadedCallback: PerformDataLoad);
        }

        private void PerformDataLoad()
        {
            _saveData.LoadData();
        }

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
                System.Array.Sort(fileInfoArray, delegate (FileInfo f1, FileInfo f2)
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

        #endregion

        public static void DeleteGame(string fileName)
        {
            JsonDataService.Delete(fileName);
        }
    }
}