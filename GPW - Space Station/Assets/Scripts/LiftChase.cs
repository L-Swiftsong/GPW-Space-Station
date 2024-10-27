using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LiftChase : MonoBehaviour
{
    public Transform spawnPoint;    
    public Transform player;        
    public ChaseDoors[] doors;  
    private bool isChaseStarted = false;

    
    public void StartChase()
    {
        if (!isChaseStarted)
        {
            isChaseStarted = true;

           
            StartCoroutine(CloseDoorsOneByOne());
        }
    }
    private IEnumerator CloseDoorsOneByOne()
    {
        for (int i = 0; i < doors.Length; i++)
        {
           
            while (!PlayerHasPassedDoor(i))
            {
                yield return null;  
            }

            
            doors[i].CloseDoor();
        }
    }
    private bool PlayerHasPassedDoor(int doorIndex)
    {
        
        return player.position.z > doors[doorIndex].door.position.z;
    }
}