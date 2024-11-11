using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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