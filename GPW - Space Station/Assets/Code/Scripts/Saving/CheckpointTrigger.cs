using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Saving
{
    [RequireComponent(typeof(Collider))]
    public class CheckpointTrigger : MonoBehaviour
    {
        [SerializeField] private LayerMask _playerLayers;
        [SerializeField] private bool _onlyTriggerOnce = true;

        #if UNITY_EDITOR
        private void OnValidate() => GetComponent<Collider>().isTrigger = true;
        #endif

        private void OnTriggerEnter(Collider other)
        {
            if (!_playerLayers.Contains(other.gameObject.layer))
            {
                // Not the player.
                return;
            }

            Debug.Log("Checkpoint Save");

            //SaveManager.SaveCheckpoint();

            if (_onlyTriggerOnce)
            {
                Destroy(this.gameObject);
            }
            throw new System.NotImplementedException();
        }
    }
}