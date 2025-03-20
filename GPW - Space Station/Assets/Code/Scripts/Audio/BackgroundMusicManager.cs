using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static System.TimeZoneInfo;

namespace Audio
{
    public class BackgroundMusicManager : Singleton<BackgroundMusicManager>
    {
        #region Defaults

        private const float DEFAULT_VOLUME = 1.0f;
        private const float DEFAULT_PITCH = 1.0f;
        private const float DEFAULT_INITIAL_TRANSITION_DELAY = 0.0f;
        private const float DEFAULT_INITIAL_TRANSITION_DURATION = 1.0f;

        #endregion



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
        private Coroutine _handleTransitionSecondaryCoroutine;
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
            SelectNewAudioClip(transitionTime: 0.0f, delayBetweenClips: 0.0f);
        }
        private void Update()
        {
            if (_isTransitioning)
            {
                return;
            }


            if (_currentAudioSource.time >= (_currentAudioSource.clip.length - _defaultFadeDuration))
            {
                SelectNewAudioClip(transitionTime: _defaultFadeDuration, delayBetweenClips: 0.0f);
            }
        }
        private void OnEnable() => _isTransitioning = false;


        #region OverrideBackgroundMusic Overloads

        public static void OverrideBackgroundMusic(AudioClip audioClip, float volume = DEFAULT_VOLUME, float pitch = DEFAULT_PITCH, float initialTransitionDelay = DEFAULT_INITIAL_TRANSITION_DELAY, float transitionTime = DEFAULT_INITIAL_TRANSITION_DURATION)
            => OverrideBackgroundMusic(new AudioClipSettings[1] { new AudioClipSettings(audioClip, volume, pitch) }, initialTransitionDelay, transitionTime);
        public static void OverrideBackgroundMusic(AudioClip[] audioClips, float volume, float pitch, float initialTransitionDelay = DEFAULT_INITIAL_TRANSITION_DELAY, float transitionTime = DEFAULT_INITIAL_TRANSITION_DURATION)
        {
            AudioClipSettings[] audioClipSettings = new AudioClipSettings[audioClips.Length];
            for (int i = 0; i < audioClipSettings.Length; ++i)
            {
                audioClipSettings[i] = new AudioClipSettings(audioClips[i], volume, pitch);
            }

            OverrideBackgroundMusic(audioClipSettings, initialTransitionDelay, transitionTime);
        }
        public static void OverrideBackgroundMusic(AudioClip[] audioClips, float[] volume, float[] pitch, float initialTransitionDelay = DEFAULT_INITIAL_TRANSITION_DELAY, float transitionTime = DEFAULT_INITIAL_TRANSITION_DURATION)
        {
            AudioClipSettings[] audioClipSettings = new AudioClipSettings[audioClips.Length];
            for(int i = 0; i < audioClipSettings.Length; ++i)
            {
                audioClipSettings[i] = new AudioClipSettings(audioClips[i], volume[i], pitch[i]);
            }

            OverrideBackgroundMusic(audioClipSettings, initialTransitionDelay, transitionTime);
        }
        public static void OverrideBackgroundMusic(AudioClipSettings audioClip, float initialTransitionDelay = DEFAULT_INITIAL_TRANSITION_DELAY, float transitionTime = DEFAULT_INITIAL_TRANSITION_DURATION) => OverrideBackgroundMusic(new AudioClipSettings[1] { audioClip }, initialTransitionDelay, transitionTime);

        public static void OverrideBackgroundMusic(AudioClipSettings[] audioClips, float initialTransitionDelay = DEFAULT_INITIAL_TRANSITION_DELAY, float transitionTime = DEFAULT_INITIAL_TRANSITION_DURATION)
        {
            Instance._overridenAudioClips = audioClips;
            Instance.SelectNewAudioClip(transitionTime, initialTransitionDelay, skipSameClipCheck: true);
        }

        #endregion

        public static void PlaySingleClip(AudioClip audioClip, float baseVolume = DEFAULT_VOLUME, float basePitch = DEFAULT_PITCH, float initialTransitionDelay = DEFAULT_INITIAL_TRANSITION_DELAY, float transitionTime = DEFAULT_INITIAL_TRANSITION_DURATION) => PlaySingleClip(new AudioClipSettings(audioClip, baseVolume, basePitch), initialTransitionDelay, transitionTime);
        public static void PlaySingleClip(AudioClipSettings audioClip, float initialTransitionDelay = DEFAULT_INITIAL_TRANSITION_DELAY, float transitionTime = DEFAULT_INITIAL_TRANSITION_DURATION)
        {
            Instance.TransitionToNewClip(audioClip, initialTransitionDelay, transitionTime);
        }

        public static void RemoveBackgroundMusicOverride(float initialTransitionDelay = DEFAULT_INITIAL_TRANSITION_DELAY, float transitionTime = DEFAULT_INITIAL_TRANSITION_DURATION)
        {
            Instance._overridenAudioClips = null;
            Instance.SelectNewAudioClip(transitionTime, initialTransitionDelay, skipSameClipCheck: true);
        }

        public static void PauseBackgroundMusicForDuration(float pauseDuration)
        {
            Instance.PauseBackgroundMusicForDuration_NonStatic(pauseDuration);
        }
        private void PauseBackgroundMusicForDuration_NonStatic(float pauseDuration)
        {
            StopActiveTransitions(true);
            _currentAudioSource.Pause();
            _handleTransitionCoroutine = StartCoroutine(TriggerAfterDelay(pauseDuration, () => _currentAudioSource.UnPause()));
        }
        public static void PauseBackgroundMusic()
        {
            Instance.StopActiveTransitions(true);
            Instance._currentAudioSource.Pause();
        }
        public static void ResumeBackgroundMusic()
        {
            Instance.StopActiveTransitions(true);
            Instance._currentAudioSource.UnPause();
        }


        #region Value Multiplier Overrides

        // Volume.
        public static void OverrideVolumeMultiplier(float newMultiplier) => Instance._currentVolumeMultiplier = newMultiplier;
        public static void ResetVolumeMultiplier() => Instance._currentVolumeMultiplier = 1.0f;

        // Pitch.
        public static void OverridePitchMultiplier(float newMultiplier) => Instance._currentPitchMultiplier = newMultiplier;
        public static void ResetPitchMultiplier() => Instance._currentPitchMultiplier = 1.0f;

        #endregion


        private void SelectNewAudioClip(float transitionTime, float delayBetweenClips, bool skipSameClipCheck = false)
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
            

            TransitionToNewClip(_currentActiveBackgroundClips[_currentAudioClipIndex], delayBetweenClips, transitionTime);
        }


        private void TransitionToNewClip(in AudioClipSettings audioClipSettings, float delayBetweenClips, float transitionTime)
        {
            StopActiveTransitions(false);

            if (transitionTime <= 0.0f)
            {
                // Instant transition.
                _currentAudioSource.Stop();
                _currentAudioSource.clip = audioClipSettings.AudioClip;
                
                _currentVolumeOverride = audioClipSettings.BaseVolume;
                _currentAudioSource.volume = GetDesiredVolume();

                _currentPitchOverride = audioClipSettings.BasePitch;
                _currentAudioSource.pitch = GetDesiredPitch();

                _handleTransitionCoroutine = StartCoroutine(TriggerAfterDelay(delayBetweenClips, () => _currentAudioSource.Play()));
            }
            else
            {
                // Gradual Transition.
                _isTransitioning = true;
                
                _handleTransitionSecondaryCoroutine = StartCoroutine(FadeOutCurrentClip(_currentAudioSource, transitionTime));

                _isCurrentSourcePrimary = !_isCurrentSourcePrimary;
                _handleTransitionCoroutine = StartCoroutine(FadeInNewClip(_currentAudioSource, audioClipSettings, delayBetweenClips, transitionTime));
            }
        }

        private IEnumerator FadeOutCurrentClip(AudioSource audioSource, float transitionTime)
        {
            // Fade the clip out.
            float fadeOutRate = audioSource.volume / transitionTime;
            while (audioSource.volume > 0.0f)
            {
                audioSource.volume -= fadeOutRate * Time.deltaTime;

                yield return null;
            }

            // Ensure proper values.
            audioSource.volume = 0.0f;
            audioSource.Stop();
        }
        private IEnumerator FadeInNewClip(AudioSource audioSource, AudioClipSettings audioClipSettings, float transitionEntryDelay, float transitionTime)
        {
            // Setup the new clip.
            audioSource.clip = audioClipSettings.AudioClip;

            _currentVolumeOverride = audioClipSettings.BaseVolume;
            _currentPitchOverride = audioClipSettings.BasePitch;

            audioSource.volume = 0.0f;
            audioSource.pitch = GetDesiredPitch();

            yield return new WaitForSeconds(transitionEntryDelay);

            audioSource.Play();


            // Fade between the two clips.
            float fadeInRate = GetDesiredVolume() / transitionTime;
            while(audioSource.volume < GetDesiredVolume())
            {
                audioSource.volume += fadeInRate * Time.deltaTime;

                yield return null;
            }

            // Ensure proper values.
            audioSource.volume = GetDesiredVolume();


            _isTransitioning = false;
        }
        private IEnumerator TriggerAfterDelay(float delay, System.Action callback)
        {
            yield return new WaitForSeconds(delay);
            callback?.Invoke();
        }

        private void StopActiveTransitions(bool updateValues)
        {
            if (_handleTransitionCoroutine != null)
            {
                StopCoroutine(_handleTransitionCoroutine);
            }
            if (_handleTransitionSecondaryCoroutine != null)
            {
                StopCoroutine(_handleTransitionSecondaryCoroutine);
            }


            if (updateValues)
            {
                _currentAudioSource.volume = GetDesiredVolume();
                _currentAudioSource.pitch = GetDesiredPitch();

                _otherAudioSource.volume = 0.0f;
                _otherAudioSource.Stop();
            }
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
