using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Inventory
{
    public class InventoryHealthKit : InventoryItem
    {
        private PlayerInventory _playerInventory;
        private PlayerHealth _playerHealth;

        [SerializeField] private float _healingAmount = 25.0f;
        [SerializeField] private float _healingDuration = 3.0f;

        public override void Initialise(PlayerInventory playerInventory)
        {
            base.Initialise(playerInventory);

            _playerInventory = playerInventory;
            _playerHealth = playerInventory.PlayerHealth;
        }

        public override void Equip()
        {
            base.Equip();
            _playerHealth.OnUsedHealthKit += PlayerHealth_OnUsedHealthKit;
        }
        public override void Unequip()
        {
            base.Unequip();
            _playerHealth.OnUsedHealthKit -= PlayerHealth_OnUsedHealthKit;
        }


        // Remove this health kit instance when the player heals using a Health Kit (We will only be subscribed when this is the equipped item).
        private void PlayerHealth_OnUsedHealthKit() => _playerInventory.RemoveInventoryItem(this);
        


        public override void StartUse() => _playerHealth.StartHealing(_healingAmount, _healingDuration);
        public override void StopUse() => _playerHealth.CancelHealing();

        public override string GetItemName() => "Medkit";
    }
}