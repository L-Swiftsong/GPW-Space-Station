using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Items;

namespace Interaction
{
    public class PlayerInteraction : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Camera _playerCamera;


        private PlayerInventory _playerInventory;
        public PlayerInventory Inventory => _playerInventory;


        private IInteractable m_currentInteractable = null;
        private IInteractable _currentInteractable
        {
            get => _currentInteractableOverride != null ? _currentInteractableOverride : m_currentInteractable;
            set
            {
                if (value != m_currentInteractable)
                {
                    // Our current interactable has changed.
                    if (value == null)
                    {
                        // We have stopped looking at an interactable.
                        m_currentInteractable.StopHighlighting();
                    }
                    else
                    {
                        // We are looking at a new interactable object
                        OnHighlightedInteractableObject?.Invoke();
                        value.Highlight();
                    }
                }

                m_currentInteractable = value;
            }
        }
        private IInteractable _currentInteractableOverride = null;


        [Header("Layers")]
        public LayerMask interactableLayer;


        public static System.Action OnHighlightedInteractableObject;


        private void Start()
        {
            _playerInventory = GetComponent<PlayerInventory>();
        }
        private void OnEnable() => PlayerInput.OnInteractPerformed += PlayerInput_OnInteractPerformed;
        private void OnDisable() => PlayerInput.OnInteractPerformed -= PlayerInput_OnInteractPerformed;
        

        private void Update()
        {
            UpdateCurrentInteractable();
        }

        private void PlayerInput_OnInteractPerformed() => AttemptInteraction();


        private void UpdateCurrentInteractable()
        {
            if (Physics.Raycast(_playerCamera.transform.position, _playerCamera.transform.forward, out RaycastHit hit, 3f, interactableLayer))
            {
                // We found a potential interactable.

                if (hit.collider.TryGetComponentThroughParents<IInteractable>(out IInteractable interactableScript))
                {
                    // This is an interactable.
                    _currentInteractable = interactableScript;
                }
                else
                {
                    _currentInteractable = null;
                }
            }
            else
            {
                _currentInteractable = null;
            }
        }
        private void AttemptInteraction()
        {
            if (_currentInteractable != null)
            {
                // Interact with our currently highlighted interactable.
                _currentInteractable.Interact(this);

                PlayInteractionSound(_currentInteractable);
            }
        }
        private void PlayInteractionSound(IInteractable interactable)
        {
            // Try to get an AudioSource component from the interactable object.
            MonoBehaviour interactableMono = interactable as MonoBehaviour;
            if (interactableMono != null)
            {
                AudioSource audioSource = interactableMono.GetComponent<AudioSource>();
                if (audioSource != null && audioSource.clip != null)
                {
                    audioSource.Play();
                }
                else
                {
                    Debug.LogWarning("No AudioSource or AudioClip found on interactable object!", interactableMono);
                }
            }
        }

            public void SetCurrentInteractableOverride(IInteractable interactableOverride)
        {
            _currentInteractableOverride = interactableOverride;

            OnHighlightedInteractableObject?.Invoke();
            _currentInteractableOverride.Highlight();
        }
        public void ResetCurrentInteractableOverride()
        {
            _currentInteractableOverride.StopHighlighting();
            _currentInteractableOverride = null;
        }
    }
}