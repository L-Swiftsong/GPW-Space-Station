using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI
{
    [RequireComponent(typeof(MouseOnlySlider))]
    public class SliderMover : MonoBehaviour
    {
        private Slider _thisSlider;
        private GameObject _sliderGO;
        private float _minValue, _maxValue, _sliderRange;
        private float _currentChange;

        [Tooltip("What percentage of the bar do we cover in a second of continuous input.")]
            [SerializeField] private float _sliderSensitivity = 15.0f;


#if UNITY_EDITOR
        private void OnValidate()
        {
            if (this.GetComponent<Slider>().wholeNumbers)
            {
                Debug.LogWarning($"You cannot use the SliderMover script with a slider that uses whole numbers. \n Setting {this.name}'s wholeNumbers value to false");
                this.GetComponent<Slider>().wholeNumbers = false;
            }
        }
#endif


        private void Awake()
        {
            _thisSlider = GetComponent<Slider>();
            _sliderGO = this.gameObject;
        }
        private void OnEnable()
        {
            _minValue = _thisSlider.minValue;
            _maxValue = _thisSlider.maxValue;
            _sliderRange = _maxValue - _minValue;
        }

        private void Update()
        {
            // We need to cache the currentSelectedGO as otherwise the comparison doesn't trigger (It seems to read null in if statements).
            GameObject selectedGO = EventSystem.current.currentSelectedGameObject;
            if (_sliderGO == selectedGO)
            {
                // This slider is the selected UI element.
                _currentChange = PlayerInput.SliderHorizontal * (_sliderRange / _sliderSensitivity) * Time.deltaTime;
                _thisSlider.value = Mathf.Clamp(_thisSlider.value + _currentChange, _minValue, _maxValue);
            }
        }
    }
}