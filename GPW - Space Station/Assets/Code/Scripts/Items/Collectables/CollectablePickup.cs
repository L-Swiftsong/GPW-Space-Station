using Audio;
using Interaction;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Items.Collectables
{
    public class CollectablePickup : ItemPickup
    {
        [SerializeField] private CollectableData _collectableData;
        [SerializeField] private AudioClip pickupSound; // Assign in Inspector
        [SerializeField] private AudioSource audioSource;


        protected override bool PerformInteraction(PlayerInteraction interactingScript)
        {

            SFXManager.Instance.PlayClipAtPosition(pickupSound, transform.position, 1, 1, 1f);
            CollectableManager.AddCollectable(_collectableData);



            Debug.Log($"Obtained Collectable of Type: {_collectableData.GetType()}");

            Debug.Log($"Current Count (CollectableData): {CollectableManager.GetCollectablesOfType<CollectableData>().Count}");
            Debug.Log($"Current Count (CodexData): {CollectableManager.GetCollectablesOfType<CodexData>().Count}");
            return true;
        }
    }
}
