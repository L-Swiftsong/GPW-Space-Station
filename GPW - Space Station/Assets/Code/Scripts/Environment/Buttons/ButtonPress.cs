using Environment.Doors;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Interaction;

namespace Environment.Buttons
{
    public class SequenceButton : MonoBehaviour, IInteractable
    {
        public event System.Action<SequenceButton> OnButtonPressed;

        public void Interact(PlayerInteraction playerInteraction)
        {
            Debug.Log(gameObject.name + " pressed");
            OnButtonPressed?.Invoke(this);
        }
    }
}