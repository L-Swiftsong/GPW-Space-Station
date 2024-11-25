using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillChaser : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Mimic")
        {
            Destroy(other.gameObject);
        }
    }
}
