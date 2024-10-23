using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SceneManagement;

namespace Teleporters
{
    /// <summary> A teleporter which acts between multiple scenes. These teleporters will only ever affect the player.</summary>
    public class InterSceneTeleporter : BaseTeleporter
    {
        private static bool _allowTeleportation = true; // Set to false when teleporting objects between scenes.
        
        
        [SerializeField] private SceneTransition _sceneTransition;


        protected override void PerformTeleportation()
        {
            // Prevent immediate teleportation when we arrive at our target.
            _allowTeleportation = false;


            SceneLoader.PerformTransition(_sceneTransition);
            StartCoroutine(DebugProgress());
        }
        private IEnumerator DebugProgress() // Instantly stop swhen scene is unloaded.
        {
            float progress = 0.0f; 
            while(progress < 1.0f)
            {
                progress = SceneLoader.GetSceneLoadProgress();
                Debug.Log(progress);
                yield return null;
            }

            Debug.Log("100%");

            yield return new WaitForSeconds(0.5f);

            Debug.Log("Completed");
        }


        private void OnTriggerEnter(Collider other)
        {
            if (!_allowTeleportation)
            {
                return;
            }
            
            Debug.LogWarning("Replace the 'BasicPlayerController' script with whatever our primary player script will be.");
            if (!other.GetComponent<BasicPlayerController>())
            {
                // The exiting collider is not the player.
                return;
            }

            StartTeleportation();
        }
        private void OnTriggerExit(Collider other)
        {
            Debug.LogWarning("Replace the 'BasicPlayerController' script with whatever our primary player script will be.");
            if (!other.GetComponent<BasicPlayerController>())
            {
                // The exiting collider is not the player.
                return;
            }

            StopTeleportation();

            // When the player exits a teleporter, allow further teleportations.
            // This means that when teleported into a scene, they cannot be immediately teleported out, but they can enter and re-exit.
            _allowTeleportation = true;
        }
    }
}