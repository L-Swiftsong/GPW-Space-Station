using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Items;

namespace Saving
{
    [System.Serializable]
    public class InventorySaveData
    {
        // Flashlight.
        public bool FlashlightObtained;
        public float FlashlightBattery;

        // Keycard Decoder.
        public bool DecoderObtained;
        public int DecoderSecurityLevel;

        // Medkits.
        public int MedkitCount;

        // Key Items.
        public bool[] KeyItemsObtained;

        // Collectables.
        public CollectableSaveData[] CollectablesSaveData;



        public static InventorySaveData Default = new InventorySaveData()
        {
            FlashlightObtained = false,
            FlashlightBattery = 0.0f,

            DecoderObtained = false,
            DecoderSecurityLevel = 0,

            MedkitCount = 0,

            KeyItemsObtained = new bool[0],//[KeyItemManager.KeyItemCount],

            CollectablesSaveData = new CollectableSaveData[0],
        };


        public static InventorySaveData FromInventoryData(PlayerInventory playerInventory) => new InventorySaveData()
        {
            FlashlightObtained = playerInventory.HasFlashlight(),
            FlashlightBattery = playerInventory.GetFlashlightBattery(),

            DecoderObtained = playerInventory.HasKeycardDecoder(),
            DecoderSecurityLevel = playerInventory.GetDecoderSecurityLevel(),

            MedkitCount = playerInventory.GetMedkitCount(),

            KeyItemsObtained = new bool[0],

            CollectablesSaveData = CollectableSaveData.GetCurrentSaveData(),
        };
    }
}
