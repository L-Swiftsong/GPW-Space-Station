using Interaction;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Items
{
    public abstract class ItemPickup : MonoBehaviour, IInteractable
    {
        #region IInteractable Properties & Events

        private int _previousLayer;

        public event System.Action OnSuccessfulInteraction;
        public event System.Action OnFailedInteraction;

        #endregion


        public void Interact(PlayerInteraction interactingScript)
        {
            if (PerformInteraction(interactingScript))
            {
                OnSuccessfulInteraction?.Invoke();
                Destroy(this.gameObject);
            }
            else
            {
                OnFailedInteraction?.Invoke();
            }
        }
        public void Highlight() => IInteractable.StartHighlight(this.gameObject, ref _previousLayer);
        public void StopHighlighting() => IInteractable.StopHighlight(this.gameObject, _previousLayer);

        protected abstract bool PerformInteraction(PlayerInteraction interactingScript);
    }
}
