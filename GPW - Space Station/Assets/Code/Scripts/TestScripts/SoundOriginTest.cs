using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundOriginTest : MonoBehaviour
{
    [SerializeField] private bool _performTest;

    [Header("Test Settings")]
    [SerializeField] private float _soundRadius;


    private void Update()
    {
        if (_performTest)
        {
            // Only perform the test once.
            _performTest = false;

            // Notify all active EntitySenses Instances that a sound has been triggered, prompting them to try and detect it.
            AI.EntitySenses.SoundTriggered(transform.position, _soundRadius);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, _soundRadius);
    }
}
