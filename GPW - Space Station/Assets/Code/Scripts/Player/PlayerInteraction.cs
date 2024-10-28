using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    [Header("References")]
    private PlayerInventory playerInventory;
    public Camera cam;
    private PlayerHide playerHide;  

    [Header("Layers")]
    public LayerMask interactableLayer;

    private void Start()
    {
        playerInventory = GetComponent<PlayerInventory>();
        playerHide = GetComponent<PlayerHide>();
    }

    private void Update()
    {
        HandleKeyCardPickup();
        HandleDoorInteraction();
        HandleHideInteraction();
    }

    private void HandleKeyCardPickup()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, 3f, interactableLayer))
            {
                KeyCard keyCard = hit.collider.GetComponent<KeyCard>();

                if (keyCard != null)
                {
                    playerInventory.AddKeyCard(keyCard.KeyCardId);
                    Destroy(keyCard.gameObject);
                    Debug.Log($"Picked up {keyCard.KeyCardId}");
                }
            }
            else
            {
                Debug.Log("Raycast did not hit any objects.");
            }
        }
    }

    private void HandleDoorInteraction()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, 3f, interactableLayer))
            {
                Door door = hit.collider.GetComponent<Door>();

                if (door != null)
                {
                    door.TryUnlockDoor(playerInventory);
                }
            }
            else
            {
                Debug.Log("Raycast did not hit any objects.");
            }
        }
    }

    private void HandleHideInteraction()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (playerHide.isHiding && !playerHide.isTransitioning)
            {
                StartCoroutine(playerHide.ExitHidingCoroutine());
            }
            else
            {
                RaycastHit hit;

                if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, 3f, interactableLayer))
                {
                    if (hit.collider.CompareTag("Hideable"))
                    {
                        StartCoroutine(playerHide.HideCoroutine(hit.collider.transform));
                    }

                }
                else
                {
                    Debug.Log("No hideable object found");
                }
            }


        }
    }
}


