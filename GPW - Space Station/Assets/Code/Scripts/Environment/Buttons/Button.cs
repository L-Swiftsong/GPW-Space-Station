using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Interaction;
using UnityEngine.Events;

namespace Environment.Buttons
{
    public class Button : MonoBehaviour, IInteractable
    {
        public UnityEvent OnPressedEvent;


        public virtual void Interact(PlayerInteraction interactingPlayer)
        {
            OnPressedEvent?.Invoke();
        }
    }
}
