using Audio.Footsteps;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Audio
{
    public class AnimationAudioTrigger : MonoBehaviour
    {
        [Header("Footstep Settings")]
        [SerializeField] private EntityFootstepClips _footstepClips;
        [SerializeField] private float _baseVolume = 1.0f;
        [SerializeField] private float _basePitch = 1.0f;

        [Space(5)]
        [SerializeField] private float _minDistance = 1.0f;
        [SerializeField] private float _maxDistance = 50.0f;
        [SerializeField] private AnimationCurve _falloffCurve;

        public void PlayFootstep()
        {
            (AudioClip FootstepClip, Vector2 PitchRange) footstepClipValues = _footstepClips.GetAudioSettings(MovementState.Walking);
            footstepClipValues.PitchRange *= _basePitch;

            SFXManager.Instance.PlayClipAtPosition(footstepClipValues.FootstepClip, transform.position, minPitch: footstepClipValues.PitchRange.x, maxPitch: footstepClipValues.PitchRange.y, volume: _baseVolume, minDistance: _minDistance, maxDistance: _maxDistance, falloffCurve: _falloffCurve);
        }
    }
}
