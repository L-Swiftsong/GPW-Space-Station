using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    [Header("References")]
    private PlayerInventory playerInventory;
    public Camera cam;
    private PlayerHide playerHide;  

    public PlayerInventory Inventory => playerInventory;
    public PlayerHide PlayerHide => playerHide;


    [Header("Layers")]
    public LayerMask interactableLayer;

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

    private void PlayerInput_OnInteractPerformed() => AttemptInteraction();


    private void AttemptInteraction()
    {
        if (playerHide.isHiding && !playerHide.isTransitioning)
        {
            // We are wanting to exit a hiding spot.
            playerHide.StopHiding();
            return;
        }


        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out RaycastHit hit, 3f, interactableLayer))
        {
            // We found a potential interactable.

            if (hit.collider.TryGetComponentThroughParents<IInteractable>(out IInteractable interactableScript))
            {
                // We are interacting with an interactable.
                interactableScript.Interact(this);
            }
        }
    }
}