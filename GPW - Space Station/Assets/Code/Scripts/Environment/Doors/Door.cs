using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Environment.Doors
{
    public abstract class Door : MonoBehaviour
    {
        [SerializeField] private bool _startOpen = false;
        private bool _canOpen = true;

        [SerializeField] private float _minToggleDelay = 0.0f;

        [SerializeField] private AudioSource _audioSource;

        [SerializeField] private AudioClip _doorOpenClip;

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
        public event System.Action<bool> OnOpenStateInstantChange;


        private void Awake()
        {
            m_isOpen = _startOpen;
            OnOpenStateInstantChange?.Invoke(m_isOpen);

			_audioSource = gameObject.AddComponent<AudioSource>();
			_audioSource.playOnAwake = false;
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

            PlaySound();

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

        private void PlaySound()
        {
            if (_audioSource != null && _doorOpenClip != null)
            {
                _audioSource.clip = _doorOpenClip;
                _audioSource.Play();
            }
        }
    }
}