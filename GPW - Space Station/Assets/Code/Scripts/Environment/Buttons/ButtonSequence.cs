using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Environment.Buttons
{
    public class ButtonSequence : MonoBehaviour
    {
        [Header("Button Press Sequence")]
        [SerializeField] public SequenceButton[] _correctButtonSequence;
        private int _currentButtonIndex = 0;
        private bool _canAcceptInput = true;


        [Header("References")]
        [SerializeField] private GameObject _connectedObject;
        private ITriggerable _connectedTriggerable;
        [SerializeField] private Light[] _buttonLights;


        [Header("Colours")]
        [SerializeField] private Color _defaultColour = Color.white;
        [SerializeField] private Color _correctColour = Color.green;

        [Space(5)]
        [SerializeField] private Color _incorrectFlashColour = Color.red;
        [SerializeField] private int _incorrectFlashes = 3;
        [SerializeField] private float _incorrectFlashLifetime = 0.2f;


        void Awake()
        {
            foreach (Light light in _buttonLights)
            {
                light.enabled = true;
                light.color = _defaultColour;
            }
            _canAcceptInput = true;

            for (int i = 0; i < _correctButtonSequence.Length; ++i)
            {
                _correctButtonSequence[i].OnButtonPressed += ButtonPressed;
            }
        }
        private void OnDisable()
        {
            for (int i = 0; i < _correctButtonSequence.Length; ++i)
            {
                _correctButtonSequence[i].OnButtonPressed -= ButtonPressed;
            }
        }

        private void ButtonPressed(SequenceButton button)
        {
            if (!_canAcceptInput)
            {
                return;
            }


            if (_correctButtonSequence[_currentButtonIndex] == button)
            {
                Debug.Log(button.gameObject.name + " pressed correctly!");

                // Set the button light to display as correct.
                _buttonLights[_currentButtonIndex].color = _correctColour;

                _currentButtonIndex++;

                if (_currentButtonIndex == _correctButtonSequence.Length)
                {
                    Debug.Log("All buttons pressed correctly. Opening the door!");
                    CompletedCode();
                }
            }
            else
            {
                StartCoroutine(HandleIncorrectInput());
            }
        }

        private void CompletedCode()
        {
            if (_connectedTriggerable != null)
            {
                _connectedTriggerable.Trigger();
            }
        }

        private void ResetSequence()
        {
            Debug.Log("Wrong button! Resetting sequence.");
            _currentButtonIndex = 0;

            foreach (Light light in _buttonLights)
            {
                light.color = _defaultColour;
            }
        }

        private IEnumerator HandleIncorrectInput()
        {
            _canAcceptInput = false;
            
            yield return StartCoroutine(FailureFlash());
            ResetSequence();

            _canAcceptInput = true;
        }
        private IEnumerator FailureFlash()
        {
            WaitForSeconds incorrectFlashLifetime = new WaitForSeconds(_incorrectFlashLifetime);
            
            for (int i = 0; i < _incorrectFlashes; ++i)
            {
                // Incorrect flash.
                foreach (Light light in _buttonLights)
                {
                    light.color = _incorrectFlashColour;
                }
                yield return incorrectFlashLifetime;

                // Default flash.
                foreach (Light light in _buttonLights)
                {
                    light.color = _defaultColour;
                }
                yield return incorrectFlashLifetime;
            }
        }


    #if UNITY_EDITOR
        private void OnValidate()
        {
            if (_connectedObject != null)
            {
                // We have a reference to our Connected Object.
                if (!_connectedObject.TryGetComponent<ITriggerable>(out _connectedTriggerable))
                {
                    // The Connected Object doesn't have an instance of ITriggerable on it.
                    throw new System.ArgumentException($"{_connectedObject.name} does not have an instance of ITriggerable on it.");
                }
            }
        }
    #endif
    }
}