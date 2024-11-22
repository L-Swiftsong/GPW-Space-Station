using UnityEngine;

namespace Items.Flashlight
{
    /// <summary> A collectable that gives the player a flashlight.</summary>
    public class FlashlightPickup : MonoBehaviour, IInteractable
    {
        [SerializeField] private float _startingBattery = 100.0f;

        public void Interact(PlayerInteraction playerInteraction)
        {
            playerInteraction.Inventory.AddFlashlight(_startingBattery);
            Destroy(gameObject);
        }
    }
}