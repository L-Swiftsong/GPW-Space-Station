using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Interaction;
using Saving.LevelData;

namespace Environment.Buttons
{
    public class KeycardReader : MonoBehaviour, IInteractable, ISaveableObject
    {
        #region Saving Properties

        [field: SerializeField] public SerializableGuid ID { get; set; } = SerializableGuid.NewGuid();
        [SerializeField] private KeycardReaderSaveInformation _saveData;

        #endregion


        [Header("Keycard Reader Settings")]
        [SerializeField] private GameObject _connectedObject;
        private ITriggerable _connectedTriggerable;
        [SerializeField] private int _securityLevel = 0;

        [Space(5)]
        [SerializeField] private bool _isUnlocked = false;
        [SerializeField] private bool _alsoTriggerWhenFirstUnlocked = true;

        [Space(5)]
        [SerializeField] private bool _canOnlyActivate = false;

        [Space(5)]
        [SerializeField] private bool _limitedDuration = false;
        [SerializeField] private float _duration = 3.0f;
        private Coroutine _deactivateDoorCoroutine;


        [Header("GFX")]
        [SerializeField] private MeshRenderer _renderer;
        [SerializeField] private int _keycardScreenMaterialIndex;

        private MaterialPropertyBlock _materialPropertyBlock;
        private const string SECURITY_LEVEL_IDENTIFIER = "_SecurityLevel";
        private const string IS_UNLOCKED_IDENTIFIER = "_IsSecurityLevelValid";
        private const string NOISE_OFFSET_IDENTIFIER = "_NoiseTimeOffset";


        public static event System.EventHandler OnAnyKeycardReaderHighlighted;
        public static event System.EventHandler OnAnyKeycardReaderStopHighlighted;


        #region IInteractable Properties & Events

        private int _previousLayer;

        public event System.Action OnSuccessfulInteraction;
        public event System.Action OnFailedInteraction;

        #endregion


        private void Awake()
        {
            _connectedTriggerable = _connectedObject.GetComponent<ITriggerable>();

            // Setup the MaterialPropertyBlock.
            _materialPropertyBlock = new MaterialPropertyBlock();
            _materialPropertyBlock.SetInteger(SECURITY_LEVEL_IDENTIFIER, _securityLevel);
            _materialPropertyBlock.SetInteger(IS_UNLOCKED_IDENTIFIER, _isUnlocked ? 1 : 0);
            _materialPropertyBlock.SetFloat(NOISE_OFFSET_IDENTIFIER, Random.Range(0.01f, 5.0f));

            // Setup the Keycard Screen.
            _renderer.SetPropertyBlock(_materialPropertyBlock);
        }


        public void Interact(PlayerInteraction interactingScript)
        {
            if (!_isUnlocked)
            {
                if (!interactingScript.Inventory.HasKeycardDecoder())
                {
                    // The player doesn't have a keycard decoder.
                    Debug.Log("No Decoder");
                    return;
                }
                if (_securityLevel > interactingScript.Inventory.GetDecoderSecurityLevel())
                {
                    // The player's keycard reader doesn't have a high enough security rating to use this KeycardReader.
                    FailedInteraction();
                    return;
                }
            }

            // The player has a keycard reader of a valid security level.
            SuccessfulInteraction();
        }
        public void Highlight()
        {
            OnAnyKeycardReaderHighlighted?.Invoke(this, System.EventArgs.Empty);
            IInteractable.StartHighlight(this.gameObject, ref _previousLayer);
        }
        public void StopHighlighting()
        {
            OnAnyKeycardReaderStopHighlighted?.Invoke(this, System.EventArgs.Empty);
            IInteractable.StopHighlight(this.gameObject, _previousLayer);
        }


        private void FailedInteraction()
        {
            Debug.Log("Failed Interaction");
            OnFailedInteraction?.Invoke();
        }
        private void SuccessfulInteraction()
        {
            Debug.Log("Successful Interaction");
            OnSuccessfulInteraction?.Invoke();
            
            if (!_isUnlocked)
            {
                _isUnlocked = true;

                _renderer.GetPropertyBlock(_materialPropertyBlock);
                _materialPropertyBlock.SetInteger(IS_UNLOCKED_IDENTIFIER, 1);
                _renderer.SetPropertyBlock(_materialPropertyBlock);

                if (!_alsoTriggerWhenFirstUnlocked)
                {
                    return;
                }
            }
            
            Activate();
        }

        private void Activate()
        {
            if (_canOnlyActivate)
            {
                _connectedTriggerable.Activate();
            }
            else
            {
                _connectedTriggerable.Trigger();
            }


            if (_limitedDuration)
            {
                // This keycard reader only activates its trigger for a limited time.
                if (_deactivateDoorCoroutine != null)
                {
                    StopCoroutine(_deactivateDoorCoroutine);
                }

                _deactivateDoorCoroutine = StartCoroutine(DeactivateAfterDelay());
            }
        }
        private IEnumerator DeactivateAfterDelay()
        {
            yield return new WaitForSeconds(_duration);
            _connectedTriggerable.Deactivate();
        }


        public int GetSecurityLevel() => _securityLevel;
        public bool GetIsUnlocked() => _isUnlocked;



        #region Saving Functions

        public void BindExisting(ObjectSaveData saveData)
        {
            this._saveData = new KeycardReaderSaveInformation(saveData);
            _saveData.ID = ID;

            if (_saveData.WasDestroyed)
            {
                Destroy(this.gameObject);
            }

            this._isUnlocked = this._saveData.IsUnlocked;
        }
        public ObjectSaveData BindNew()
        {
            if (this._saveData == null || !this._saveData.Exists)
            {
                this._saveData = new KeycardReaderSaveInformation(this.ID, this._isUnlocked);
            }

            return this._saveData.ObjectSaveData;
        }
        private void LateUpdate()
        {
            // Transfer to where we are changing the value of '_isUnlocked'?
            this._saveData.IsUnlocked = _isUnlocked;
        }
        private void OnDestroy() => _saveData.WasDestroyed = true;

        #endregion




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

            if (_renderer != null && !Application.isPlaying)
            {
                // Setup the MaterialPropertyBlock.
                MaterialPropertyBlock materialPropertyBlock = new MaterialPropertyBlock();
                materialPropertyBlock.SetInteger(SECURITY_LEVEL_IDENTIFIER, _securityLevel);
                materialPropertyBlock.SetInteger(IS_UNLOCKED_IDENTIFIER, _isUnlocked ? 1 : 0);

                // Setup the Keycard Screen.
                _renderer.SetPropertyBlock(materialPropertyBlock);
            }
        }
    #endif
    }
}