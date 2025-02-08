using Interaction;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Items.Collectables
{
    public class CollectablePickup : ItemPickup
    {
        [SerializeField] private CollectableData _collectableData;

        protected override bool PerformInteraction(PlayerInteraction interactingScript)
        {
            CollectableManager.AddCollectable(_collectableData);

            Debug.Log($"Obtained Collectable of Type: {_collectableData.GetType()}");

            Debug.Log($"Current Count (CollectableData): {CollectableManager.GetCollectablesOfType<CollectableData>().Count}");
            Debug.Log($"Current Count (CodexData): {CollectableManager.GetCollectablesOfType<CodexData>().Count}");
            return true;
        }
    }
}
