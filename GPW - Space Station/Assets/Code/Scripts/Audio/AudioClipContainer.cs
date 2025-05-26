using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Audio
{
    [CreateAssetMenu(menuName = "Audio Containers/New Clip Container", fileName = "AudioClipContainer", order = 2)]
    public class AudioClipContainer : ScriptableObject
    {
        [SerializeField] private AudioClip[] _audioClips;

        [Space(5)]
        [SerializeField] private float _volumeMultiplier = 1.0f;

        [Space(5)]
        [SerializeField] private float _minPitch = 1.0f;
        [SerializeField] private float _maxPitch = 1.0f;


        #region Properties
        public AudioClip[] AudioClips => _audioClips;
        public AudioClip GetRandomClip() => _audioClips[Random.Range(0, _audioClips.Length)];
        
        public float VolumeMultiplier => _volumeMultiplier;

        public float MinPitch => _minPitch;
        public float MaxPitch => _maxPitch;

        #endregion
    }
}
