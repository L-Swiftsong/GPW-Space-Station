using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Inventory
{
    public class InventoryKeycard : InventoryItem
    {
        private int _keycardID;

        public int GetKeycardID() => _keycardID;
        public void SetKeycardID(int keycardID)
        {
            // Set our Keycard ID.
            _keycardID = keycardID;

            // Set the Keycard material.
            Material keycardMaterial = ItemSpawnManager.GetMaterialFromID(keycardID);
            if (keycardMaterial != null)
            {
                GetComponentInChildren<Renderer>().material = keycardMaterial;
            }
        }


        public override string GetItemName() => "Keycard (" + _keycardID + ")";
    }
}