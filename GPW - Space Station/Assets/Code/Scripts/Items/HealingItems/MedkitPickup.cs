using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Interaction;

namespace Items.Healing
{
    public class MedkitPickup : MonoBehaviour, IInteractable
    {
        [Header("Settings")]
        [SerializeField] private int _medkitsContained = 1;


        public void Interact(PlayerInteraction interactingScript)
        {
            interactingScript.Inventory.AddMedkits(_medkitsContained);
            Destroy(this.gameObject);
        }
    }
}