using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

// Adapted from: 'https://www.youtube.com/watch?v=mntS45g8OK4&t=794s' and 'https://www.youtube.com/watch?v=z1sMhGIgfoo'.
namespace JSONSerialisation
{
    public static class JsonDataService
    {
        public static readonly string DATA_PATH;
        public const string FILE_EXTENSION = "json";

        static JsonDataService()
        {
            DATA_PATH = Application.persistentDataPath;
        }


        private static string GetPathForFile(string fileName)
        {
            return Path.Combine(DATA_PATH, string.Concat(fileName, ".", FILE_EXTENSION));
        }


        public static bool Save<T>(string fileName, T data, bool prettyPrint = false, bool overwrite = true)
        {
            string filePath = GetPathForFile(fileName);
            Debug.Log("Saving to path: " + filePath);

            if (!overwrite && File.Exists(filePath))
            {
                throw new IOException($"The file '{fileName}.{FILE_EXTENSION}' already exists and cannot be overwritten.");
            }

            File.WriteAllText(filePath, JsonUtility.ToJson(data, prettyPrint));
            return true;
        }
        public static T Load<T>(string fileName)
        {
            string filePath = GetPathForFile(fileName);

            if (!File.Exists(filePath))
            {
                // No file exists in the given path.
                throw new ArgumentException($"No saved data with name '{fileName}'.");
            }

            try
            {
                // Load the data from the chosen file and return it.
                T data = JsonUtility.FromJson<T>(File.ReadAllText(filePath));
                return data;
            }
            catch (Exception e)
            {
                // There was an error while retrieving the data.
                Debug.LogError("Failed to load data due to: " + e.Message + " " + e.StackTrace);
                throw;
            }
        }


        public static void Delete(string fileName)
        {
            string filePath = GetPathForFile(fileName);

            if (File.Exists(filePath))
            {
                Debug.Log("Deleting File: " + filePath);
                File.Delete(filePath);
            }
        }
        public static void Delete(FileInfo fileInfo)
        {
            if (File.Exists(fileInfo.FullName))
            {
                Debug.Log("Deleting File: " + fileInfo.FullName);
                File.Delete(fileInfo.FullName);
            }
        }
        public static void DeleteAll()
        {
            foreach(string filePath in Directory.GetFiles(DATA_PATH))
            {
                File.Delete(filePath);
            }
        }


        public static IEnumerable<string> GetSaveNames()
        {
            foreach(string path in Directory.GetFiles(DATA_PATH))
            {
                if (Path.GetExtension(path).EndsWith(FILE_EXTENSION))
                {
                    yield return Path.GetFileNameWithoutExtension(path);
                }
            }
        }
    }
}