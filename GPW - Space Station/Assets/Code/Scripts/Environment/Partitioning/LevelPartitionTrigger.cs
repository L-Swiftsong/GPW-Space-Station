﻿using UnityEngine;

namespace Environment.Partitioning
{
    public class LevelPartitionTrigger : MonoBehaviour
    {
        [SerializeField] private LevelSection _associatedSection;
        [SerializeField] private LayerMask _playerLayers = 1 << 3;
        private int _selfPlayerCounts = 0;


        private void OnDestroy()
        {
            // Ensure that we don't accidentally cause the level section to persist if we end up getting destroyed (E.g. By a level reload).
            if (_selfPlayerCounts > 0)
            {
                LevelPartitionManager.SubtractFromEnabledCount(_associatedSection);
            }
        }


        private void OnTriggerEnter(Collider other)
        {
            if (!_playerLayers.Contains(other.gameObject.layer))
            {
                return;
            }
            ++_selfPlayerCounts;
            LevelPartitionManager.AddToEnabledCount(_associatedSection);
        }
        private void OnTriggerExit(Collider other)
        {
            if (!_playerLayers.Contains(other.gameObject.layer))
            {
                return;
            }
            --_selfPlayerCounts;
            LevelPartitionManager.SubtractFromEnabledCount(_associatedSection);
        }


#if UNITY_EDITOR

        private void OnValidate()
        {
            foreach(var collider in this.GetComponents<Collider>())
            {
                collider.isTrigger = true;
            }
        }


        [ContextMenu("Display Locks for Type")]
        private void DisplayLocksForType()
        {
            Debug.Log(LevelPartitionManager.GetCountForSectionType(_associatedSection));
        }
#endif
    }
}