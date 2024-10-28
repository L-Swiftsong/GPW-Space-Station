using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HidingSpot : MonoBehaviour, IInteractable
{
    [SerializeField] private Vector3 _hidingPosition;

    public void Interact(PlayerInteraction interactingScript)
    {
        interactingScript.PlayerHide.StartHiding(transform.TransformPoint(_hidingPosition));
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.TransformPoint(_hidingPosition), Vector3.one * 0.5f);
    }
}
