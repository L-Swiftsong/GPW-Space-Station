using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Interaction;

namespace Items
{
    public class BatteryPickup : ItemPickup
    {
        [SerializeField] private float _rechargeAmount = 50f;

        protected override bool PerformInteraction(PlayerInteraction interactingScript)
        {
            var flashlight = interactingScript.GetComponentInChildren<Flashlight.FlashlightController>();

            if (flashlight != null)
            {
                flashlight.AddBattery(_rechargeAmount);
                return true;
            }

            return false;
        }
    }
}
