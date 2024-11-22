using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Saving;

public class TempPlayerDeath : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            RestartScene();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            RestartScene();
        }
    }


    private void RestartScene()
    {
        SaveManager.ReloadCheckpointSave();
    }
}
