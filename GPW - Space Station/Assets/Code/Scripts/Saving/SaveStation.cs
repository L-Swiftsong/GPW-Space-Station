using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Interaction;

namespace Saving
{
    public class SaveStation : MonoBehaviour, IInteractable
    {
        public void Interact(PlayerInteraction interactingScript)
        {
            Debug.Log("Manual Save");
            SaveManager.ManualSave();
        }
    }
}