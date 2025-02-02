using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Audio.Footsteps
{
    [CreateAssetMenu(menuName = "Audio Containers/Footsteps/New Footstep Clip Container", fileName = "FootstepClipContainer", order = 2)]
    public class FootstepClipContainer : ScriptableObject
    {
        [SerializeField] private AudioClip[] _footstepClips;

        [SerializeField] private float _minPitch = 1.0f;
        [SerializeField] private float _maxPitch = 1.0f;


        #region Properties
        public AudioClip[] FootstepClips => _footstepClips;
        public float MinPitch => _minPitch;
        public float MaxPitch => _maxPitch;

        #endregion
    }
}
