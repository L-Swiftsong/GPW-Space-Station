using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Audio.Footsteps
{
    [CreateAssetMenu(menuName = "Audio Containers/Footsteps/New Entity Footstep Clips", fileName = "EntityFootstepClips", order = 0)]
    public class EntityFootstepClips : ScriptableObject
    {
        [SerializeField] private MaterialFootstepClips[] _materialFootstepClipsArray;

        public FootstepClipInformation GetAudioSettings(MovementState movementState, float volumeOverride = -1f)
        {
            return _materialFootstepClipsArray[0].GetAudioValues(movementState, volumeOverride);
        }
    }

    public class FootstepClipInformation
    {
        public AudioClip FootstepClip;
        public float VolumeMultiplier;
        public Vector2 PitchRange;
    }
}
