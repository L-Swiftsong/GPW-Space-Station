using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Saving.LevelData;

namespace Entities.Mimic
{
    public abstract class BaseMimic : MonoBehaviour, ISaveableObject
    {
        [field: SerializeField] public SerializableInstanceGuid ID { get; set; } = SerializableInstanceGuid.NewUnlinkedGuid();
        [SerializeField] private MimicSaveInformation _saveData;



        public void BindExisting(ObjectSaveData saveData)
        {
            this._saveData = new MimicSaveInformation(saveData, ISaveableObject.DetermineDisabledState(this));
            _saveData.ID = ID;

            ISaveableObject.PerformBindingChecks(this._saveData.ObjectSaveData, this);
        }
        public ObjectSaveData BindNew()
        {
            if (this._saveData == null || !this._saveData.Exists)
            {
                this._saveData = new MimicSaveInformation(this.ID, ISaveableObject.DetermineDisabledState(this), GetSavableState());
            }

            return this._saveData.ObjectSaveData;
        }

        protected abstract MimicSavableState GetSavableState();
        protected void UpdateSaveableState() => this._saveData.MimicSavableState = GetSavableState();

        protected virtual void OnEnable() => ISaveableObject.DefaultOnEnableSetting(this._saveData.ObjectSaveData, this);
        protected virtual void OnDestroy() => _saveData.DisabledState = DisabledState.Destroyed;
        protected virtual void OnDisable() => ISaveableObject.DefaultOnDisableSetting(this._saveData.ObjectSaveData, this);
        protected virtual void LateUpdate() => ISaveableObject.UpdatePositionAndRotationInformation(this._saveData.ObjectSaveData, this);

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
