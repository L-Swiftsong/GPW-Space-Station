using System;
using System.IO;
using UnityEngine;

// Adapted from: 'https://www.youtube.com/watch?v=mntS45g8OK4&t=794s'.

namespace JSONSerialisation
{
    public static class JsonDataService
    {
        public static bool SaveDataRelative<T>(string relativePath, T data, bool prettyPrint = false) => SaveDataAbsolute<T>(Application.persistentDataPath + "/" + relativePath, data, prettyPrint);
        public static T LoadDataRelative<T>(string relativePath) => LoadDataAbsolute<T>(Application.persistentDataPath + "/" + relativePath);


        public static bool SaveDataAbsolute<T>(string absolutePath, T data, bool prettyPrint = false)
        {
            Debug.Log("Saving to path: " + absolutePath);
            File.WriteAllText(absolutePath, JsonUtility.ToJson(data, prettyPrint));
            return true;
        }
        public static T LoadDataAbsolute<T>(string absolutePath)
        {
            if (!File.Exists(absolutePath))
            {
                // No file exists in the given path.
                Debug.LogError("Cannot load file at " + absolutePath + ". File does not exist");
                throw new FileNotFoundException(absolutePath + " does not exist");
            }

            try
            {
                // Load the data from the chosen file and return it.
                T data = JsonUtility.FromJson<T>(File.ReadAllText(absolutePath));
                return data;
            }
            catch (Exception e)
            {
                // There was an error while retrieving the data.
                Debug.LogError("Failed to load data due to: " + e.Message + " " + e.StackTrace);
                throw;
            }
        }
    }
}