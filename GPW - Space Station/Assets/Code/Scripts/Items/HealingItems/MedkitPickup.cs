using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Interaction;

namespace Items.Healing
{
    public class MedkitPickup : ItemPickup
    {
        [Header("Settings")]
        [SerializeField] private int _medkitsContained = 1;


        protected override bool PerformInteraction(PlayerInteraction interactingScript)
        {
            interactingScript.Inventory.AddMedkits(_medkitsContained);
            return true;
        }
    }
}