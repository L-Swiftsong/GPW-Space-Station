using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Saving.LevelData
{
    public class ExampleLevelObject : MonoBehaviour, ISaveableObject
    {
#region Saving Information

        [field: SerializeField] public SerializableInstanceGuid ID { get; set; } = SerializableInstanceGuid.NewUnlinkedGuid();
        [SerializeField] private DoorSaveInformation _saveData;

#endregion


        [SerializeField] private bool _isOpen;
        

        public void BindExisting(ObjectSaveData saveData)
        {
            this._saveData = new DoorSaveInformation(saveData);
            _saveData.ID = ID;

            if (_saveData.WasDestroyed)
            {
                Destroy(this.gameObject);
            }

            _isOpen = _saveData.IsOpen;
        }
        public ObjectSaveData BindNew()
        {
            if (this._saveData == null || !this._saveData.Exists)
            {
                this._saveData = new DoorSaveInformation(this.ID, _isOpen);
            }

            return this._saveData.ObjectSaveData;
        }
        private void LateUpdate()
        {
            _saveData.IsOpen = _isOpen;
        }
        private void OnDestroy() => _saveData.WasDestroyed = true;
        public void InitialiseID() => ID.LinkGuidToGameObject(this.gameObject);

#if UNITY_EDITOR

        private void OnValidate()
        {
            // Initialise our Guid ID.
            if (ID.IsUnlinked())
            {
                InitialiseID();
            }
        }

#endif
    }
}