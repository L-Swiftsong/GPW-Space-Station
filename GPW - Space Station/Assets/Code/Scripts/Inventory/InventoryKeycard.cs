using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Inventory.Data;

namespace Inventory.Items
{
    public class InventoryKeycard : InventoryItem
    {
        private int _keycardID;

        public override void Initialise(PlayerInventory inventory, InventoryItemDataSO itemData, float[] itemValues)
        {
            base.Initialise(inventory, itemData, itemValues);

            _keycardID = Mathf.RoundToInt(itemValues[0]);
        }


        public int GetKeycardID() => _keycardID;


        public override string GetItemName() => "Keycard (" + _keycardID + ")";
    }
}