using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

namespace UI.EndStates
{
    public class EndCreditsUI : MonoBehaviour
    {
        [SerializeField] private UnityEngine.Audio.AudioMixerGroup _masterAudioGroup;
        private const string AUDIO_MIXER_VOLUME_IDENTIFIER = "MasterVolume";
        private const float AUDIO_FADE_DURATION = 2.0f;
        private static float s_initialMixerVolume = 0.0f;

        [Space(5)]
        [SerializeField] private CameraFocusLook _focusLookOverride = null;



        private void Awake() => Hide();
        
        public void TriggerCreditsDeathCutscene(Transform mimicRoot, Transform mimicLookAt)
        {
            DeathCutsceneManager.TriggerDeathCutscene(mimicRoot, mimicLookAt, OnDeathCutsceneComplete, _focusLookOverride);
        }


        private void OnDeathCutsceneComplete()
        {
            // Show Self.
            Show();

            // Fade out the audio and return to the main menu.
            StartCoroutine(FadeOutToMenu());
        }

        private IEnumerator FadeOutToMenu()
        {
            Debug.Log(_masterAudioGroup.audioMixer.GetFloat(AUDIO_MIXER_VOLUME_IDENTIFIER, out float s_initialMixerVolume));

            float audioFadeRate = 1.0f / AUDIO_FADE_DURATION;
            float lerpTime = 0.0f;
            while (lerpTime < 1.0f)
            {
                _masterAudioGroup.audioMixer.SetFloat(AUDIO_MIXER_VOLUME_IDENTIFIER, GetAudioValueForPercent(1.0f - lerpTime));

                yield return null;
                lerpTime += audioFadeRate * Time.deltaTime;
            }

            yield return new WaitForSecondsRealtime(0.75f); // Add a small delay to make it feel better.


            // Start returning to the main menu.
            SceneManagement.SceneLoader.Instance.ReloadToMainMenu(notifyListeners: false);

            // Reset our audio volume once back in the main menu.
            SceneManagement.SceneLoader.OnMainMenuReloadFinished += SceneLoader_OnMainMenuReloadFinished;
        }

        private void SceneLoader_OnMainMenuReloadFinished()
        {
            // Reset our audio volume to its original value.
            _masterAudioGroup.audioMixer.SetFloat(AUDIO_MIXER_VOLUME_IDENTIFIER, s_initialMixerVolume);
            SceneManagement.SceneLoader.OnMainMenuReloadFinished -= SceneLoader_OnMainMenuReloadFinished;
        }

        private void Show() => gameObject.SetActive(true);
        private void Hide() => gameObject.SetActive(false);


        private float GetAudioValueForPercent(float percentValue) => Mathf.Log10(Mathf.Clamp(percentValue, 0.0001f, 1f)) * 20.0f;
    }
}
