using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SceneManagement;
using JSONSerialisation;
using System.IO;
using System.Linq;
using Saving.LevelData;

namespace Saving
{
    // Source: 'https://www.youtube.com/watch?v=z1sMhGIgfoo'.
    public class SaveManager : Singleton<SaveManager>
    {
        private int _currentSaveID;


        private const bool USE_PRETTY_PRINT = true;


        public void NewGame()
        {
            // Reset Input Prevention.
            PlayerInput.ResetInputPrevention();

            // Initialise our Save.
            _currentSaveID = GetUnusedSaveID();
            UI.ItemDisplay.KeyItemEntryUI.ResetUsedKeyItems();
            SaveData.PrepareForNewGame();

            Debug.Log("Starting New Game. ID: " + _currentSaveID);

            // Save after starting the new game.
            // We need to listen to the 'OnLoadFinished' event so that we don't save when saveables (Such as the player) haven't been loaded.
            SceneLoader.OnLoadFinished += SceneLoader_OnLoadFinished;
        }



        private void SceneLoader_OnLoadFinished()
        {
            // Save after starting the new game.
            SaveGameAutosave();

            // Unsubscribe from the event so that we don't double-trigger.
            SceneLoader.OnLoadFinished -= SceneLoader_OnLoadFinished;
        }

        #region Saving

        public void SaveGameManual() => SaveGame(CreateSaveName("Manual"));
        public void SaveGameAutosave() => SaveGame(CreateSaveName("Autosave"));
        public void SaveGameDebug() => SaveGame("DebugSave");
        private void SaveGame(string saveDataName)
        {
            SaveData.PrepareForSave();
            JsonDataService.Save<SaveData>(saveDataName, SaveData.FromCurrent(_currentSaveID), USE_PRETTY_PRINT, overwrite: true);
            SaveData.OnAfterSave();
        }

        // To-Do: Find a better way to do this.
        private int GetUnusedSaveID()
        {
            // Find all existing save IDs and put them into a HashSet for checking later (Scales better for searching than a list does).
            HashSet<int> existingIDs;
            int fileCount;
            {
                string[] fileNames = JsonDataService.GetSaveNames().ToArray();
                fileCount = fileNames.Length;
                existingIDs = new HashSet<int>(fileCount); // We know our capacity won't exceed fileCount. Initialise our capacity so that we don't need to resize when adding the elements.

                // Simplify our fileNames.
                for (int i = 0; i < fileCount; ++i)
                {
                    // Save Data in the form 'Playthrough-X_Autosave'/'Playthrough-Manual', where 'X' is the SaveID and should be an integer.
                    try
                    {
                        if (fileNames[i].StartsWith("Playthrough-"))
                        {
                            if (int.TryParse(fileNames[i].Remove(0, 12).Split('_')[0], out int result))
                            {
                                // Successful parse - This Save is setup properly for our IDs.
                                existingIDs.Add(result);
                                continue;
                            }
                        }
                    }
                    catch { } // Catch any exceptions (Such as the fileName being in the wrong format)and just continue.

                    // Failed to parse.
                    // This save isn't named in the way we are expecting, so it's value doesn't matter.
                    // We use '-1' to represent this so that we can still have saves of ID 0 (Int Default). 
                    existingIDs.Add(-1);
                }
            }

            // Try to find the first free ID.
            for (int potentialID = 0; potentialID < fileCount; ++potentialID)
            {
                if (!existingIDs.Contains(potentialID))
                {
                    // We have no save of this ID, so use it.
                    return potentialID;
                }
            }

            // All our existing saves have sequential IDs from '0' to 'fileCount - 1', so use 'fileCount' as our Save ID.
            return fileCount;
        }
        private string CreateSaveName(string saveTypeIdentifier) => string.Concat("Playthrough-", _currentSaveID, "_", saveTypeIdentifier);

        #endregion


        #region Loading

        public void LoadMostRecentSave() => LoadGame(GetMostRecentSaveFile());
        public void LoadGame(FileInfo fileInfo) => LoadGame(Path.GetFileNameWithoutExtension(fileInfo.Name));
        public void LoadGame(string fileName)
        {
            // Get the desired Save Data.
            SaveData saveData = JsonDataService.Load<SaveData>(fileName);
            _currentSaveID = saveData.SaveID;

            // Reset input prevention.
            PlayerInput.ResetInputPrevention();

            // Load the save.
            SceneLoader.Instance.LoadFromSave(saveData.LoadedSceneIndices, saveData.ActiveSceneIndex, onScenesLoadedCallback: () => PerformDataLoad(saveData));
        }

        private void PerformDataLoad(SaveData saveData) => saveData.LoadData();
        

        public static FileInfo[] GetAllSaveFiles(bool ordered = false)
        {
            // Get all '.json' files in our save directory.
            string path = Application.persistentDataPath + "/";
            DirectoryInfo directoryInfo = new System.IO.DirectoryInfo(path);
            FileInfo[] fileInfoArray = directoryInfo.GetFiles("*.json");

            if (ordered)
            {
                // Order the files by their last edit time in descending order (Index 0 is the newest file).
                // Courtesy of 'Henrik'. Link: 'https://stackoverflow.com/a/23627452'.
                System.Array.Sort(fileInfoArray, delegate (FileInfo f1, FileInfo f2)
                {
                    return f2.CreationTime.CompareTo(f1.LastWriteTime);
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

        public static void DeleteSave(string fileName) => JsonDataService.Delete(fileName);
        public static void DeleteSave(FileInfo fileInfo) => JsonDataService.Delete(fileInfo);
        public static void DeleteAllSaves() => JsonDataService.DeleteAll();
    }
}