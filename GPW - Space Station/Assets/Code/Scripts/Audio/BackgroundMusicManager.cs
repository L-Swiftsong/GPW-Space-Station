using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static System.TimeZoneInfo;

namespace Audio
{
    public class BackgroundMusicManager : Singleton<BackgroundMusicManager>
    {
        [SerializeField] private AudioClipSettings[] _defaultBackgroundClips;
        private AudioClipSettings[] _overridenAudioClips;
        int _currentAudioClipIndex;
        private AudioClipSettings[] _currentActiveBackgroundClips => _overridenAudioClips != null ? _overridenAudioClips : _defaultBackgroundClips;


        [SerializeField] private float _defaultFadeDuration = 1.0f;

        private float _currentVolumeOverride = 1.0f;
        private float _currentPitchOverride = 1.0f;

        private float _currentVolumeMultiplier = 1.0f;
        private float _currentPitchMultiplier = 1.0f;


        private Coroutine _handleTransitionCoroutine;
        private bool _isTransitioning = false;


        [Header("References")]
        [SerializeField] private AudioSource _primaryAudioSource;
        [SerializeField] private AudioSource _secondaryAudioSource;
        private bool _isCurrentSourcePrimary;

        private AudioSource _currentAudioSource => _isCurrentSourcePrimary ? _primaryAudioSource : _secondaryAudioSource;
        private AudioSource _otherAudioSource => _isCurrentSourcePrimary ? _secondaryAudioSource : _primaryAudioSource;


        protected override void Awake()
        {
            base.Awake();
            SelectNewAudioClip(0.0f);
        }
        private void Update()
        {
            if (_isTransitioning)
            {
                return;
            }


            if (_currentAudioSource.time >= (_currentAudioSource.clip.length - _defaultFadeDuration))
            {
                SelectNewAudioClip(_defaultFadeDuration);
            }
        }
        private void OnEnable() => _isTransitioning = false;


        #region OverrideBackgroundMusic Overloads

        public static void OverrideBackgroundMusic(AudioClip audioClip, float volume = 1.0f, float pitch = 1.0f, float transitionTime = 1.0f)
            => OverrideBackgroundMusic(new AudioClipSettings[1] { new AudioClipSettings(audioClip, volume, pitch) }, transitionTime);
        public static void OverrideBackgroundMusic(AudioClip[] audioClips, float volume, float pitch, float transitionTime = 1.0f)
        {
            AudioClipSettings[] audioClipSettings = new AudioClipSettings[audioClips.Length];
            for (int i = 0; i < audioClipSettings.Length; ++i)
            {
                audioClipSettings[i] = new AudioClipSettings(audioClips[i], volume, pitch);
            }

            OverrideBackgroundMusic(audioClipSettings, transitionTime);
        }
        public static void OverrideBackgroundMusic(AudioClip[] audioClips, float[] volume, float[] pitch, float transitionTime = 1.0f)
        {
            AudioClipSettings[] audioClipSettings = new AudioClipSettings[audioClips.Length];
            for(int i = 0; i < audioClipSettings.Length; ++i)
            {
                audioClipSettings[i] = new AudioClipSettings(audioClips[i], volume[i], pitch[i]);
            }

            OverrideBackgroundMusic(audioClipSettings, transitionTime);
        }

        public static void OverrideBackgroundMusic(AudioClipSettings[] audioClips, float transitionTime = 1.0f)
        {
            Instance._overridenAudioClips = audioClips;
            Instance.SelectNewAudioClip(transitionTime, skipSameClipCheck: true);
        }

        #endregion

        public static void PlaySingleClip(AudioClip audioClip, float baseVolume = 1.0f, float basePitch = 1.0f, float transitionTime = 1.0f) => PlaySingleClip(new AudioClipSettings(audioClip, baseVolume, basePitch), transitionTime);
        public static void PlaySingleClip(AudioClipSettings audioClip, float transitionTime = 1.0f)
        {
            Instance.TransitionToNewClip(audioClip, transitionTime);
        }

        public static void RemoveBackgroundMusicOverride(float transitionTime = 1.0f)
        {
            Instance._overridenAudioClips = null;
            Instance.SelectNewAudioClip(transitionTime, skipSameClipCheck: true);
        }


        #region Value Multiplier Overrides

        // Volume.
        public static void OverrideVolumeMultiplier(float newMultiplier) => Instance._currentVolumeMultiplier = newMultiplier;
        public static void ResetVolumeMultiplier() => Instance._currentVolumeMultiplier = 1.0f;

        // Pitch.
        public static void OverridePitchMultiplier(float newMultiplier) => Instance._currentPitchMultiplier = newMultiplier;
        public static void ResetPitchMultiplier() => Instance._currentPitchMultiplier = 1.0f;

        #endregion


        private void SelectNewAudioClip(float transitionTime, bool skipSameClipCheck = false)
        {
            Debug.Log("Selecting New Clip");

            if (skipSameClipCheck)
            {
                _currentAudioClipIndex = UnityEngine.Random.Range(0, _currentActiveBackgroundClips.Length);
            }
            else
            {
                int clipsCount = _currentActiveBackgroundClips.Length;
                if (clipsCount <= 1)
                {
                    // We only have 1 clip.
                    _currentAudioClipIndex = 0;
                }
                else
                {
                    // Select a NEW clip from our array.
                    int newClipIndex = UnityEngine.Random.Range(0, clipsCount - 1);
                    if (newClipIndex == _currentAudioClipIndex)
                    {
                        ++newClipIndex;
                    }
                
                    _currentAudioClipIndex = newClipIndex;
                }
            }
            

            TransitionToNewClip(_currentActiveBackgroundClips[_currentAudioClipIndex], transitionTime);
        }


        private void TransitionToNewClip(in AudioClipSettings audioClipSettings, float transitionTime)
        {
            if (transitionTime <= 0.0f)
            {
                // Instant transition.
                _currentAudioSource.Stop();
                _currentAudioSource.clip = audioClipSettings.AudioClip;
                
                _currentVolumeOverride = audioClipSettings.BaseVolume;
                _currentAudioSource.volume = GetDesiredVolume();

                _currentPitchOverride = audioClipSettings.BasePitch;
                _currentAudioSource.pitch = GetDesiredPitch();

                _currentAudioSource.Play();
            }
            else
            {
                // Gradual Transition.
                if (_handleTransitionCoroutine != null)
                {
                    StopCoroutine(_handleTransitionCoroutine);
                }
                _handleTransitionCoroutine = StartCoroutine(HandleTransition(audioClipSettings, transitionTime));
            }
        }
        private IEnumerator HandleTransition(AudioClipSettings audioClipSettings, float transitionTime)
        {
            _isTransitioning = true;
            _isCurrentSourcePrimary = !_isCurrentSourcePrimary;

            // Setup the new clip.
            _currentAudioSource.clip = audioClipSettings.AudioClip;

            _currentVolumeOverride = audioClipSettings.BaseVolume;
            _currentPitchOverride = audioClipSettings.BasePitch;

            _currentAudioSource.volume = 0.0f;
            _currentAudioSource.pitch = GetDesiredPitch();

            _currentAudioSource.Play();


            // Fade between the two clips.
            float fadeInRate = GetDesiredVolume() / transitionTime;
            float fadeOutRate = _otherAudioSource.volume / transitionTime;
            while(_otherAudioSource.volume > 0.0f)
            {
                _currentAudioSource.volume += fadeInRate * Time.deltaTime;
                _otherAudioSource.volume -= fadeOutRate * Time.deltaTime;

                yield return null;
            }

            // Ensure proper values.
            _otherAudioSource.volume = 0.0f;
            _currentAudioSource.volume = GetDesiredVolume();


            _isTransitioning = false;
        }


        private float GetDesiredVolume() => _currentVolumeOverride * _currentVolumeMultiplier;
        private float GetDesiredPitch() => _currentPitchOverride * _currentPitchMultiplier;
        


        #if UNITY_EDITOR
        
        private void Reset()
        {
            // Ensure that there are at least 2 AudioSources on this GameObject.
            AudioSource[] audioSources = this.gameObject.GetComponents<AudioSource>();
            if (audioSources != null && audioSources.Length != 0)
            {
                if (audioSources.Length == 1)
                {
                    // Audio Source Count == 1. Add another AudioSource.
                    _primaryAudioSource = audioSources[0];
                    _secondaryAudioSource = this.gameObject.AddComponent<AudioSource>();
                }
                else
                {
                    // Audio Source Count >= 2
                    _primaryAudioSource = audioSources[0];
                    _secondaryAudioSource = audioSources[1];
                }
            }
            else
            {
                // Audio Source Count == 0. Add two AudioSources.
                _primaryAudioSource = this.gameObject.AddComponent<AudioSource>();
                _secondaryAudioSource = this.gameObject.AddComponent<AudioSource>();
            }
        }

        #endif


        [System.Serializable]
        public struct AudioClipSettings
        {
            public AudioClip AudioClip;
            public float BaseVolume;
            public float BasePitch;

            public AudioClipSettings(AudioClip audioClip, float baseVolume = 1.0f, float basePitch = 1.0f)
            {
                this.AudioClip = audioClip;
                this.BaseVolume = baseVolume;
                this.BasePitch = basePitch;
            }
        }
    }
}
