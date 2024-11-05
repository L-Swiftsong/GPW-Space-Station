using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorTriggerController : MonoBehaviour
{
    [Header("Doors to Move")]
    public Transform[] doors;

    [Header("Target Positions")]
    public Transform[] targetPositions;

    [Header("Movement Settings")]
    public float moveSpeed = 2f;
    public bool useSmoothMovement = true;

    private bool isTriggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (!isTriggered && other.CompareTag("Player"))
        {
            isTriggered = true;
            Debug.Log("DOOR CLOSING");

            // begin moving the doors
            for (int i = 0; i < doors.Length; i++)
            {
                StartCoroutine(MoveDoor(doors[i], targetPositions[i]));
            }
        }
    }

    private IEnumerator MoveDoor(Transform door, Transform targetPosition)
    {
        if (useSmoothMovement)
        {
            // Move the door over time
            while (Vector3.Distance(door.position, targetPosition.position) > 0.01f)
            {
                door.position = Vector3.MoveTowards(door.position, targetPosition.position, moveSpeed * Time.deltaTime);
                yield return null;
            }
            door.position = targetPosition.position;
        }
        else
        {
            // Moves the door instantly [maybe not need this though]
            door.position = targetPosition.position;
        }
        Debug.Log(door.name + " DOOR HAS CLOSED.");
    }
}
