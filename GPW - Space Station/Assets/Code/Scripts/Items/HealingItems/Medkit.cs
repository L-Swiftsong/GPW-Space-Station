using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Items.Healing
{
    public class Medkit : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private PlayerHealth _playerHealth;


        [Header("Healing Settings")]
        [SerializeField] private int _healingAmount = 1;
        [SerializeField] private float _healingDelay = 1.5f;


        [Header("Count Settings")]
        [SerializeField] [ReadOnly] private int _currentMedkitCount;
        [SerializeField] private int _maxMedkitCount;


        private void OnEnable()
        {
            PlayerInput.OnUseHealingItemStarted += PlayerInput_OnUseHealingItemStarted;
            //PlayerInput.OnUseHealingItemCancelled += PlayerInput_OnUseHealingItemCancelled;

            _playerHealth.OnUsedHealthKit += PlayerHealth_OnHealthKitUsed;
        }
        private void OnDisable()
        {
            PlayerInput.OnUseHealingItemStarted -= PlayerInput_OnUseHealingItemStarted;
            //PlayerInput.OnUseHealingItemCancelled -= PlayerInput_OnUseHealingItemCancelled;

            _playerHealth.OnUsedHealthKit -= PlayerHealth_OnHealthKitUsed;
        }


        private void PlayerInput_OnUseHealingItemStarted() => StartHealing();
        //private void PlayerInput_OnUseHealingItemCancelled() => CancelHealing();
        private void PlayerHealth_OnHealthKitUsed() => RemoveMedkits(1);


        public int AddMedkits(int numberToAdd)
        {
            if (_currentMedkitCount + numberToAdd > _maxMedkitCount)
            {
                // Taking all of this medkit would mean that we would be full.
                int overflow = _maxMedkitCount - _currentMedkitCount;
                _currentMedkitCount = _maxMedkitCount;
                return overflow;
            }
            else
            {
                // We can take all of the desired medkits.
                _currentMedkitCount += numberToAdd;
                return 0;
            }
        }
        public void SetMedkits(int newCount)
        {
            // Set our number of medkits, clamping against the maximum number we can hold.
            _currentMedkitCount = Mathf.Min(newCount, _maxMedkitCount);
        }
        public void RemoveMedkits(int numberToRemove)
        {
            // Reduce our number of medkits by the 'numberToRemove' towards 0.
            _currentMedkitCount = Mathf.Max(_currentMedkitCount - numberToRemove, 0);
        }


        public int GetCurrentCount() => _currentMedkitCount;


        private void StartHealing() => _playerHealth.StartHealing(_healingAmount, _healingDelay);
        private void CancelHealing() => _playerHealth.CancelHealing();
    }
}