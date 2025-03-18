using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static System.TimeZoneInfo;

namespace Audio
{
    public class BackgroundMusicManager : Singleton<BackgroundMusicManager>
    {
        [SerializeField] private AudioClip[] _defaultBackgroundClips;
        private AudioClip[] _overridenAudioClips;
        int _currentAudioClipIndex;
        private AudioClip[] _currentActiveBackgroundClips => _overridenAudioClips != null ? _overridenAudioClips : _defaultBackgroundClips;


        [SerializeField] private float _defaultFadeDuration = 1.0f;
        [SerializeField] private float _defaultVolume = 1.0f;


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
        


        public static void OverrideBackgroundMusic(AudioClip audioClip, float transitionTime = 1.0f) => OverrideBackgroundMusic(new AudioClip[1] { audioClip }, transitionTime);
        public static void OverrideBackgroundMusic(AudioClip[] audioClips, float transitionTime = 1.0f)
        {
            Instance._overridenAudioClips = audioClips;
            Instance.SelectNewAudioClip(transitionTime, skipSameClipCheck: true);
        }
        public static void PlaySingleClip(AudioClip audioClip, float transitionTime = 1.0f)
        {
            Instance.TransitionToNewClip(audioClip, transitionTime);
        }

        public static void RemoveBackgroundMusicOverride(float transitionTime = 1.0f)
        {
            Instance._overridenAudioClips = null;
            Instance.SelectNewAudioClip(transitionTime, skipSameClipCheck: true);
        }


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


        private void TransitionToNewClip(AudioClip audioClip, float transitionTime)
        {
            if (transitionTime <= 0.0f)
            {
                // Instant transition.
                _currentAudioSource.Stop();
                _currentAudioSource.clip = audioClip;
                _currentAudioSource.volume = _defaultVolume;
                _currentAudioSource.Play();
            }
            else
            {
                // Gradual Transition.
                if (_handleTransitionCoroutine != null)
                {
                    StopCoroutine(_handleTransitionCoroutine);
                }
                _handleTransitionCoroutine = StartCoroutine(HandleTransition(audioClip, transitionTime));
            }
        }
        private IEnumerator HandleTransition(AudioClip audioClip, float transitionTime)
        {
            _isTransitioning = true;

            // Setup the new clip.
            _otherAudioSource.clip = audioClip;
            _otherAudioSource.volume = 0.0f;
            _otherAudioSource.Play();


            float transitionRate = _defaultVolume / transitionTime;
            while(_otherAudioSource.volume < _defaultVolume)
            {
                _otherAudioSource.volume += transitionRate * Time.deltaTime;
                _currentAudioSource.volume -= transitionRate * Time.deltaTime;

                yield return null;
            }

            // Ensure proper values.
            _currentAudioSource.volume = 0.0f;
            _otherAudioSource.volume = _defaultVolume;

            // Update current audio source.
            _isCurrentSourcePrimary = !_isCurrentSourcePrimary;

            _isTransitioning = false;
        }


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
    }
}
