using UnityEngine;

namespace Environment.Partitioning
{
    public class LevelPartitionTrigger : MonoBehaviour
    {
        [SerializeField] private LevelSection _associatedSection;
        [SerializeField] private LayerMask _playerLayers = 1 << 3;
        private int _selfPlayerCounts = 0;


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
#endif
    }
}