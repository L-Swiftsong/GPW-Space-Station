using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Saving.LevelData;

namespace Environment.Doors
{
    public abstract class Door : MonoBehaviour, ISaveableObject
    {
        #region Saving Properties

        [field: SerializeField] public SerializableGuid ID { get; set; }
        [SerializeField] private DoorSaveInformation _saveData;

        #endregion


        [SerializeField] private bool _startOpen = false;
        private bool _canOpen = true;

        [SerializeField] private float _minToggleDelay = 0.2f;

        private bool m_isOpen = false;
        protected bool IsOpen
        {
            get => m_isOpen;
            set
            {
                m_isOpen = value;
                _saveData.IsOpen = value;
                OnOpenStateChanged?.Invoke(value);
            }
        }

        public event System.Action<bool> OnOpenStateChanged;
        public event System.Action<bool> OnOpenStateInstantChange;


        protected virtual void ToggleOpen()
        {
            if (IsOpen)
                Close();
            else
                Open();
        }
        protected virtual void Open()
        {
            if (!_canOpen)
                return;


            IsOpen = true;

            // Handle minimum toggle time (If we want to prevent rapid toggling).
            if (_minToggleDelay > 0.0f)
            {
                _canOpen = false;
                Invoke(nameof(ResetCanOpen), _minToggleDelay);
            }
        }
        protected virtual void Close()
        {
            if (!_canOpen || !IsOpen)
                return;


            IsOpen = false;

            // Handle minimum toggle time (If we want to prevent rapid toggling).
            if (_minToggleDelay > 0.0f)
            {
                _canOpen = false;
                Invoke(nameof(ResetCanOpen), _minToggleDelay);
            }
        }

        private void ResetCanOpen() => _canOpen = true;


        #region Saving Functions

        public void BindExisting(ObjectSaveData saveData)
        {
            this._saveData = new DoorSaveInformation(saveData);
            _saveData.ID = ID;

            ISaveableObject.PerformBindingChecks(this._saveData.ObjectSaveData, this);

            m_isOpen = _saveData.IsOpen;
            OnOpenStateInstantChange?.Invoke(m_isOpen);
        }
        public ObjectSaveData BindNew()
        {
            if (this._saveData == null || !this._saveData.Exists)
            {
                this._saveData = new DoorSaveInformation(this.ID, ISaveableObject.DetermineDisabledState(this), _startOpen);
            }
            m_isOpen = _startOpen;
            OnOpenStateInstantChange?.Invoke(m_isOpen);

            ISaveableObject.UpdatePositionAndRotationInformation(this._saveData.ObjectSaveData, this);

            return this._saveData.ObjectSaveData;
        }

        protected virtual void OnEnable() => ISaveableObject.DefaultOnEnableSetting(this._saveData.ObjectSaveData, this);
        protected virtual void OnDestroy() => _saveData.DisabledState = DisabledState.Destroyed;
        protected virtual void OnDisable() => ISaveableObject.DefaultOnDisableSetting(this._saveData.ObjectSaveData, this);
        protected virtual void LateUpdate() => ISaveableObject.UpdatePositionAndRotationInformation(this._saveData.ObjectSaveData, this);

        #endregion
    }
}