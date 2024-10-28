using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MimicController : MonoBehaviour
{
    [SerializeField]
    private float moveSpeed = 3f;

    private void Update()
    {
        if (PlayerManager.Instance.Player != null)
        {
            if (HasLineOfSightToPlayer())
            {
                
                Vector3 direction = new Vector3(
                    PlayerManager.Instance.Player.position.x - transform.position.x,
                    0f,
                    PlayerManager.Instance.Player.position.z - transform.position.z
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
        Vector3 directionToPlayer = PlayerManager.Instance.Player.position - transform.position;
        float distanceToPlayer = directionToPlayer.magnitude;

        if (Physics.Raycast(transform.position, directionToPlayer.normalized, out hit, distanceToPlayer))
        {
            if (hit.transform == PlayerManager.Instance.Player)
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
