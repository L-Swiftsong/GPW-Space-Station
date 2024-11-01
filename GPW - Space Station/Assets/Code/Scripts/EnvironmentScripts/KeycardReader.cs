using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Environment.Doors;

public class KeycardReader : MonoBehaviour, IInteractable
{
    [SerializeField] private ExternalInputDoor _connectedDoor;
    [SerializeField] private int _requiredKeycardID = -1;
    public int RequiredKeycardID => _requiredKeycardID;

    [Space(5)]
    [SerializeField] private bool _interactOnlyOpens = false;

    [Space(5)]
    [SerializeField] private bool _limitedDuration = false;
    [SerializeField] private float _duration = 3.0f;
    private Coroutine _closeDoorCoroutine;


    private void Start()
    {
        if (_requiredKeycardID == -1)
        {
            Debug.LogError("Error: Required Keycard Index of KeycardReader " + this.name +  " is not set");
        }
    }


    public void Interact(PlayerInteraction interactingScript)
    {
        if (TestKeycard(interactingScript.Inventory))
        {
            if (_interactOnlyOpens)
            {
                _connectedDoor.Open();
            }
            else
            {
                _connectedDoor.ToggleOpen();
            }


            if (_limitedDuration)
            {
                if (_closeDoorCoroutine != null)
                    StopCoroutine(_closeDoorCoroutine);
                _closeDoorCoroutine = StartCoroutine(CloseAfterDelay());
            }
        }
    }
    private bool TestKeycard(PlayerInventory playerInventory) => playerInventory.HasKeyCard(_requiredKeycardID);


    private IEnumerator CloseAfterDelay()
    {
        yield return new WaitForSeconds(_duration);
        _connectedDoor.Close();
    }


    public void OverrideMaterial(Material overrideMaterial)
    {
        // Override the material list of the renderers in the keycard reader.
        foreach (Renderer renderer in transform.GetComponentsInChildren<Renderer>())
        {
            renderer.material = overrideMaterial;
        }

        // Ensure our connected door matches our override material.
        _connectedDoor.OverrideMaterial(overrideMaterial);
    }
}
