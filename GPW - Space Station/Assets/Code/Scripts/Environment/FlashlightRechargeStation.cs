using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Items;
using Interaction;
using Saving.LevelData;

namespace Environment
{
    /// <summary> An environmental item which recharges the player flashlight when interacted with.</summary>
    public class FlashlightRechargeStation : MonoBehaviour, IInteractable, ISaveableObject
    {
        #region Saving Variables

        [field: SerializeField] public SerializableGuid ID { get; set; }
        [SerializeField] private FlashlightChargeStationSaveInformation _saveData;

        #endregion


        private bool m_hasFlashlight = false;
        private bool _hasFlashlight
        {
            get => m_hasFlashlight;
            set
            {
                m_hasFlashlight = value;
                if (_saveData != null)
                    _saveData.HasFlashlight = value;
            }
        }


        [Header("Recharge Settings")]
        [SerializeField] private GameObject _flashlightModel;

        [Space(5)]
        [SerializeField] private float _rechargeRate = 20.0f;
        private float _maxBattery = 100.0f;
        private float _currentBattery = 0.0f;

        private Coroutine _rechargeFlashlightCoroutine;


        [Header("Effects")]
        [SerializeField] private AudioSource _rechargeAudioSource;
        [SerializeField] private AudioClip _rechargeClip;
        [SerializeField] private AudioClip _rechargeFinishClip;

        [Space(5)]
        [SerializeField] private GameObject _progressBarContainer;
        [SerializeField] private ProgressBar _rechargeProgressBar;


        #region IInteractable Properties & Events

        [field: SerializeField] public bool IsInteractable { get; set; } = true;

        private int _previousLayer;

        public event System.Action OnSuccessfulInteraction;
        public event System.Action OnFailedInteraction;

        #endregion



        private void Awake()
        {
            // Start with everything hidden.
            HideRechargeProgressBar();
            _flashlightModel.SetActive(false);
            _hasFlashlight = false;
        }


        public void Interact(PlayerInteraction playerInteraction)
        {
            if (playerInteraction.Inventory.HasFlashlight())
            {
                // The player has a flashlight.
                // Start recharging the player's flashlight.
                StartRecharge(playerInteraction.Inventory.GetFlashlightBattery());
                playerInteraction.Inventory.RemoveFlashlight();
            }
            else
            {
                // The player has no flashlight.
                if (!_hasFlashlight)
                {
                    // This recharge station has no flashlight.
                    OnFailedInteraction?.Invoke();
                    return;
                }

                // This recharge station has a flashlight.
                // Return the flashlight to the player.
                ReturnFlashlightToPlayer(playerInteraction.Inventory);
            }

            OnSuccessfulInteraction?.Invoke();
        }
        public void Highlight() => IInteractable.StartHighlight(this.gameObject, ref _previousLayer);
        public void StopHighlighting() => IInteractable.StopHighlight(this.gameObject, _previousLayer);


        private void StartRecharge(float initialBattery)
        {
            // Set values to start recharging the equipped flashlight.
            _hasFlashlight = true;
            _flashlightModel.SetActive(true);


            // Stop recharging if we currently are.
            CancelRecharge();

            // Start recharging the current flashlight.
            _currentBattery = initialBattery;
            ShowRechargeProgressBar();
            _rechargeFlashlightCoroutine = StartCoroutine(RechargeFlashlight());

			_rechargeAudioSource.Stop();
            if (_rechargeClip != null && _currentBattery < 100.0f)
            {
			    _rechargeAudioSource.clip = _rechargeClip;
                
                float progressPercentage = _currentBattery / 100.0f;
                float desiredTime = _rechargeClip.length * progressPercentage;
                Debug.Log(desiredTime);
                _rechargeAudioSource.time = desiredTime;

			    _rechargeAudioSource.Play();
            }
		}
        private void ResumeRecharge()
        {
            if (!_hasFlashlight)
            {
                // We don't have a flashlight in this station.
                return;
            }

            // Stop recharging if we currently are.
            CancelRecharge();

            // Resume recharging the current flashlight.
            ShowRechargeProgressBar();
            _rechargeFlashlightCoroutine = StartCoroutine(RechargeFlashlight());
        }
        private void CancelRecharge()
        {
            if (_rechargeFlashlightCoroutine != null)
            {
                StopCoroutine(_rechargeFlashlightCoroutine);

                _rechargeAudioSource.Stop();
            }

            HideRechargeProgressBar();
        }

        private void ReturnFlashlightToPlayer(PlayerInventory playerInventory)
        {
            // Hide our flashlight.
            _hasFlashlight = false;
            _flashlightModel.SetActive(false);

            // Hide the progress bar.
            HideRechargeProgressBar();

            // Allow the player to use their flashlight again.
            playerInventory.AddFlashlight(_currentBattery);
        }


        /// <summary> Recharge the current flashlight at a fixed rate over time.</summary>
        private IEnumerator RechargeFlashlight()
        {
            while (_currentBattery < _maxBattery)
            {
                // Recharge the flashlight at a fixed rate over time.
                _currentBattery = Mathf.MoveTowards(_currentBattery, _maxBattery, _rechargeRate * Time.deltaTime);

                if (_rechargeProgressBar != null)
                {
                    // Update the progress bar.
                    _rechargeProgressBar.SetValues(current: _currentBattery, min: 0.0f, max: _maxBattery);

                }

                yield return null;
            }

            _rechargeProgressBar.SetValues(current: _maxBattery, min: 0.0f, max: _maxBattery);


            // We have finished recharging.
            _rechargeAudioSource.Stop();
            if (_rechargeFinishClip != null)
            {
                _rechargeAudioSource.clip = _rechargeFinishClip;
                _rechargeAudioSource.Play();
            }
        }


        private void ShowRechargeProgressBar() => _progressBarContainer.SetActive(true);
        private void HideRechargeProgressBar() => _progressBarContainer.SetActive(false);



        #region Saving Functions

        public void BindExisting(ObjectSaveData saveData)
        {
            this._saveData = new FlashlightChargeStationSaveInformation(saveData);
            _saveData.ID = ID;

            ISaveableObject.PerformBindingChecks(this._saveData.ObjectSaveData, this);

            if (this._saveData.HasFlashlight)
            {
                StartRecharge(100.0f);
            }
        }
        public ObjectSaveData BindNew()
        {
            if (this._saveData == null || !this._saveData.Exists)
            {
                this._saveData = new FlashlightChargeStationSaveInformation(this.ID, ISaveableObject.DetermineDisabledState(this), _hasFlashlight);
            }

            ISaveableObject.UpdatePositionAndRotationInformation(this._saveData.ObjectSaveData, this);

            return this._saveData.ObjectSaveData;
        }
        public void InitialiseID()
        {
            ID = SerializableGuid.NewGuid();
            Debug.Log(ID);
        }

        protected virtual void OnEnable() => ISaveableObject.DefaultOnEnableSetting(this._saveData.ObjectSaveData, this);
        protected virtual void OnDestroy() => _saveData.DisabledState = DisabledState.Destroyed;
        protected virtual void OnDisable() => ISaveableObject.DefaultOnDisableSetting(this._saveData.ObjectSaveData, this);
        protected virtual void LateUpdate() => ISaveableObject.UpdatePositionAndRotationInformation(this._saveData.ObjectSaveData, this);

        #endregion
    }
}