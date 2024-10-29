using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPack : MonoBehaviour
{
    //Reference to PlayerHealth script
    private PlayerHealth playerHealth;

    //Upon colliding with player call the PlayerHeal function from PlayerHealth script
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            other.GetComponent<PlayerHealth>().PickUpHeal();
            Destroy(gameObject);
        }
    }
}
