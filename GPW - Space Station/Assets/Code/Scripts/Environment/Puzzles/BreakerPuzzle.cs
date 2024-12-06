using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Environment.Buttons;
using System.Linq;

namespace Environment
{
    public class BreakerPuzzle : MonoBehaviour
    {
        [System.Serializable] private enum SwitchToggleType { Neighbours, Custom }
        [SerializeField] private SwitchToggleType _switchToggleType = SwitchToggleType.Neighbours;

        [Space(5)]
        [SerializeField] private BreakerSwitch[] _breakerSwitches;
        private int _breakerCount;

        [SerializeField] private Light _completionLight;


        private void Awake()
        {
            _breakerCount = _breakerSwitches.Length;

            // Setup the breakers.
            for (int i = 0; i < _breakerCount; ++i)
            {
                SetupBreaker(i);
            }

            _completionLight.enabled = false;
        }
        private void SetupBreaker(int index)
        {
            _breakerSwitches[index].Button.OnPressedEvent.AddListener(() => OnSwitchFlipped(index));
            _breakerSwitches[index].SetupEnabled(_breakerSwitches[index].StartEnabled);
        }


        private void OnSwitchFlipped(int switchIndex)
        {
            if (_switchToggleType == SwitchToggleType.Neighbours)
            {
                // Swtiches toggle their neighbours.

                if (switchIndex != 0)
                {
                    // This is NOT the first breaker.
                    // Toggle the previous breaker.
                    _breakerSwitches[switchIndex - 1].ToggleEnabled();
                }

                // Toggle this breaker.
                _breakerSwitches[switchIndex].ToggleEnabled();

                if (switchIndex != _breakerCount - 1)
                {
                    // This is NOT the final breaker.
                    // Toggle the next breaker.
                    _breakerSwitches[switchIndex + 1].ToggleEnabled();
                }
            }
            else if (_switchToggleType == SwitchToggleType.Custom)
            {
                // Switches have custom toggle settings.

                foreach(int toggledIndex in _breakerSwitches[switchIndex].ToggledSwitches)
                {
                    _breakerSwitches[toggledIndex].ToggleEnabled();
                }
            }


            CheckCompletionState();
        }

        private void CheckCompletionState()
        {
            if (_breakerSwitches.All(t => t.IsEnabled))
            {
                Debug.Log("Complete");
                _completionLight.enabled = true;
            }
        }


        [System.Serializable]
        private struct BreakerSwitch
        {
            public Button Button;
            public BreakerLight Light;
            public bool StartEnabled;

            public int[] ToggledSwitches;


            private bool m_isEnabled;
            public bool IsEnabled
            {
                get => m_isEnabled;
                private set
                {
                    m_isEnabled = value;
                    Light.SetIsEnabled(value);
                }
            }
            public void SetupEnabled(bool newValue)
            {
                IsEnabled = newValue;
                if (Button.TryGetComponent<BreakerPuzzleRotator>(out BreakerPuzzleRotator rotator))
                    rotator.SetValueInstant(newValue);
            }
            public void SetIsEnabled(bool newValue) => IsEnabled = newValue;
            public void ToggleEnabled() => IsEnabled = !IsEnabled;
        }
    }
}