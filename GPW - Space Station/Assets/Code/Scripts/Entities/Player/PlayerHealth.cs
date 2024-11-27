using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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

    public bool isHealing = false;
    private Coroutine _healingCoroutine;


    public event System.Action OnUsedHealthKit;


    void Start()
    {
        //Set health to max at start of the game
        health = maxHealth;
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

    // (Temp Implementation) Show the game over UI once the player dies.
    void Die() => UI.GameOver.GameOverUI.Instance.ShowGameOverUI();

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


    /// <summary> Start healing the player from a Medkit.</summary>
    public void StartHealing(float healingAmount, float healingDelay)
    {
        if (_healingCoroutine != null)
        {
            StopCoroutine(_healingCoroutine);
        }

        _healingCoroutine = StartCoroutine(HealPlayerAfterDelay(healingAmount, healingDelay));
    }
    /// <summary> Stop the current healing process.</summary>
    public void CancelHealing()
    {
        if (_healingCoroutine != null)
        {
            StopCoroutine(_healingCoroutine);
        }

        isHealing = false;
    }

    // Healing process functionality.
    IEnumerator HealPlayerAfterDelay(float healingAmount, float healingDelay)
    {
        // Start healing.
        isHealing = true;

        // Wait a few seconds before player is healed and health pack disappears.
        yield return new WaitForSeconds(healingDelay);

        // Finish healing.
        isHealing = false;
        health += healingAmount;
        OnUsedHealthKit?.Invoke();
    }
}
