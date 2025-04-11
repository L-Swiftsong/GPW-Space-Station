using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Saving.LevelData
{
    public class ExampleLevelObject : MonoBehaviour, ISaveableObject
    {
        #region Saving Information

        [field: SerializeField] public SerializableGuid ID { get; set; }
        [SerializeField] private DoorSaveInformation _saveData;

#endregion


        [SerializeField] private bool _isOpen;
        

        public void BindExisting(ObjectSaveData saveData)
        {
            this._saveData = new DoorSaveInformation(saveData, ISaveableObject.DetermineDisabledState(this));
            _saveData.ID = ID;

            ISaveableObject.PerformBindingChecks(this._saveData.ObjectSaveData, this);

            _isOpen = _saveData.IsOpen;
        }
        public ObjectSaveData BindNew()
        {
            if (this._saveData == null || !this._saveData.Exists)
            {
                this._saveData = new DoorSaveInformation(this.ID, ISaveableObject.DetermineDisabledState(this), _isOpen);
            }

            ISaveableObject.UpdatePositionAndRotationInformation(this._saveData.ObjectSaveData, this);

            return this._saveData.ObjectSaveData;
        }
        private void LateUpdate()
        {
            _saveData.IsOpen = _isOpen;
            ISaveableObject.UpdatePositionAndRotationInformation(this._saveData.ObjectSaveData, this);
        }

        private void OnEnable() => ISaveableObject.DefaultOnEnableSetting(this._saveData.ObjectSaveData, this);
        private void OnDestroy() => _saveData.DisabledState = DisabledState.Destroyed;
        private void OnDisable() => ISaveableObject.DefaultOnDisableSetting(this._saveData.ObjectSaveData, this);
    }
}