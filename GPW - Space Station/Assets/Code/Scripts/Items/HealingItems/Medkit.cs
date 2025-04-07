using Audio;
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

		[Header("Animation")]
		private Animator _animator;
		private const string ANIMATOR_EQUIPPED_IDENTIFIER = "IsEquipped";

		[Header("Audio")]
		[SerializeField] private AudioClip _healSound;
		[SerializeField] private AudioSource _audioSource;

        [Header("VFX")]
        [SerializeField] private ParticleSystem _particleSystem;

		private void Awake()
		{
			_animator = GetComponent<Animator>();
		}

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


        private void PlayerInput_OnUseHealingItemStarted()
        {
            StartHealing();
        }
        //private void PlayerInput_OnUseHealingItemCancelled() => CancelHealing();
        private void PlayerHealth_OnHealthKitUsed()
        {
            Unequip();
            RemoveMedkits(1);
            _particleSystem.Stop();
        }

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


        private void StartHealing()
        {
            Equip();
            _playerHealth.StartHealing(_healingAmount, _healingDelay);
            SFXManager.Instance.PlayClipAtPosition(_healSound, transform.position, 1, 1, 2);
            _particleSystem.Play();
        }

        private void CancelHealing() => _playerHealth.CancelHealing();

		private void Equip()
		{
			Debug.Log("Equip");
			_animator.SetBool(ANIMATOR_EQUIPPED_IDENTIFIER, true);
		}
		private void Unequip()
		{
			_animator.SetBool(ANIMATOR_EQUIPPED_IDENTIFIER, false);
		}
	}
}