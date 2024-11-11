using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZeroGravityZone : MonoBehaviour
{
    [Header("Tags")]

    [SerializeField]
    private string gravityTag = "Gravity";

    private bool isZeroGravityActive = false;

    private void Start()
    {
    }
    public void ToggleZeroGravity()
    {
        isZeroGravityActive = !isZeroGravityActive;

        Rigidbody[] rigidbodies = FindObjectsOfType<Rigidbody>();

        foreach (Rigidbody rb in rigidbodies)
        {
            if (rb.CompareTag(gravityTag))
            {
                if (isZeroGravityActive)
                {
                    rb.useGravity = false;
                    ApplyFloatToCeiling(rb);
                }
                else
                {
                    rb.useGravity = true;
                    ResetObjectPosition(rb);
                }
            }
        }
    }

    private void ApplyFloatToCeiling(Rigidbody rb)
    {
        rb.velocity = Vector3.zero;  
        rb.AddForce(Vector3.up * 3f, ForceMode.Impulse);  
    }

    private void ResetObjectPosition(Rigidbody rb)
    {
        rb.velocity = Vector3.zero;  
        rb.AddForce(Vector3.down * 10f, ForceMode.Impulse);  
    }
}
