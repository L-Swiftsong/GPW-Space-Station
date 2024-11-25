using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Entities.Mimic;

namespace Chase
{
    public class AwakenChaser : MonoBehaviour
    {
        private void OnDestroy() => ActivateChasers();
        private void OnDisable() => ActivateChasers();
        

        private void ActivateChasers()
        {
            ChaseMimic.StartChase();
            Debug.Log($"Chase activated by {this.gameObject.name}!");
        }
    }
}