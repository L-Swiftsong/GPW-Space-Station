using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Inventory.Data;

namespace Inventory.Items
{
    public class InventoryHealthKit : InventoryItem
    {
        private PlayerInventory _playerInventory;
        private PlayerHealth _playerHealth;

        private float _healingAmount = 25.0f;
        private float _healingDelay = 3.0f;

        public override void Initialise(PlayerInventory playerInventory, InventoryItemDataSO itemData, float[] itemValues)
        {
            base.Initialise(playerInventory, itemData, itemValues);

            _playerInventory = playerInventory;
            _playerHealth = playerInventory.PlayerHealth;

            this._healingAmount = itemValues[0];
            this._healingDelay = itemValues[1];
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
        


        public override void StartUse() => _playerHealth.StartHealing(_healingAmount, _healingDelay);
        public override void StopUse() => _playerHealth.CancelHealing();

        public override string GetItemName() => "Medkit";
        public float[] GetItemValues() => new float[2] { _healingAmount, _healingDelay };
    }
}