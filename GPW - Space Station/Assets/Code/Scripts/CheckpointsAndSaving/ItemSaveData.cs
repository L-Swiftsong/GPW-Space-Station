using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Inventory.Data;
using Inventory.Items;

[System.Serializable]
public struct ItemSaveData
{
    public InventoryItemDataSO ItemData;


    /* Meaning:
        Keycards: ID
        Flashlight: Remaining Battery
        Health Kits: Healing Amount, Healing delay
        Flares: Remaining Lifetime
     */
    public float[] ItemValues;


    private ItemSaveData(InventoryItemDataSO itemData, float[] itemValue)
    {
        this.ItemData = itemData;
        this.ItemValues = itemValue;
    }


    public static ItemSaveData Empty => new ItemSaveData(null, null);

    public static ItemSaveData CreateFromItemInstance(InventoryItem inventoryItem)
    {
        if (inventoryItem == null)
        {
            return Empty;
        }
        
        System.Type itemType = inventoryItem.GetType();
        if (itemType == typeof(InventoryKeycard))
        {
            return DataFromKeycard(inventoryItem as InventoryKeycard);
        }
        else if (itemType == typeof(FlashlightController))
        {
            return DataFromFlashlight(inventoryItem as FlashlightController);
        }
        else if (itemType == typeof(InventoryHealthKit))
        {
            return DataFromHealthKit(inventoryItem as InventoryHealthKit);
        }
        else
        {
            Debug.LogError("Error: Saving unset InventoryItem type");
            return Empty;
        }
    }
    public static ItemSaveData DataFromKeycard(InventoryKeycard keycard) => new ItemSaveData(keycard.GetItemData(), new float[1] { keycard.GetKeycardID() });
    public static ItemSaveData DataFromFlashlight(FlashlightController flashlight) => new ItemSaveData(flashlight.GetItemData(), new float[1] { flashlight.GetCurrentBattery() });
    public static ItemSaveData DataFromHealthKit(InventoryHealthKit healthKit) => new ItemSaveData(healthKit.GetItemData(), healthKit.GetItemValues());
    //public static ItemSaveData DataFromFlare(InventoryFlare flare) => new ItemSaveData(flare.GetItemData(), Mathf.FloorToInt(flare.RemainingLifetime));
}
