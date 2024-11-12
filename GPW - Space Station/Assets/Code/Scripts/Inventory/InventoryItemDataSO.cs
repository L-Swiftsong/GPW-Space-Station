using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Inventory.Data
{
    [CreateAssetMenu(menuName = "Inventory/New Inventory Item Data", fileName = "NewInventoryItemData")]
    public class InventoryItemDataSO : ScriptableObject
    {
        public InventoryItem ItemPrefab;
    }
}