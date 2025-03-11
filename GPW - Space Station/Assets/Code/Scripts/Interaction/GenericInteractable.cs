using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Audio;

namespace Interaction
{
    public class GenericInteractable : MonoBehaviour, IInteractable
    {
        [Header("Audio")]
        [SerializeField] private AudioClip[] _interactionAudioClips = new AudioClip[0];
        [SerializeField] private Vector3 _audioClipOffset = Vector3.zero;

        [Space(5)]
        [SerializeField] private float _pitchOffset = 0.05f;
        [SerializeField] private float _volume = 1.0f;


        [Header("Animation")]
        [SerializeField] private Animator _animator = null;
        private const string ANIMATION_TRIGGER_IDENTIFIER = "OnInteracted";


        #region IInteractable Properties

        private int _previousLayer;

        public event Action OnSuccessfulInteraction;
        public event Action OnFailedInteraction;

        #endregion


        public void Interact(PlayerInteraction interactingScript)
        {
            print("Sound Called");
            // Play Audio.
            if (_interactionAudioClips != null)
            {
                int length = _interactionAudioClips.Length;
                if (length > 0)
                {
                    int randomClipIndex = UnityEngine.Random.Range(0, length);
                    print("Sound Called");
                    SFXManager.Instance.PlayClipAtPosition(_interactionAudioClips[randomClipIndex], transform.TransformPoint(_audioClipOffset),
                        minPitch: 1.0f - _pitchOffset, maxPitch: 1.0f + _pitchOffset, volume: _volume);
                }
            }


            // Play Animation.
            if (_animator != null)
            {
                _animator.SetTrigger(ANIMATION_TRIGGER_IDENTIFIER);
            }


            OnSuccessfulInteraction?.Invoke();
        }

        public void Highlight() => IInteractable.StartHighlight(this.gameObject, ref _previousLayer);
        public void StopHighlighting() => IInteractable.StopHighlight(this.gameObject, _previousLayer);



#if UNITY_EDITOR

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(transform.TransformPoint(_audioClipOffset), 0.1f);
        }

#endif
    }
}