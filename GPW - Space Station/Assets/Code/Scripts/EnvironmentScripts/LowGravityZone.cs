/* CONTEXT
 * This script handles the behavior of a low gravity zone. It applies low gravity effects
 * to objects with the "Gravity" tag and to the player when they enter the trigger zone.
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LowGravityZone : MonoBehaviour
{
    [Header("Low Gravity Settings")]
    public float lowGravityScale = 0.2f; 

    [Header("Drift Settings")]
    public float driftForceMagnitude = 0.5f;  
    public float driftForceInterval = 1f;    

    [Header("Tags")]

    [SerializeField]
    private string gravityTag = "Gravity"; 
    [SerializeField]
    private string playerTag = "Player";   

    private CharacterController playerController;
    private bool isPlayerInLowGravity = false;
    private Vector3 playerVelocity;

    private void Start()
    {
        // Find the player's character controller.
        playerController = PlayerManager.Instance.Player.GetComponent<CharacterController>();

        // Start coroutine to apply drift forces at intervals
        InvokeRepeating(nameof(ApplyDriftToObjects), 0f, driftForceInterval);
    }

    // handles onjects with gravity tag and applies low gravity to them 
    private void OnTriggerEnter(Collider other)
    {
       
        if (other.CompareTag(gravityTag))
        {
         
            Rigidbody rb = other.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.useGravity = false;  
            }
        }
    // player 
        else if (other.CompareTag(playerTag))
        {
            // Gets the PlayerController script and sets the low gravity
            PlayerController playerController = other.GetComponent<PlayerController>();
            if (playerController != null)
            {
                playerController.EnterLowGravityZone();  // Enable low gravity for the player
            }
        }
    }

    // when player exits / onbjects go back to normal gravity 
    private void OnTriggerExit(Collider other)
    {
        
        if (other.CompareTag(gravityTag))
        {
            
            Rigidbody rb = other.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.useGravity = true;
            }
        }
        
        else if (other.CompareTag(playerTag))
        {
            
            PlayerController playerController = other.GetComponent<PlayerController>();
            if (playerController != null)
            {
                playerController.ExitLowGravityZone(); 
            }
        }
    }

    // This persists low gravity for the player even when not in the zone
    private void Update()
    {
        if (isPlayerInLowGravity && playerController != null)
        {
            ApplyLowGravityToPlayer();
        }
    }

    // Apply low gravity force to the player
    private void ApplyLowGravityToPlayer()
    {
        if (playerController.isGrounded)
        {
            
            if (playerVelocity.y < 0)
            {
                playerVelocity.y = -0.5f;  
            }
        }
        else
        {
            // Apply low gravity effect while in the air
            playerVelocity.y += Physics.gravity.y * lowGravityScale * Time.deltaTime;
        }

        
        playerController.Move(playerVelocity * Time.deltaTime);
    }

    // applies random drifting forces to objects in the zone making more realistic  
    private void ApplyDriftToObjects()
    {
       
        foreach (Rigidbody rb in FindObjectsOfType<Rigidbody>())
        {
            if (rb.CompareTag(gravityTag))
            {
                Vector3 randomDrift = new Vector3(
                    Random.Range(-1f, 1f),
                    Random.Range(-1f, 1f),
                    Random.Range(-1f, 1f)
                ).normalized * driftForceMagnitude;

                rb.AddForce(randomDrift, ForceMode.Acceleration);
            }
        }
    }

    // reset gravity back to normal for the player
    public void ResetPlayerGravity()
    {
        isPlayerInLowGravity = false;
    }
}
