using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Audio;

namespace Interaction
{
    public class PlayAudioOnInteraction : MonoBehaviour
    {
        private IInteractable _interactableScript;

        [SerializeField] private AudioClip _successAudioClip;
        [SerializeField] private AudioClip _failedAudioClip;

        [Space(5)]
        [SerializeField] private float _volume = 1.0f;
        [SerializeField] private float _minPitch = 1.0f;
        [SerializeField] private float _maxPitch = 1.0f;
        [SerializeField] private float _minDistance = 1.0f;


        private void Awake() => _interactableScript = GetComponent<IInteractable>();
        private void OnEnable()
        {
            if (_successAudioClip != null)
                _interactableScript.OnSuccessfulInteraction += PlaySuccessAudioClip;
            if (_failedAudioClip != null)
                _interactableScript.OnFailedInteraction += PlayFailedAudioClip;
        }
        private void OnDisable()
        {
            _interactableScript.OnSuccessfulInteraction -= PlaySuccessAudioClip;
            _interactableScript.OnFailedInteraction -= PlayFailedAudioClip;
        }


        private void PlaySuccessAudioClip() => SFXManager.Instance.PlayClipAtPosition(_successAudioClip, transform.position, minPitch: _minPitch, maxPitch: _maxPitch, volume: _volume, minDistance: _minDistance);
        private void PlayFailedAudioClip() => SFXManager.Instance.PlayClipAtPosition(_failedAudioClip, transform.position, minPitch: _minPitch, maxPitch: _maxPitch, volume: _volume, minDistance: _minDistance);



#if UNITY_EDITOR

        private void OnValidate()
        {
            if (this.TryGetComponent<IInteractable>(out _) == false)
            {
                Debug.LogError($"Error: A {this.GetType().Name} instance is on an object with no IInteractable script.", this);
            }
        }

        #endif
    }
}