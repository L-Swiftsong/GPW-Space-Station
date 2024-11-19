using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPack : MonoBehaviour, IInteractable
{
    [SerializeField] private float _healingAmount = 25.0f;
    [SerializeField] private float _healingDelay = 3.0f;


    public void Interact(PlayerInteraction playerInteraction)
    {
        //playerInteraction.Inventory.AddItem(_healthKitInventoryData, new float[2] { _healingAmount, _healingDelay });
        Destroy(this.gameObject);
        throw new System.NotImplementedException();
    }
}
