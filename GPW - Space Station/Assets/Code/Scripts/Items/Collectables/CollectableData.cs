using UnityEngine;

namespace Items.Collectables
{
    public abstract class CollectableData : ScriptableObject
    {
        [Header("Collectable Data")]
        [SerializeField] private string _collectableName;

        public string CollectableName => _collectableName;
    }
}
