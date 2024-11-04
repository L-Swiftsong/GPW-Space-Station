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

    void Start()
    {
        //Set health to max at start of the game
        health = maxHealth;
    }

    void Update()
    {
        //Updates health UI to accurately reflect current health/damage taken
        UpdateHealthUI();

        //Test damage by pressing Q 
        if (Input.GetKeyDown(KeyCode.R))
        {
            health -= damageTest;
        }

        //Equip Heal
        if (Input.GetKeyDown(KeyCode.H) && !isHealing)
        {
            if (healthPack.activeSelf)
            {
                healthPack.SetActive(false);
            }
            else
            {
                healthPack.SetActive(true);
            }
        }

        //Cant Equip heal if there are no health packs currently held
        if (healthPackAmount <= 0)
        {
            healthPack.SetActive(false);
        }

        //Use Heal when healthpack is equipped and health isn't full
        if (Input.GetKeyDown(KeyCode.Mouse0) && healthPack.activeSelf && health < maxHealth)
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
        if (!isDamageOnCooldown)
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
    }

    //Function to call player heal from external scripts
    public void PlayerHeal()
    {
        health += healAmount;
    }

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

    IEnumerator PlayerHealDelay()
    {
        isHealing = true;
        healthPack.SetActive(false);
        healthPack2.SetActive(true);

        yield return new WaitForSeconds(healDuration);

        isHealing = false;
        healthPack.SetActive(true);
        healthPack2.SetActive(false);

        PlayerHeal();
        healthPackAmount--;
    }
}
