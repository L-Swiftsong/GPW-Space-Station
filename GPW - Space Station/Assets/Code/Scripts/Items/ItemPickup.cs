using Interaction;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Items
{
    public abstract class ItemPickup : MonoBehaviour, IInteractable
    {
        public event System.Action OnSuccessfulInteraction;
        public event System.Action OnFailedInteraction;


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

        protected abstract bool PerformInteraction(PlayerInteraction interactingScript);
    }
}
