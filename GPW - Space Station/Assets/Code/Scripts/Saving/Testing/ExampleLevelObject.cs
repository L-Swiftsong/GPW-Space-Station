using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Saving
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
        [field: SerializeField] public SerializableGuid ID { get; set; } = SerializableGuid.NewGuid();
        [SerializeField] private ObjectSaveData _saveData;
        [SerializeField] private bool _isLocked;
        [SerializeField] private bool _isOpen;

        public void BindExisting(ObjectSaveData saveData)
        {
            this._saveData = saveData;
            _saveData.ID = ID;

            if (_saveData.WasDestroyed)
            {
                Destroy(this.gameObject);
            }

            _isLocked = _saveData.IsLocked;
            _isOpen = _saveData.IsOpen;
        }
        public ObjectSaveData BindNew()
        {
            if (this._saveData == null || !this._saveData.Exists)
            {
                this._saveData = new ObjectSaveData()
                {
                    ID = this.ID,
                    Exists = true,
                    IsLocked = this._isLocked,
                    IsOpen = this._isOpen,
                };
            }

            return this._saveData;
        }
        private void LateUpdate()
        {
            _saveData.IsLocked = _isLocked;
            _saveData.IsOpen = _isOpen;
        }
        private void OnDestroy() => _saveData.WasDestroyed = true;
    }
}