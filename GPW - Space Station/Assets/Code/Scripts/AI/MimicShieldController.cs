using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MimicShieldController : MonoBehaviour
{
    [SerializeField]
    private float mimicShield = 100f;       // Shield for the mimic

    [SerializeField]
    private float damagePerSecond = 10f;    // Amount of damage to take per second

    private bool isBeingDamaged = false;
    private bool isStunned = false;

    // Reference to MimicController
    private MimicController mimicController;

    private void Awake()
    {
        // Get reference to MimicController component
        mimicController = GetComponent<MimicController>();
    }

    private void Update()
    {
        if (isBeingDamaged && !isStunned)
        {
            TakeDamage(damagePerSecond * Time.deltaTime);
        }
    }

    // Method to reduce shield
    public void TakeDamage(float damageAmount)
    {
        mimicShield -= damageAmount;

        if (mimicShield <= 0f && !isStunned)
        {
            StartCoroutine(StunMimic());
        }
    }

    // Stuns the mimic when shield is depleted
    private IEnumerator StunMimic()
    {
        isStunned = true;
        mimicShield = 0f;
        isBeingDamaged = false; // Stop applying damage during stun

        Debug.Log("Mimic stunned!");

        // Disable mimic actions
        if (mimicController != null)
        {
            mimicController.enabled = false; // Disable the MimicController script
        }

        // Stun time
        float stunDuration = Random.Range(3f, 5f);
        yield return new WaitForSeconds(stunDuration);

        // Re-enable mimic actions
        if (mimicController != null)
        {
            mimicController.enabled = true; // Enable the MimicController script
        }

        isStunned = false;
        mimicShield = 100f; // Reset shield after stun

        Debug.Log("Mimic recovered from stun!");

        // Optionally, start applying damage again if conditions are met
        // isBeingDamaged = true; // Uncomment if you want damage to resume immediately
    }

    // Start applying damage
    public void StartTakingDamage()
    {
        if (!isStunned) // Only start taking damage if not stunned
        {
            isBeingDamaged = true;
        }
    }

    // Stop applying damage
    public void StopTakingDamage()
    {
        isBeingDamaged = false;
    }
}
