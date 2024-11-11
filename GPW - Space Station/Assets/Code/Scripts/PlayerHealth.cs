using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    public float maxHealth = 100f;
    public float health;

    public int damageTest;

    public float damageCooldown = 0.25f;
    private bool isDamageOnCooldown = false;

    public GameObject brokenVisor1;
    public GameObject brokenVisor2;
    public GameObject brokenVisor3;

    public GameObject healthPack;
    public GameObject healthPack2;

    public int healAmount = 25;
    public int healthPackAmount;

    public float healDuration = 3f;
    public bool isHealing = false;

    private PlayerInventory playerInventory;

    void Start()
    {
        //Set health to max at start of the game
        health = maxHealth;

        //References
        playerInventory = FindObjectOfType<PlayerInventory>();
    }

    void Update()
    {
        //Updates health UI to accurately reflect current health/damage taken
        UpdateHealthUI();

        //Test damage by pressing R 
        if (Input.GetKeyDown(KeyCode.R))
        {
            health -= damageTest;
        }

        //Cant Equip heal if there are no health packs currently held
        if (healthPackAmount <= 0)
        {
            healthPack.SetActive(false);
        }
        
        //Use Heal by left clicking when healthpack is equipped, health isn't full and inventory menu isn't open
        if (Input.GetKeyDown(KeyCode.Mouse0) && healthPack.activeSelf && health < maxHealth && !playerInventory.inventoryMenuOpen)
        {
            StartCoroutine(PlayerHealDelay());
        }

        //Ensure player doesnt overheal
        if (health > maxHealth)
        {
            health = maxHealth;
        }

        //Once health is depleted restart level
        else if (health <= 0)
        {
            Die();
        }
    }

    //Function to call player damage from external scripts
    public void PlayerTakeDamage(float damage)
    {
        if (!isDamageOnCooldown) //Only take damage if enemies attack cooldown is finished
        {
            health -= damage;
            health = Mathf.Max(health, 0f);

            isDamageOnCooldown = true;
            StartCoroutine(StartDamageCooldown());
        }
    }

    //Counter to track number of heals held
    public void PickUpHeal()
    {
        healthPackAmount++;
        playerInventory.PickupHealthPack(); //Assigns health pack to inventory slot
    }

    //Equip heal called from Player Inventory script when an inventory slot with a health pack is pressed
    public void EquipHeal()
    {
        if (!healthPack2.activeSelf) //Can't equip another health pack while healing
        {
            healthPack.SetActive(true);
        } 
    }

    //Function to call player heal from external scripts
    public void PlayerHeal()
    {
        health += healAmount;
    }

    //Reload Scene when player dies
    void Die()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    void UpdateHealthUI()
    {
        //changes visor state depending on current health
        if (health == 100)
        {
            brokenVisor1.SetActive(false);
            brokenVisor2.SetActive(false);
            brokenVisor3.SetActive(false);
        }
        else if (health == 75)
        {
            brokenVisor1.SetActive(true);
            brokenVisor2.SetActive(false);
            brokenVisor3.SetActive(false);
        }
        else if (health == 50)
        {
            brokenVisor1.SetActive(false);
            brokenVisor2.SetActive(true);
            brokenVisor3.SetActive(false);
        }
        else if (health == 25)
        {
            brokenVisor1.SetActive(false);
            brokenVisor2.SetActive(false);
            brokenVisor3.SetActive(true);
        }
    }

    IEnumerator StartDamageCooldown()
    {
        //Timer for when the enemy can damage the player again
        yield return new WaitForSeconds(damageCooldown);
        isDamageOnCooldown = false;
    }

    //Healing process functionality
    IEnumerator PlayerHealDelay()
    {
        isHealing = true;
        healthPack.SetActive(false); //change active health pack game object to show player is healing
        healthPack2.SetActive(true);

        yield return new WaitForSeconds(healDuration); //Wait a few seconds before player is healed and health pack disappears

        isHealing = false;
        healthPack2.SetActive(false);

        PlayerHeal();
        healthPackAmount--;
        playerInventory.RemoveHealthPack(); //Remove health pack from inventory slot
    }
}
