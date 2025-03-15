using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleTrigger : MonoBehaviour
{
    public SimonSays simonSays; // Drag your SimonSays component here in the Inspector

    private bool hasTriggered = false;

    private void OnTriggerEnter(Collider other)
    {
        // Check if the object entering the trigger is the Player
        // and ensure we haven’t already triggered once.
        if (!hasTriggered && other.CompareTag("Player"))
        {
            hasTriggered = true;

            // Start the puzzle
            simonSays.StartPuzzle();
            Debug.Log("SimonSays puzzle started via trigger!");
        }
    }
}

