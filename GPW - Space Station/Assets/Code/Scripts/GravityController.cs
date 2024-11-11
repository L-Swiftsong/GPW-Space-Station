using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityController : MonoBehaviour, IInteractable
{
    private ZeroGravityZone zeroGravityZone;

    private void Start()
    {
        zeroGravityZone = FindObjectOfType<ZeroGravityZone>();
    }

    public void Interact(PlayerInteraction playerInteraction)
    {
        if (zeroGravityZone != null)
        {
            zeroGravityZone.ToggleZeroGravity();
            Debug.Log("Gravity was toggled");
        }
    }
}

