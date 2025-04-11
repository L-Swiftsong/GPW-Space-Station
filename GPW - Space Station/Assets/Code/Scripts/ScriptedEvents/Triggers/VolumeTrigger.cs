using Entities.Mimic;
using Entities.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ScriptedEvents.Triggers
{
    [RequireComponent(typeof(Collider))]
    public class VolumeTrigger : ScriptedEventTrigger
    {
        [System.Serializable] [System.Flags]
        private enum TriggerTypes { None = 0, Player = 1 << 0, Mimic = 1 << 1, Object = 1 << 2 }

        [Header("Trigger Volume Settings")]
        [SerializeField] private TriggerTypes _triggerTypes = TriggerTypes.None;

        protected virtual void OnTriggerEnter(Collider other)
        {
            Debug.Log(other.name);
            if (IsValidCollider(other))
            {
                // The collider is valid.
                // Activate our trigger.
                ActivateTrigger();
            }
        }


        protected bool IsValidCollider(Collider collider)
        {
            if (_triggerTypes.HasFlag(TriggerTypes.Player))
            {
                // We are wanting to test if the collider was a Player.

                if (collider.GetComponentInParent<PlayerController>())
                {
                    // The collider was a player.
                    return true;
                }
            }

            if (_triggerTypes.HasFlag(TriggerTypes.Mimic))
            {
                // We are wanting to test if the collider was a Mimic.

                if (collider.GetComponent<GeneralMimic>())
                {
                    return true;
                }
                if (collider.GetComponent<ChaseMimic>())
                {
                    return true;
                }
                if (collider.GetComponent<MimicController>())
                {
                    return true;
                }
            }

            if (_triggerTypes.HasFlag(TriggerTypes.Object))
            {
                // We are wanting to test if the collider was an object.
                // For now, this means that the collider has a Rigidbody component that isn't Kinematic.

                if (collider.TryGetComponent<Rigidbody>(out Rigidbody rb))
                {
                    if (!rb.isKinematic)
                    {
                        // This object has a non-kinematic rigidbody attached.
                        return true;
                    }
                }
            }

            return false;
        }



#if UNITY_EDITOR
        private void OnValidate()
        {
            foreach(Collider collider in this.GetComponents<Collider>())
            {
                collider.isTrigger = true;
            }

            if (_triggerTypes == TriggerTypes.None)
            {
                Debug.LogWarning("Warning: No values are set for the '_triggerTypes' of this object, and so it will never trigger its event.", this);
            }
        }
#endif
    }
}