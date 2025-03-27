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
        private int _sceneBuildIndex = -1;

        [SerializeField] private Component[] _saveableObjects = new Component[0];


        private static event System.Action<LevelSaveData> OnLoadLevelSave;


        public static void LoadLevelSaves(LevelSaveData[] levelSaveDatas)
        {
            for(int i = 0; i < levelSaveDatas.Length; ++i)
            {
                OnLoadLevelSave(levelSaveDatas[i]);
            }
        }
        public static LevelSaveData[] GetAllExistingSaveData() => s_sceneIndexToSaveDataDictionary.Values.ToArray();


        #region Editor Only
        #if UNITY_EDITOR

        public void Editor_FindAllSaveableObjects(bool debugFoundObjects)
        {
            if (Editor_IsSceneInvalid())
            {
                Debug.LogError("ERROR: The LevelDataManager instance is in a scene that is not a part of the build.");
                return;
            }

            _sceneBuildIndex = this.gameObject.scene.buildIndex;
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

            SetupSaveData();
            OnLoadLevelSave += TryLoadSaveData;

            if (s_sceneIndexToSaveDataDictionary.TryAdd(_sceneBuildIndex, _saveData) == false)
            {
                s_sceneIndexToSaveDataDictionary[_sceneBuildIndex] = this._saveData;
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
                catch
                {
                    Debug.LogError("Error encountered when trying to bind ISaveable.\nEnsure that you haven't deleted a GameObject with an ISaveable component without regenerating the list in the LevelDataManager.");
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
            Debug.Log($"Loading data for {_saveableObjects.Length} objects.");

            for (int i = 0; i < levelSaveData.ObjectSaveData.Length; ++i)
            {
                Component foundSaveable = _saveableObjects.Where(s => (s as ISaveableObject).ID == levelSaveData.ObjectSaveData[i].ID).FirstOrDefault();
                if (foundSaveable != null)
                {
                    (foundSaveable as ISaveableObject).BindExisting(levelSaveData.ObjectSaveData[i]);
                }
                else
                {
                    Debug.LogError("Failed to find object for ID: " + levelSaveData.ObjectSaveData[i].ID);
                }
            }
        }

        #region Testing

        [ContextMenu("Tests/Load")]
        private void LoadTestSaveData()
        {
            LoadSaveData(_saveData);
        }

        #endregion
    }
}