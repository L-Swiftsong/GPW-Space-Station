using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UI.Icons;

namespace Tutorials
{
    /// <summary>
    ///     Displays a message from a 'TutorialTextData' object on a world Canvas.
    /// </summary>
    public class InWorldTutorialDisplay : MonoBehaviour
    {
        [SerializeField] private TutorialTextData _tutorialTextData;
        [SerializeField] private bool _startEnabled = false;


        [Header("Flicker")]
        [SerializeField] private bool _useFlicker;
        [SerializeField] private AnimationCurve _enabledFlickerDuration;
        [SerializeField] private AnimationCurve _disabledFlickerDuration;
        private Coroutine _flickerCoroutine;


        [Header("World Canvas")]
        [SerializeField] private GameObject[] _disabledObjects;
        [SerializeField] private TMP_Text _tutorialText;


        private void Awake() => this.enabled = _startEnabled;

        public void Activate() => this.enabled = true;   // Enable the Display.
        public void Deactivate() => this.enabled = false;   // Disable the Display.


        private void OnEnable()
        {
            // Update and show the tutorial message.
            UpdateTutorialText();
            ShowCanvas();
            if (_useFlicker)
                StartFlickerCoroutine();

            // Subscribe to events.
            PlayerInput.OnInputDeviceChanged += UpdateTutorialText; // Update the tutorial message on device change.
        }
        private void OnDisable()
        {
            // Hide the tutorial message.
            HideCanvas();
            if (_useFlicker)
                StopFlickerCoroutine();

            // Unsubscribe from events.
            PlayerInput.OnInputDeviceChanged -= UpdateTutorialText;
        }


        public void SetTutorialTextData(TutorialTextData newTutorialTextData)
        {
            _tutorialTextData = newTutorialTextData;
            UpdateTutorialText();
        }


        private void ShowCanvas()
        {
            // Show the tutorial text and other desired objects.
            for(int i = 0; i < _disabledObjects.Length; ++i)
            {
                _disabledObjects[i].SetActive(true);
            }
        }

        private void UpdateTutorialText()
        {
            // Update the active sprite atlas.
            _tutorialText.spriteAsset = InputIconManager.GetSpriteAsset(PlayerInput.LastUsedDevice);

            // Update the tutorial text.
            _tutorialText.text = _tutorialTextData.GetFormattedTutorialText();
        }
        private void HideCanvas()
        {
            // Hide the tutorial text and other desired objects.
            for (int i = 0; i < _disabledObjects.Length; ++i)
            {
                _disabledObjects[i].SetActive(false);
            }
        }


        private void StartFlickerCoroutine()
        {
            StopFlickerCoroutine();
            _flickerCoroutine = StartCoroutine(FlickerCoroutine());
        }
        private void StopFlickerCoroutine()
        {
            if (_flickerCoroutine != null)
            {
                StopCoroutine(_flickerCoroutine);
            }
        }
        private IEnumerator FlickerCoroutine()
        {
            float flickerDuration;
            const float MIN_FLICKER_DURATION = 0.1f;    // Min duration to prevent rapid flashing.
            while(true)
            {
                ShowCanvas();
                flickerDuration = Mathf.Max(_enabledFlickerDuration.Evaluate(Random.Range(0.0f, 1.0f)), MIN_FLICKER_DURATION);
                yield return new WaitForSeconds(flickerDuration);

                HideCanvas();
                flickerDuration = Mathf.Max(_disabledFlickerDuration.Evaluate(Random.Range(0.0f, 1.0f)), MIN_FLICKER_DURATION);
                yield return new WaitForSeconds(flickerDuration);
            }
        }
    }
}