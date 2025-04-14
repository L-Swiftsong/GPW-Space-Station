using Audio.Footsteps;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Entities.Mimic;

namespace Audio
{
    public class AnimationAudioTrigger : MonoBehaviour
    {
        [Header("Footstep Settings")]
        [SerializeField] private EntityFootstepClips _footstepClips;
        [SerializeField] private float _baseVolume = 1.0f;
        [SerializeField] private float _basePitch = 1.0f;
        [SerializeField] private float _walkVolume = 0.5f;
        [SerializeField] private float _chaseVolume = 1f;

        [Space(5)]
        [SerializeField] private float _minDistance = 1.0f;
        [SerializeField] private float _maxDistance = 50.0f;
        [SerializeField] private AnimationCurve _falloffCurve;

        [SerializeField] private GeneralMimic _generalMimic;


        public void PlayFootstep()
        {
            float volumeOverride = GetVolumeOverrideBasedOnState();

            FootstepClipInformation footstepClipValues = _footstepClips.GetAudioSettings(MovementState.Walking, volumeOverride);

            footstepClipValues.PitchRange *= _basePitch;

            if (_generalMimic != null && _generalMimic.GetCurrentState() == _generalMimic.GetWanderState())
            {
                _baseVolume = _walkVolume;
            }
            else
            {
                _baseVolume = _chaseVolume;
            }

            SFXManager.Instance.PlayClipAtPosition(
                footstepClipValues.FootstepClip,
                transform.position,
                minPitch: footstepClipValues.PitchRange.x,
                maxPitch: footstepClipValues.PitchRange.y,
                volume: _baseVolume,
                minDistance: _minDistance,
                maxDistance: _maxDistance,
                falloffCurve: _falloffCurve
            );
        }

        private float GetVolumeOverrideBasedOnState()
        {
            if (_generalMimic != null && _generalMimic.GetCurrentState() == _generalMimic.GetWanderState())
            {
                return 0.5f;
            }

            return -1f;
        }
    }
}
