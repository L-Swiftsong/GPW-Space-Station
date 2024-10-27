using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaseDoors : MonoBehaviour
{

    public float closingSpeed = 2f;  
    public Transform door;          
    public Transform closedPositionTarget;  
    private bool isClosing = false;  

    // Method to start closing the door
    public void CloseDoor()
    {
        if (!isClosing)
        {
            isClosing = true;
            StartCoroutine(CloseDoorRoutine());
        }
    }

 
    private IEnumerator CloseDoorRoutine()
    {
        while (Vector3.Distance(door.position, closedPositionTarget.position) > 0.01f)
        {
            door.position = Vector3.MoveTowards(door.position, closedPositionTarget.position, Time.deltaTime * closingSpeed);
            yield return null;
        }

        door.position = closedPositionTarget.position;
        isClosing = false;
    }
}
