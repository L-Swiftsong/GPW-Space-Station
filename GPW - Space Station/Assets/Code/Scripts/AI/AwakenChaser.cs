using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Chase
{
    public class AwakenChaser : MonoBehaviour
    {
        private void OnDestroy()
        {
            ActivateChasers();
        }

        private void OnDisable()
        {
            ActivateChasers();
        }

        private void ActivateChasers()
        {
            // Find all EnemyChaser instances and activate them
            EnemyChaser[] enemyChasers = FindObjectsOfType<EnemyChaser>();
            foreach (EnemyChaser chaser in enemyChasers)
            {
                chaser.ActivateChase();
            }

            Debug.Log("Chase activated by KeycardChaseActivator!");
        }
    }
}