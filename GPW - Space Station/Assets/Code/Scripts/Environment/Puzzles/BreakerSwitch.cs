using Environment.Buttons;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Environment.Puzzles
{
    [RequireComponent(typeof(Button))]
    public class BreakerSwitch : MonoBehaviour
    {
        [SerializeField] private GridBreakerPuzzle _parentPuzzle;
        private BreakerPuzzleRotator _rotator;

        private int _gridX;
        private int _gridY;
        private bool _isEnabled;


        private void Awake()
        {
            this.GetComponent<Button>().OnPressedEvent.AddListener(Button_OnPressedEvent);
            _rotator = GetComponent<BreakerPuzzleRotator>();
        }


        public void SetInitialValue(int gridX, int gridY, bool initialEnabledValue)
        {
            this._gridX = gridX;
            this._gridY = gridY;

            _isEnabled = initialEnabledValue;
            _rotator.SetValueInstant(_isEnabled);
        }
        public bool GetIsEnabled() => _isEnabled;

        
        private void Button_OnPressedEvent() => _parentPuzzle.SwitchToggled(_gridX, _gridY);
        public void ToggleEnabled() => SetEnabled(!this._isEnabled);
        public void SetEnabled(bool newValue)
        {
            this._isEnabled = newValue;
            _rotator.SetValue(newValue);
            Debug.Log("Set");
        }
    }
}
