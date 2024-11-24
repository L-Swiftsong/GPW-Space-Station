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
        private PlayerHide _playerHide;  

        public PlayerInventory Inventory => _playerInventory;
        public PlayerHide PlayerHide => _playerHide;

        private IInteractable m_currentInteractable = null;
        private IInteractable _currentInteractable
        {
            get => m_currentInteractable;
            set
            {
                if (value != m_currentInteractable)
                {
                    // Our current interactable has changed.
                    if (value == null)
                    {
                        // We have stopped looking at an interactable.
                        if (m_currentInteractable is KeycardReader)
                        {
                            // We were looking at a keycard reader.
                            (m_currentInteractable as KeycardReader).StopHighlighting();
                        }
                    }
                    else
                    {
                        // We are looking at a new interactable object
                        OnHighlightedInteractableObject?.Invoke();

                        if (value is KeycardReader)
                        {
                            // We are looking at a keycard reader.
                            (value as KeycardReader).Highlight();
                        }
                    }
                }

                m_currentInteractable = value;
            }
        }


        [Header("Layers")]
        public LayerMask interactableLayer;


        public static System.Action OnHighlightedInteractableObject;


        private void Start()
        {
            _playerInventory = GetComponent<PlayerInventory>();
            _playerHide = GetComponent<PlayerHide>();
        }
        private void OnEnable()
        {
            PlayerInput.OnInteractPerformed += PlayerInput_OnInteractPerformed;
        }
        private void OnDisable()
        {
            PlayerInput.OnInteractPerformed -= PlayerInput_OnInteractPerformed;
        }

        private void Update()
        {
            UpdateCurrentInteractable();
        }

        private void PlayerInput_OnInteractPerformed() => AttemptInteraction();


        private void UpdateCurrentInteractable()
        {
            if (_playerHide.isHiding && !_playerHide.isTransitioning)
            {
                // We are currently hiding, so cannot interact.
                return;
            }
        
        
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
            if (_playerHide.isHiding && !_playerHide.isTransitioning)
            {
                // We are wanting to exit a hiding spot.
                _playerHide.StopHiding();
                return;
            }

            if (_currentInteractable != null)
            {
                // Interact with our currently highlighted interactable.
                _currentInteractable.Interact(this);
            }
        }
    }
}