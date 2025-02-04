using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Interaction;

namespace Saving
{
    public class SaveStation : MonoBehaviour, IInteractable
    {
        public event System.Action OnSuccessfulInteraction;
        public event System.Action OnFailedInteraction;


        public void Interact(PlayerInteraction interactingScript)
        {
            Debug.Log("Manual Save");
            SaveManager.ManualSave();

            OnSuccessfulInteraction?.Invoke();
        }
    }
}