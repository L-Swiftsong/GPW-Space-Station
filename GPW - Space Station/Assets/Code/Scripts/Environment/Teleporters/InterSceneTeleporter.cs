using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Entities.Player;
using SceneManagement;

namespace Teleporters
{
    /// <summary> A teleporter which acts between multiple scenes. These teleporters will only ever affect the player.</summary>
    public class InterSceneTeleporter : BaseTeleporter
    {
        [SerializeField] private SceneTransition _sceneTransition;


        protected override void PerformTeleportation()
        {
            // Load the target scene using our given transition.
            SceneLoader.Instance.PerformTransition(_sceneTransition);
        }


        private void OnTriggerEnter(Collider other)
        {
            if (!other.GetComponent<PlayerController>())
            {
                // The exiting collider is not the player.
                return;
            }

            StartTeleportation();
        }
        private void OnTriggerExit(Collider other)
        {
            if (!other.GetComponent<PlayerController>())
            {
                // The exiting collider is not the player.
                return;
            }

            StopTeleportation();
        }
    }
}