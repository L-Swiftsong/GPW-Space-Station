using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Items;

namespace Interaction
{
    public class PlayerInteraction : ProtectedSingleton<PlayerInteraction>
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
                    if (m_currentInteractable != null && !m_currentInteractable.Equals(null))
                    {
                        m_currentInteractable.StopHighlighting();
                    }

                    if (value != null && !value.Equals(null))
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


        [Header("Settings")]
        [SerializeField] private float _interactionRange = 3.0f;
        [SerializeField] private LayerMask _interactableObstructionLayers = 1 << 0 | 1 << 6 | 1 << 7;
        [SerializeField] private LayerMask _interactableLayers = 1 << 9 | 1 << 10 | 1 << 11;


        [Header("Temp")]
        [SerializeField] private bool _displayAudioSourceWarnings = false;


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
            // Find all potential interactables, and order them based on their distance to the player camera (Ascending).
            IEnumerable<RaycastHit> potentialInteractables = Physics.RaycastAll(_playerCamera.transform.position, _playerCamera.transform.forward, _interactionRange, _interactableLayers, QueryTriggerInteraction.Collide).OrderBy(t => (t.transform.position - _playerCamera.transform.position).sqrMagnitude);
            for (int i = 0; i < potentialInteractables.Count(); ++i)
            {
                if (potentialInteractables.ElementAt(i).collider.TryFindFirstWithCondition<IInteractable>((interactable) => interactable.IsInteractable, out IInteractable interactableScript))
                {
                    // This is an active interactable.
                    if (Physics.Linecast(_playerCamera.transform.position, potentialInteractables.ElementAt(i).point, _interactableObstructionLayers, QueryTriggerInteraction.Ignore))
                    {
                        // There is an obstruction between this and the player.
                        break;
                    }

                    _currentInteractable = interactableScript;
                    return;
                }
                else if (potentialInteractables.ElementAt(i).collider.isTrigger == false)
                {
                    // This object's collider is not a trigger, and therefore we shouldn't be able to interact through it.
                    break;
                }
            }

            // There were no valid interactables.
            _currentInteractable = null;
        }
        private void AttemptInteraction()
        {
            if (_currentInteractable != null)
            {
                // Interact with our currently highlighted interactable.
                _currentInteractable.Interact(this);

                //PlayInteractionSound(_currentInteractable);
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
                else if (_displayAudioSourceWarnings)
                {
                    Debug.LogWarning("No AudioSource or AudioClip found on interactable object!", interactableMono);
                }
            }
        }


        public static void SetCurrentInteractableOverride(IInteractable interactableOverride)
        {
            if (PlayerInteraction.HasInstance)
            {
                PlayerInteraction.Instance.SetCurrentInteractableOverride_Local(interactableOverride);
            }
        }
        public static void ResetCurrentInteractableOverride()
        {
            if (PlayerInteraction.HasInstance)
            {
                PlayerInteraction.Instance.ResetCurrentInteractableOverride_Local();
            }
        }

        private void SetCurrentInteractableOverride_Local(IInteractable interactableOverride)
        {
            _currentInteractableOverride = interactableOverride;

            OnHighlightedInteractableObject?.Invoke();
            _currentInteractableOverride.Highlight();
        }
        private void ResetCurrentInteractableOverride_Local()
        {
            _currentInteractableOverride.StopHighlighting();
            _currentInteractableOverride = null;
        }
    }
}