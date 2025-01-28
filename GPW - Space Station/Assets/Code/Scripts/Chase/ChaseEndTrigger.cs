using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Entities.Mimic;
using ScriptedEvents.Triggers;


namespace Chase
{
    public class ChaseEndTrigger : VolumeTrigger
    {
        protected override void OnTriggerEnter(Collider other)
        {
            if (IsValidCollider(other))
            {
                // The collider is valid.
                // Activate our trigger.
                ActivateTrigger();

                // Stop the active chase.
                ChaseMimic.EndChase();
            }
        }
    }
}