using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Audio.Footsteps
{
    [CreateAssetMenu(menuName = "Audio Containers/Footsteps/New Material Footstep Clips", fileName = "[Material]FootstepClips", order = 1)]
    public class MaterialFootstepClips : ScriptableObject
    {
        /*private MaterialType _materialType;*/

        [SerializeField] private AudioClipContainer _defaultFootstepClips;
        [SerializeField] private MovementStateClipOverrides[] _movementStateClipOverridesArray;


        /*public MaterialType MaterialType => _materialType;*/



        public FootstepClipInformation GetAudioValues(MovementState movementState, float volumeOverride = -1f)
        {
            AudioClipContainer footstepClipContainer = _defaultFootstepClips;
            for (int i = 0; i < _movementStateClipOverridesArray.Length; ++i)
            {
                if (_movementStateClipOverridesArray[i].MovementState == movementState)
                {
                    footstepClipContainer = _movementStateClipOverridesArray[i].FootstepClipContainer;
                    break;
                }
            }

            int randomClipIndex = Random.Range(0, footstepClipContainer.AudioClips.Length);
            return new FootstepClipInformation()
            {
                FootstepClip = footstepClipContainer.AudioClips[randomClipIndex],
                VolumeMultiplier = (volumeOverride >= 0f) ? volumeOverride : footstepClipContainer.VolumeMultiplier,
                PitchRange = new Vector2(footstepClipContainer.MinPitch, footstepClipContainer.MaxPitch)
            };
        }



        [System.Serializable]
        private struct MovementStateClipOverrides
        {
            [SerializeField] private MovementState _movementState;
            [SerializeField] private AudioClipContainer _footstepClipContainer;

            public MovementState MovementState => _movementState;
            public AudioClipContainer FootstepClipContainer => _footstepClipContainer;
        }
    }
}