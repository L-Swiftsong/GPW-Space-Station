using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPack : MonoBehaviour, IInteractable
{
    //Reference to PlayerHealth script
    private PlayerHealth playerHealth;

    private void Start()
    {
        playerHealth = FindObjectOfType<PlayerHealth>();
    }

    public void Interact(PlayerInteraction playerInteraction)
    {
        if (playerHealth != null)
        {
            playerHealth.PickUpHeal();
            Destroy(gameObject);
        }
    }
}
