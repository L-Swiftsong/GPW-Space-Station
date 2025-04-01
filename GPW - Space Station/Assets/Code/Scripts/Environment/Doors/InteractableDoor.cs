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


        #region IInteractable Properties & Events

        [field: SerializeField] public bool IsInteractable { get; set; } = true;

        private int _previousLayer;

        public event System.Action OnSuccessfulInteraction;
        public event System.Action OnFailedInteraction;

        #endregion


        public void Interact(PlayerInteraction interactingScript)
        {
            Vector3 directionToInteractor = (interactingScript.transform.position - transform.position).normalized;
            _wasOpenedFromFacingDirection = Vector3.Dot(directionToInteractor, transform.forward) > 0.0f;
            
            ToggleOpen();

            OnSuccessfulInteraction?.Invoke();
        }
        public void Highlight() => IInteractable.StartHighlight(this.gameObject, ref _previousLayer);
        public void StopHighlighting() => IInteractable.StopHighlight(this.gameObject, _previousLayer);
    }
}