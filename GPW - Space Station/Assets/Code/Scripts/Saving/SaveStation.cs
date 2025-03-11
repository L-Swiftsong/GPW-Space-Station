using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Interaction;

namespace Saving
{
    public class SaveStation : MonoBehaviour, IInteractable
    {
        #region IInteractable Properties & Events

        private int _previousLayer;

        public event System.Action OnSuccessfulInteraction;
        public event System.Action OnFailedInteraction;

        #endregion


        public void Interact(PlayerInteraction interactingScript)
        {
            Debug.Log("Manual Save");
            SaveManager.ManualSave();

            OnSuccessfulInteraction?.Invoke();
        }
        public void Highlight() => IInteractable.StartHighlight(this.gameObject, ref _previousLayer);
        public void StopHighlighting() => IInteractable.StopHighlight(this.gameObject, _previousLayer);
    }
}