using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Collider))]
public class VentEntrance : MonoBehaviour
{
    [SerializeField] private bool _isOmnidirectional = false;
    public bool IsOmnidirectional => _isOmnidirectional;


#if UNITY_EDITOR
    private void OnValidate()
    {
        GetComponent<Collider>().isTrigger = true;
    }
#endif

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<PlayerController>(out PlayerController playerController))
        {
            if (!_isOmnidirectional)
            {
                Vector3 directionToPlayer = (other.transform.position - transform.position).normalized;
                if (Vector3.Dot(transform.forward, directionToPlayer) < 0.0f)
                {
                    // The player isn't entering from this vent entrance.
                    return;
                }
            }

            playerController.TryStartCrawling();
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<PlayerController>(out PlayerController playerController))
        {
            if (!_isOmnidirectional)
            {
                Vector3 directionToPlayer = (other.transform.position - transform.position).normalized;
                if (Vector3.Dot(transform.forward, directionToPlayer) < 0.0f)
                {
                    // The player isn't exiting from this vent entrance.
                    return;
                }
            }

            playerController.TryStopCrawling();
        }
    }
}
