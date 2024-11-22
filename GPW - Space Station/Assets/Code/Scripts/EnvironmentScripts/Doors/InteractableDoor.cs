using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Environment.Doors
{
    public class InteractableDoor : Door, IInteractable
    {
        private bool _wasOpenedFromFacingDirection = true;
        public bool WasOpenedFromFacingDirection => _wasOpenedFromFacingDirection;


        public void Interact(PlayerInteraction interactingScript)
        {
            Vector3 directionToInteractor = (interactingScript.transform.position - transform.position).normalized;
            _wasOpenedFromFacingDirection = Vector3.Dot(directionToInteractor, transform.forward) > 0.0f;
            ToggleOpen();
        }
    }
}