using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Inventory;

public class HealthPack : MonoBehaviour, IInteractable
{
    [SerializeField] private InventoryHealthKit _inventoryHealthKitPrefab;


    public void Interact(PlayerInteraction playerInteraction)
    {
        playerInteraction.Inventory.AddItem(_inventoryHealthKitPrefab);
        Destroy(this.gameObject);
    }
}
