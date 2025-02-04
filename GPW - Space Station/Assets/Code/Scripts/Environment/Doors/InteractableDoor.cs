using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Interaction;
using System;

namespace Environment.Doors
{
    public class InteractableDoor : Door, IInteractable
    {
        private bool _wasOpenedFromFacingDirection = true;
        public bool WasOpenedFromFacingDirection => _wasOpenedFromFacingDirection;


        public event Action OnSuccessfulInteraction;
        public event Action OnFailedInteraction;


        public void Interact(PlayerInteraction interactingScript)
        {
            Vector3 directionToInteractor = (interactingScript.transform.position - transform.position).normalized;
            _wasOpenedFromFacingDirection = Vector3.Dot(directionToInteractor, transform.forward) > 0.0f;
            
            ToggleOpen();

            OnSuccessfulInteraction?.Invoke();
        }
    }
}