using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Saving
{
    public class LevelDataManager : MonoBehaviour
    {
        [SerializeField] private LevelSaveData _saveData = new LevelSaveData();


        [SerializeField, ReadOnly] private Component[] _saveableObjects;


        #region Editor Only
        #if UNITY_EDITOR

        public void Editor_FindAllSaveableObjects(bool debugFoundObjects)
        {
            if (Editor_IsSceneInvalid())
            {
                Debug.LogError("ERROR: The LevelDataManager instance is in a scene that is not a part of the build.");
                return;
            }


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
        }
        private void SetupSaveData()
        {
            _saveData = new LevelSaveData();
            _saveData.SceneBuildIndex = gameObject.scene.buildIndex;

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
        public LevelSaveData GetSaveData() => _saveData;
        public void LoadSaveData(LevelSaveData levelSaveData)
        {
            if (levelSaveData.SceneBuildIndex != gameObject.scene.buildIndex)
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
    [System.Serializable]
    public class LevelSaveData
    {
        public int SceneBuildIndex = -1;
        public ObjectSaveData[] ObjectSaveData;
    }
    [System.Serializable]
    public class ObjectSaveData
    {
        [field: SerializeField] public SerializableGuid ID { get; set; }
        [ReadOnly] public bool Exists = false;
        public bool WasDestroyed = false;
        [Space(5)]

        public bool IsLocked;
        public bool IsOpen;
    }
}