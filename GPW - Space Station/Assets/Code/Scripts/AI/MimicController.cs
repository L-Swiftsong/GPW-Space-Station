using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MimicController : MonoBehaviour
{
    [SerializeField]
    private float moveSpeed = 3f;       

    private Transform playerTransform;  
    private void Start()
    {
        
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
        {
            playerTransform = player.transform;
        }
        else
        {
            Debug.LogError("Player not found! Please tag the player GameObject with 'Player'.");
        }
    }

    private void Update()
    {
        if (playerTransform != null)
        {
            if (HasLineOfSightToPlayer())
            {
                
                Vector3 direction = new Vector3(
                    playerTransform.position.x - transform.position.x,
                    0f, 
                    playerTransform.position.z - transform.position.z
                );

                if (direction.sqrMagnitude > 0.01f) 
                {
                    direction = direction.normalized;

                    
                    transform.position += direction * moveSpeed * Time.deltaTime;

                    
                    Quaternion lookRotation = Quaternion.LookRotation(direction);
                    transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
                }
            }
        }
    }

    private bool HasLineOfSightToPlayer()
    {
        RaycastHit hit;
        Vector3 directionToPlayer = playerTransform.position - transform.position;
        float distanceToPlayer = directionToPlayer.magnitude;

        if (Physics.Raycast(transform.position, directionToPlayer.normalized, out hit, distanceToPlayer))
        {
            if (hit.transform == playerTransform)
            { 
                return true;
            }
            else
            {
                
                return false;
            }
        }
        else
        {
       
            return true;
        }
    }
}
