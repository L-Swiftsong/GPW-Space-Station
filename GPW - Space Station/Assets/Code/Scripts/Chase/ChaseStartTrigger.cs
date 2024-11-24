using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Entities.Mimic;
using Entities.Player;

namespace Chase
{
    [RequireComponent(typeof(Collider))]
    public class ChaseStartTrigger : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            if (other.GetComponent<PlayerController>())
            {
                // The player entered the trigger. Start the chase.
                ChaseMimic.StartChase();

                // Destroy ourselves so that the chase can only start once.
                Destroy(this.gameObject);
            }
        }


#if UNITY_EDITOR
        private void OnValidate()
        {
            // Set this object's colliders to be trigger colliders.
            foreach (Collider collider in GetComponentsInChildren<Collider>())
            {
                collider.isTrigger = true;
            }
        }
#endif
    }
}