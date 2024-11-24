using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Environment.Doors;
using Interaction;
using System;

[RequireComponent(typeof(AudioSource))]
public class KeycardReader : MonoBehaviour, IInteractable
{
    [Header("Keycard Reader Settings")]
    [SerializeField] private GameObject _connectedObject;
    private ITriggerable _connectedTriggerable;
    [SerializeField] private int _securityLevel = 0;

    [Space(5)]
    [SerializeField] private bool _canOnlyActivate = false;

    [Space(5)]
    [SerializeField] private bool _limitedDuration = false;
    [SerializeField] private float _duration = 3.0f;
    private Coroutine _deactivateDoorCoroutine;


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
            // The player's keycard reader doesn't have a high enough security rating to use this KeycardReader.
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

        Activate();

        if (_successfulInteractionClip != null)
        {
            // Play the 'Interaction Successful' audio.
            _audioSource.PlayOneShot(_successfulInteractionClip);
        }
    }

    private void Activate()
    {
        if (_canOnlyActivate)
        {
            _connectedTriggerable.Activate();
        }
        else
        {
            _connectedTriggerable.Trigger();
        }


        if (_limitedDuration)
        {
            // This keycard reader only activates its trigger for a limited time.
            if (_deactivateDoorCoroutine != null)
            {
                StopCoroutine(_deactivateDoorCoroutine);
            }

            _deactivateDoorCoroutine = StartCoroutine(DeactivateAfterDelay());
        }
    }
    private IEnumerator DeactivateAfterDelay()
    {
        yield return new WaitForSeconds(_duration);
        _connectedTriggerable.Deactivate();
    }


    public int GetSecurityLevel() => _securityLevel;





#if UNITY_EDITOR
    private void OnValidate()
    {
        if (_connectedObject != null)
        {
            // We have a reference to our Connected Object.
            if (!_connectedObject.TryGetComponent<ITriggerable>(out _connectedTriggerable))
            {
                // The Connected Object doesn't have an instance of ITriggerable on it.
                throw new ArgumentException($"{_connectedObject.name} does not have an instance of ITriggerable on it.");
            }
        }
    }
#endif
}
