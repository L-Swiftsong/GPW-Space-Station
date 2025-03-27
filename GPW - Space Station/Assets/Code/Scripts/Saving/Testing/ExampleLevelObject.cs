using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Saving.LevelData
{
    /*public class ExampleLevelObject : MonoBehaviour, ISaveable
    {
        [field: SerializeField] public SerializableGuid ID { get; set; } = SerializableGuid.NewGuid();
        [SerializeField] private bool _isLocked;
        [SerializeField] private bool _isOpen;


        public ObjectSaveData GetSaveData() => new ObjectSaveData()
        {
            ID = this.ID,
            WasDestroyed = false,
            IsLocked = _isLocked,
            IsOpen = _isOpen,
        };

        public void LoadSaveData(ObjectSaveData saveData)
        {
            if (saveData.WasDestroyed)
            {
                Destroy(this.gameObject);
                return;
            }

            _isLocked = saveData.IsLocked;
            _isOpen = saveData.IsOpen;
        }
    }*/
    public class ExampleLevelObject : MonoBehaviour, ISaveableObject
    {
        #region Saving Information

        [field: SerializeField] public SerializableGuid ID { get; set; } = SerializableGuid.NewGuid();
        [SerializeField] private ObjectSaveData _saveData;
        private const int IS_LOCKED_INDEX = 0;
        private const int IS_OPEN_INDEX = 1;

#endregion


        [SerializeField] private bool _isOpen;


        public void BindExisting(ObjectSaveData saveData)
        {
            this._saveData = saveData;
            _saveData.ID = ID;

            if (_saveData.WasDestroyed)
            {
                Destroy(this.gameObject);
            }

            _isOpen = (_saveData.SaveInformation as DoorSaveInformation).IsOpen;
        }
        public ObjectSaveData BindNew()
        {
            if (this._saveData == null || !this._saveData.Exists)
            {
                this._saveData = new ObjectSaveData()
                {
                    ID = this.ID,
                    Exists = true,
                    SaveInformation = CreateSavedBools(),
                };
            }

            return this._saveData;


            ObjectSaveInformation CreateSavedBools()
            {
                return new DoorSaveInformation()
                {
                    IsOpen = _isOpen,
                };
            }
        }
        private void LateUpdate()
        {
            (_saveData.SaveInformation as DoorSaveInformation).IsOpen = _isOpen;
        }
        private void OnDestroy() => _saveData.WasDestroyed = true;
    }
}