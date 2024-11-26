using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Environment.Buttons
{
    public class Keypad : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private GameObject _connectedObject;
        private ITriggerable _connectedTriggerable;
        [SerializeField] private TextMeshPro _displayText;


        [Header("Code Settings")]
        [SerializeField] private string _correctCode = "0210";
        private string _playerInput = "";


        private void Awake()
        {
            _connectedTriggerable = _connectedObject.GetComponent<ITriggerable>();
        }


        public void ButtonPressed(string number)
        {
            if (_playerInput.Length < _correctCode.Length)
            {
                _playerInput += number;
                _displayText.text = _playerInput;
            }
        }
        public void EnterCode()
        {
            if (_playerInput == _correctCode)
            {
                _connectedTriggerable.Activate();
            }
            else
            {
                _playerInput = "";
                _displayText.text = "";
            }
        }
        public void DeleteLastCharacter()
        {
            if(_playerInput.Length > 0)
            {
                _playerInput = _playerInput.Substring(0, _playerInput.Length - 1); 

                _displayText.text = _playerInput;
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