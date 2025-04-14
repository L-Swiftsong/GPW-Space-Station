using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Saving.LevelData;

namespace Entities.Mimic
{
    public abstract class BaseMimic : MonoBehaviour, ISaveableObject
    {
        [field: SerializeField] public SerializableGuid ID { get; set; }
        [SerializeField] private MimicSaveInformation _saveData;



        public void BindExisting(ObjectSaveData saveData)
        {
            this._saveData = new MimicSaveInformation(saveData);
            _saveData.ID = ID;

            ISaveableObject.PerformBindingChecks(this._saveData.ObjectSaveData, this);
        }
        public ObjectSaveData BindNew()
        {
            if (this._saveData == null || !this._saveData.Exists)
            {
                this._saveData = new MimicSaveInformation(this.ID, ISaveableObject.DetermineDisabledState(this), GetSavableState());
            }

            ISaveableObject.UpdatePositionAndRotationInformation(this._saveData.ObjectSaveData, this);

            return this._saveData.ObjectSaveData;
        }

        protected abstract MimicSavableState GetSavableState();
        protected void UpdateSaveableState()
        {
            if (this._saveData != null && this._saveData.ObjectSaveData.IntValues.Length >= 1)
                this._saveData.MimicSavableState = GetSavableState();
        }

        protected virtual void OnEnable() => ISaveableObject.DefaultOnEnableSetting(this._saveData.ObjectSaveData, this);
        protected virtual void OnDestroy() => _saveData.DisabledState = DisabledState.Destroyed;
        protected virtual void OnDisable() => ISaveableObject.DefaultOnDisableSetting(this._saveData.ObjectSaveData, this);
        protected virtual void LateUpdate()
        {
            try
            {
                ISaveableObject.UpdatePositionAndRotationInformation(this._saveData.ObjectSaveData, this);
            }
            catch
            {
                Debug.Log("Mimic", this);
            }
        }



        public virtual void Activate() => this.gameObject.SetActive(true);
        public virtual void Deactivate() => this.gameObject.SetActive(false);

        public virtual void SetPositionAndRotation(Vector3 position, Quaternion rotation)
        {
            transform.position = position;
            transform.rotation = rotation;
        }
    }
}
