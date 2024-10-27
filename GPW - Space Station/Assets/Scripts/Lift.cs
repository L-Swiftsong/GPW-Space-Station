using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lift : MonoBehaviour
{ 
    public Transform liftPlatform; 
    public float moveDistance = 10f; 
    public float speed = 2f; 

    private Vector3 initialPosition; 
    private Vector3 targetPosition; 
    private bool isActivated = false; 

    private void Start()
    {
        
        initialPosition = liftPlatform.position;
        
        targetPosition = new Vector3(initialPosition.x, initialPosition.y - moveDistance, initialPosition.z);
    }

    private void OnTriggerEnter(Collider other)
    {
        
        if (other.CompareTag("Player"))
        {
            
            isActivated = true;
        }
    }

    private void Update()
    {
        
        if (isActivated)
        {
           
            liftPlatform.position = Vector3.MoveTowards(liftPlatform.position, targetPosition, speed * Time.deltaTime);

            
            if (liftPlatform.position == targetPosition)
            {
                isActivated = false; 
            }
        }
    }

   
    public void ResetLift()
    {
        liftPlatform.position = initialPosition;
        isActivated = false;
    }
}
