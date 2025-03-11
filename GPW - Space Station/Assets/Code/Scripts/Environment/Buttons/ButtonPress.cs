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

        #region IInteractable Properties & Events

        private int _previousLayer;

        public event System.Action OnSuccessfulInteraction;
        public event System.Action OnFailedInteraction;

        #endregion


        public void Interact(PlayerInteraction playerInteraction)
        {
            Debug.Log(gameObject.name + " pressed");
            OnButtonPressed?.Invoke(this);
            OnSuccessfulInteraction?.Invoke();
        }
        public void Highlight() => IInteractable.StartHighlight(this.gameObject, ref _previousLayer);
        public void StopHighlighting() => IInteractable.StopHighlight(this.gameObject, _previousLayer);
    }
}