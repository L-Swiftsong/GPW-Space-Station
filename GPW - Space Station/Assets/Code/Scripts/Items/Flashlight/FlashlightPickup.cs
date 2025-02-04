using UnityEngine;
using Interaction;

namespace Items.Flashlight
{
    /// <summary> A collectable that gives the player a flashlight.</summary>
    public class FlashlightPickup : ItemPickup
    {
        [SerializeField] private float _startingBattery = 100.0f;

        protected override bool PerformInteraction(PlayerInteraction interactingScript)
        {
            interactingScript.Inventory.AddFlashlight(_startingBattery);
            return true;
        }
    }
}