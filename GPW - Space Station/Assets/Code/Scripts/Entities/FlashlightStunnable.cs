using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Entities
{
    public class FlashlightStunnable : MonoBehaviour, IStunnable
    {
        private float m_currentStun = 0.0f;
        private float _currentStun
        {
            get => m_currentStun;
            set
            {
                if (value >= 100.0f)
                {
                    m_currentStun = 100.0f;
                    _isStunned = true;
                }
                else if (value <= 0.0f)
                {
                    m_currentStun = 0.0f;
                    _isStunned = false;
                }
                else
                {
                    m_currentStun = value;
                }
            }
        }
        private bool _isStunned = false;
        public bool IsStunned => _isStunned;


        [Header("Stun Settings")]
        [SerializeField] private float _stunDuration = 2.0f;

        [Space(5)]
        [SerializeField] private float _stunDecreaseDelay = 0.75f;
        private float _stunDecreaseDelayRemaining = 0.0f;
        [SerializeField] private float _stunDecreaseRate = 20.0f;


        private void Update()
        {
            HandleStunDecrease();
        }


        private void HandleStunDecrease()
        {
            if (_stunDecreaseDelayRemaining > 0.0f && !_isStunned)
            {
                // We are not currently stunned, and stun was applied too recently for us to regenerate.
                _stunDecreaseDelayRemaining -= Time.deltaTime;
                return;
            }

            // Decrease our current stun towards 0.
            float stunDecreaseDelta = (_isStunned ? (100.0f / _stunDuration) : _stunDecreaseRate) * Time.deltaTime;
            _currentStun -= stunDecreaseDelta;
        }

        public void ApplyStun(float stunStrengthDelta)
        {
            _currentStun += stunStrengthDelta;
            _stunDecreaseDelayRemaining = _stunDecreaseDelay;
        }
    }
}