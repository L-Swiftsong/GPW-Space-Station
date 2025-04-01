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
        public bool HasFlashlight;
        public float FlashlightBattery;
        public bool FlashlightActiveState;

        // Keycard Decoder.
        public bool HasDecoder;
        public int DecoderSecurityLevel;

        // Medkits.
        public int MedkitCount;


        public void LoadToInventory(PlayerInventory playerInventory)
        {
            playerInventory.SetHasObtainedFlashlight(this.HasFlashlight, this.FlashlightBattery);
            playerInventory.LoadFlashlightActiveState(this.FlashlightActiveState);

            playerInventory.SetHasObtainedKeycardDecoder(this.HasDecoder, this.DecoderSecurityLevel);

            playerInventory.SetMedkits(this.MedkitCount);
        }
        public static InventorySaveData CreateFromInventory(PlayerInventory playerInventory)
        {
            InventorySaveData inventorySaveData = new InventorySaveData();


            // Flashlight.
            inventorySaveData.HasFlashlight = playerInventory.HasFlashlight();
            inventorySaveData.FlashlightBattery = playerInventory.GetFlashlightBattery();
            inventorySaveData.FlashlightActiveState = playerInventory.GetFlashlightActiveState();

            // Keycard Decoder.
            inventorySaveData.HasDecoder = playerInventory.HasKeycardDecoder();
            inventorySaveData.DecoderSecurityLevel = playerInventory.GetDecoderSecurityLevel();

            // Medkit Count.
            inventorySaveData.MedkitCount = playerInventory.GetMedkitCount();


            return inventorySaveData;
        }
    }
}
