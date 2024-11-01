using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Environment.Doors
{
    public abstract class Door : MonoBehaviour
    {
        private bool _canOpen = true;
        [SerializeField] private float _minToggleDelay = 0.0f;


        private bool m_isOpen = false;
        protected bool IsOpen
        {
            get => m_isOpen;
            set
            {
                m_isOpen = value;
                OnOpenStateChanged?.Invoke(value);
            }
        }

        public event System.Action<bool> OnOpenStateChanged;


        private const string DOOR_OPEN_STATE_ANIMATOR_BOOL_IDENTIFIER = "IsOpen";


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
            if (!_canOpen)
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
    }
}