using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Saving.LevelData;

namespace Environment.Doors
{
    public abstract class Door : MonoBehaviour, ISaveableObject
    {
        #region Saving Properties

        [field: SerializeField] public SerializableGuid ID { get; set; } = SerializableGuid.NewGuid();
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


        private void Start()
        {
            m_isOpen = _startOpen;
            OnOpenStateInstantChange?.Invoke(m_isOpen);
		}


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

            if (_saveData.WasDestroyed)
            {
                Destroy(this.gameObject);
            }

            IsOpen = _saveData.IsOpen;
        }
        public ObjectSaveData BindNew()
        {
            if (this._saveData == null || !this._saveData.Exists)
            {
                this._saveData = new DoorSaveInformation(this.ID, IsOpen);
            }

            return this._saveData.ObjectSaveData;
        }
        private void OnDestroy() => _saveData.WasDestroyed = true;

        #endregion
    }
}