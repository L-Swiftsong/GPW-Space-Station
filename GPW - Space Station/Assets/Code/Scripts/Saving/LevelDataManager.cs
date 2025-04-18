using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Saving.LevelData
{
    public class LevelDataManager : MonoBehaviour
    {
        private static Dictionary<int, LevelSaveData> s_sceneIndexToSaveDataDictionary = new Dictionary<int, LevelSaveData>();
        [SerializeField] private LevelSaveData _saveData = new LevelSaveData();
        private int _sceneBuildIndex => gameObject.scene.buildIndex;

        [SerializeField] private Component[] _saveableObjects;


        private static event System.Action<LevelSaveData> OnLoadLevelSave;

        public static void LoadLevelSaves(LevelSaveData[] levelSaveDatas)
        {
            for(int i = 0; i < levelSaveDatas.Length; ++i)
            {
                OnLoadLevelSave(levelSaveDatas[i]);
            }
        }
        public static LevelSaveData[] GetAllExistingSaveData() => s_sceneIndexToSaveDataDictionary.Values.ToArray();
        public static void PrepareForSave()
        {
            foreach (LevelSaveData levelSaveData in s_sceneIndexToSaveDataDictionary.Values)
            {
                for (int i = 0; i < levelSaveData.ObjectSaveData.Length; ++i)
                {
                    levelSaveData.ObjectSaveData[i].OnBeforeSave();
                }
            }
        }
        public static void OnAfterSave()
        {
            foreach(LevelSaveData levelSaveData in s_sceneIndexToSaveDataDictionary.Values)
            {
                for(int i = 0; i < levelSaveData.ObjectSaveData.Length; ++i)
                {
                    levelSaveData.ObjectSaveData[i].OnAfterSave();
                }
            }
        }

        public static void ClearSaveDataForNewGame() => s_sceneIndexToSaveDataDictionary = new Dictionary<int, LevelSaveData>();


        #region Editor Only
        #if UNITY_EDITOR

        public void Editor_FindAllSaveableObjects(bool debugFoundObjects)
        {
            if (Editor_IsSceneInvalid())
            {
                Debug.LogError("ERROR: The LevelDataManager instance is in a scene that is not a part of the build.");
                return;
            }

            _saveData.SceneBuildIndex = _sceneBuildIndex;

            List<Component> saveableObjectsAsComponents = new List<Component>();
            foreach (GameObject rootGameObject in this.gameObject.scene.GetRootGameObjects())
            {
                foreach(Component saveableComponent in rootGameObject.GetComponentsInChildren<ISaveableObject>(includeInactive: true))
                {
                    saveableObjectsAsComponents.Add(saveableComponent);
                }
            }

            _saveableObjects = saveableObjectsAsComponents.ToArray();

            // Debug.
            Debug.Log($"Found {_saveableObjects.Length} Saveable Objects.");
            if (debugFoundObjects)
            {
                foreach (var saveableObject in _saveableObjects)
                {
                    Debug.Log("ISaveable Instance on GameObject: " + saveableObject.gameObject.name);
                }
            }
        }

        public bool Editor_AreAnySaveableObjectsInvalid()
        {
            if (_saveableObjects == null)
            {
                return true;
            }

            for(int i = 0; i < _saveableObjects.Length; ++i)
            {
                if (_saveableObjects[i] == null || _saveableObjects[i] is not ISaveableObject)
                {
                    return true;
                }
            }

            return false;
        }
        public bool Editor_IsSceneInvalid() => gameObject.scene.buildIndex == -1 || gameObject.scene.buildIndex >= UnityEngine.SceneManagement.SceneManager.sceneCountInBuildSettings;

#endif
        #endregion


        private void Awake()
        {
            if (_saveableObjects == null)
            {
                Debug.LogWarning("Saveable Objects not initialised");
            }
            
            OnLoadLevelSave += TryLoadSaveData;
        }

        private void Start()
        {
            if (s_sceneIndexToSaveDataDictionary.ContainsKey(_sceneBuildIndex) == false)
            {
                SetupSaveData();
                s_sceneIndexToSaveDataDictionary.Add(_sceneBuildIndex, _saveData);
            }
            else
            {
                this._saveData = s_sceneIndexToSaveDataDictionary[_sceneBuildIndex];
            }
        }
        private void OnDestroy() => OnLoadLevelSave -= TryLoadSaveData;
        private void SetupSaveData()
        {
            _saveData = new LevelSaveData();
            _saveData.SceneBuildIndex = _sceneBuildIndex;

            Debug.Log($"Performing initial bindings for {_saveableObjects.Length} objects.");

            ObjectSaveData[] saveDatas = new ObjectSaveData[_saveableObjects.Length];
            int i = 0;
            foreach(var saveable in _saveableObjects)
            {
                try
                {
                    saveDatas[i] = (saveable as ISaveableObject).BindNew();
                }
                catch(System.InvalidCastException e)
                {
                    Debug.LogError("Invalid cast occured when trying to bind ISaveable.", saveable);
                    Debug.LogError(e, saveable);
                }
                catch (System.Exception e)
                {
                    Debug.LogError("Error encountered when trying to bind ISaveable.\nEnsure that you haven't deleted a GameObject with an ISaveable component without regenerating the list in the LevelDataManager.", saveable);
                    Debug.LogError(e, saveable);
                }
                ++i;
            }

            _saveData.ObjectSaveData = saveDatas;
        }
        private void TryLoadSaveData(LevelSaveData levelSaveData)
        {
            if (levelSaveData.SceneBuildIndex == _sceneBuildIndex)
            {
                LoadSaveData(levelSaveData);
            }
        }
        public void LoadSaveData(LevelSaveData levelSaveData)
        {
            if (levelSaveData.SceneBuildIndex != _sceneBuildIndex)
            {
                throw new System.ArgumentException("LevelSaveData scene build index doesn't match LevelDataManager's scene index.");
            }

            this._saveData = levelSaveData;
            if (s_sceneIndexToSaveDataDictionary.TryAdd(_sceneBuildIndex, _saveData) == false)
            {
                // Entry for this Scene ID already exists.
                s_sceneIndexToSaveDataDictionary[_sceneBuildIndex] = this._saveData;
            }

            Debug.Log($"Loading data for {_saveableObjects.Length} objects.");

            for (int i = 0; i < levelSaveData.ObjectSaveData.Length; ++i)
            {
                levelSaveData.ObjectSaveData[i].OnBeforeLoad();

                try
                {
                    Component foundSaveable = _saveableObjects.Where(s => (s as ISaveableObject).ID == levelSaveData.ObjectSaveData[i].ID).FirstOrDefault();
                    if (foundSaveable != null)
                    {
                        //Debug.Log($"Binding for Object with ID {levelSaveData.ObjectSaveData[i].ID.ToString()}", foundSaveable);
                        (foundSaveable as ISaveableObject).BindExisting(levelSaveData.ObjectSaveData[i]);
                    }
                    else
                    {
                        Debug.LogError("Failed to find object for ID: " + levelSaveData.ObjectSaveData[i].ID);
                    }
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"Failed to Bind Data for Saveable Data Entry: {i}. (ID: {levelSaveData.ObjectSaveData[i].ID})");
                    Debug.LogError(e);
                }
            }
        }
    }
}