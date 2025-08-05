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


        [Header("World Canvas")]
        [SerializeField] private GameObject _worldCanvasRoot;
        [SerializeField] private TMP_Text _tutorialText;


        private void Awake() => this.enabled = false;   // Start Disabled.
        public void Trigger() => this.enabled = true;   // Enable the Display.


        private void OnEnable()
        {
            // Update and show the tutorial message.
            ShowCanvas();

            // Subscribe to events.
            PlayerInput.OnInputDeviceChanged += ShowCanvas; // Update the tutorial message on device change.
        }
        private void OnDisable()
        {
            // Hide the tutorial message.
            HideCanvas();

            // Unsubscribe from events.
            PlayerInput.OnInputDeviceChanged -= ShowCanvas;
        }


        private void ShowCanvas()
        {
            // Show the tutorial text.
            _worldCanvasRoot.SetActive(true);

            // Update the active sprite atlas.
            _tutorialText.spriteAsset = InputIconManager.GetSpriteAsset(PlayerInput.LastUsedDevice);

            // Update the tutorial text.
            _tutorialText.text = _tutorialTextData.GetFormattedTutorialText();
        }
        private void HideCanvas()
        {
            // Hide the tutorial text.
            _worldCanvasRoot.SetActive(false);
        }
    }
}