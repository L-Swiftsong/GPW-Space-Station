using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Items;

public class PlayerInteraction : MonoBehaviour
{
    [Header("References")]
    private PlayerInventory playerInventory;
    public Camera cam;
    private PlayerHide playerHide;  

    public PlayerInventory Inventory => playerInventory;
    public PlayerHide PlayerHide => playerHide;

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
        playerInventory = GetComponent<PlayerInventory>();
        playerHide = GetComponent<PlayerHide>();
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
        if (playerHide.isHiding && !playerHide.isTransitioning)
        {
            // We are currently hiding, so cannot interact.
            return;
        }
        
        
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out RaycastHit hit, 3f, interactableLayer))
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
        if (playerHide.isHiding && !playerHide.isTransitioning)
        {
            // We are wanting to exit a hiding spot.
            playerHide.StopHiding();
            return;
        }

        if (_currentInteractable != null)
        {
            // Interact with our currently highlighted interactable.
            _currentInteractable.Interact(this);
        }
    }
}