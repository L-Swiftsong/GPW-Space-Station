using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Environment.Doors;

[RequireComponent(typeof(AudioSource))]
public class KeycardReader : MonoBehaviour, IInteractable
{
    [Header("Keycard Reader Settings")]
    [SerializeField] private ExternalInputDoor _connectedDoor;
    [SerializeField] private int _securityLevel = 0;

    [Space(5)]
    [SerializeField] private bool _canCloseDoor = false;

    [Space(5)]
    [SerializeField] private bool _limitedDuration = false;
    [SerializeField] private float _duration = 3.0f;
    private Coroutine _closeDoorCoroutine;


    [Header("SFX")]
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip _successfulInteractionClip;
    [SerializeField] private AudioClip _failedInteractionClip;


    public static event System.EventHandler OnAnyKeycardReaderHighlighted;
    public static event System.EventHandler OnAnyKeycardReaderStopHighlighted;


    public void Interact(PlayerInteraction interactingScript)
    {
        if (!interactingScript.Inventory.HasKeycardDecoder())
        {
            // The player doesn't have a keycard decoder.
            Debug.Log("No Decoder");
            return;
        }
        if (_securityLevel > interactingScript.Inventory.GetDecoderSecurityLevel())
        {
            // The player's keycard reader doesn't have a high enough security rating to open this KeycardReader.
            FailedInteraction();
            return;
        }

        // The player has a keycard reader of a valid security level.
        SuccessfulInteraction();
    }
    public void Highlight() => OnAnyKeycardReaderHighlighted?.Invoke(this, System.EventArgs.Empty);
    public void StopHighlighting() => OnAnyKeycardReaderStopHighlighted?.Invoke(this, System.EventArgs.Empty);


    private void FailedInteraction()
    {
        Debug.Log("Failed Interaction");

        if (_failedInteractionClip != null)
        {
            // Play the 'Interaction Failed' audio.
            _audioSource.PlayOneShot(_failedInteractionClip);
        }
    }
    private void SuccessfulInteraction()
    {
        Debug.Log("Successful Interaction");

        OpenDoor();

        if (_successfulInteractionClip != null)
        {
            // Play the 'Interaction Successful' audio.
            _audioSource.PlayOneShot(_successfulInteractionClip);
        }
    }

    private void OpenDoor()
    {
        if (_canCloseDoor)
        {
            _connectedDoor.ToggleOpen();
        }
        else
        {
            _connectedDoor.Open();
        }


        if (_limitedDuration)
        {
            // This keycard reader only keeps its door open for a limited time.
            if (_closeDoorCoroutine != null)
            {
                StopCoroutine(_closeDoorCoroutine);
            }

            _closeDoorCoroutine = StartCoroutine(CloseAfterDelay());
        }
    }
    private IEnumerator CloseAfterDelay()
    {
        yield return new WaitForSeconds(_duration);
        _connectedDoor.Close();
    }


    public int GetSecurityLevel() => _securityLevel;
}
