using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
public class EnemyHealthController : MonoBehaviour
{
    public float enemyHealth = 100f;  // Starting health for the enemy
    public float damagePerSecond = 10f;  // Amount of damage to take per second

    private bool isBeingDamaged = false;

    private void Update()
    {
        if (isBeingDamaged)
        {
            TakeDamage(damagePerSecond * Time.deltaTime);
        }
    }

    // Method to reduce health
    public void TakeDamage(float damageAmount)
    {
        enemyHealth -= damageAmount;

        if (enemyHealth <= 0f)
        {
            DestroyEnemy();
        }
    }

    // Destroys the enemy when health is depleted
    private void DestroyEnemy()
    {
        // Add any death animation or sound effects here
        Destroy(gameObject);
        Debug.Log("Enemy destroyed!");
    }

    // Start applying damage
    public void StartTakingDamage()
    {
        isBeingDamaged = true;
    }

    // Stop applying damage
    public void StopTakingDamage()
    {
        isBeingDamaged = false;
    }
}
*/