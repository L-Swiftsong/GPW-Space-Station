using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Environment.Doors
{
    public class InteractableDoor : Door, IInteractable
    {
        [SerializeField] private int _requiredKeycardID = -1;
        private bool _isLocked = true;


        private bool _wasOpenedFromFacingDirection = true;
        public bool WasOpenedFromFacingDirection => _wasOpenedFromFacingDirection;

        public int RequiredKeycardID => _requiredKeycardID;

        private PlayerInventory playerInventory;


        private void Start()
        {
            // A 'RequiredKeycardID' of -1 means that this door starts unlocked.
            _isLocked = _requiredKeycardID != -1;

            //References
            playerInventory = FindObjectOfType<PlayerInventory>();
        }

        private void Update()
        {
            
        }


        public void Interact(PlayerInteraction interactingScript)
        {
            if (_isLocked)
            {
                TryUnlockDoor(interactingScript.Inventory);
            }
            else
            {
                Vector3 directionToInteractor = (interactingScript.transform.position - transform.position).normalized;
                _wasOpenedFromFacingDirection = Vector3.Dot(directionToInteractor, transform.forward) > 0.0f;
                ToggleOpen();
            }
        }

        private void TryUnlockDoor(PlayerInventory playerInventory)
        {
            if (playerInventory.HasKeyCard(_requiredKeycardID))
            {
                if (_requiredKeycardID == 2 && playerInventory.blueKeyCard.activeSelf) //Checks if required keyCard is equipped
                {
                    _isLocked = false;
                }
                else if (_requiredKeycardID == 1 && playerInventory.greenKeyCard.activeSelf)
                {
                    _isLocked = false;
                }
                else if (_requiredKeycardID == 3 && playerInventory.redKeyCard.activeSelf)
                {
                    _isLocked = false;
                }
                else if (_requiredKeycardID == 0 && playerInventory.blueKeyCard2.activeSelf)
                {
                    _isLocked = false;
                }
            }
        }
    }
}