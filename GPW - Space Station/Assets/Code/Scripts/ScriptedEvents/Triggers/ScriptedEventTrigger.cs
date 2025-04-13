using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UltEvents;
using Saving.LevelData;

namespace ScriptedEvents.Triggers
{
    public abstract class ScriptedEventTrigger : MonoBehaviour, ISaveableObject
    {
        #region Saving Variables & References

        [field: SerializeField] public SerializableGuid ID { get; set; }
        [SerializeField] private ObjectSaveData _saveData;

        #endregion


        [Header("Trigger Settings")]
        [SerializeField] private bool _onlyTriggerOnce = true;
        [SerializeField] private bool _destroyObjectOnTrigger = false;

        [Space(10)]
        public UltEvent OnTriggerActivated;
        
        
        [Header("Delay")]
        [SerializeField] private bool _useDelayedTrigger = false;

        [Space(5)]
        [SerializeField] private bool _canTriggerWhileAwaitingDelay = false;
        [SerializeField] private bool _overridePreviousDelay = true;

        [Space(5)]
        [SerializeField] private float _delayedTriggerDelayTime = 1.0f;
        private Coroutine _activateTriggerAfterDelayCoroutine;

        [Space(10)]
        public UltEvent OnDelayedTriggerActivated;


        protected void ActivateTrigger() => ActivateTrigger(false);
        protected void ActivateTrigger(bool forceDestruction)
        {
            if (!_canTriggerWhileAwaitingDelay && _activateTriggerAfterDelayCoroutine != null)
            {
                // We are currently waiting for the delay to elapse, and cannot trigger again until it has done so.
                return;
            }
            else if (_onlyTriggerOnce && _activateTriggerAfterDelayCoroutine != null)
            {
                // We are wanting to only activate this trigger once, and given that we are currently awaiting a delay to elapse we have already triggered it.
                return;
            }

            Debug.Log("Activate Trigger", this);
            OnTriggerActivated?.Invoke();


            if (_useDelayedTrigger)
            {
                if (_overridePreviousDelay && _activateTriggerAfterDelayCoroutine != null)
                    StopCoroutine(_activateTriggerAfterDelayCoroutine);

                _activateTriggerAfterDelayCoroutine = StartCoroutine(ActivateTriggerAfterDelay(forceDestruction));
            }
            else if (_onlyTriggerOnce)
            {
                if (_destroyObjectOnTrigger)
                    Destroy(this.gameObject);
                else
                    Destroy(this);
            }
        }
        private IEnumerator ActivateTriggerAfterDelay(bool forceDestruction)
        {
            yield return new WaitForSeconds(_delayedTriggerDelayTime);
            OnDelayedTriggerActivated?.Invoke();

            if (_onlyTriggerOnce || forceDestruction)
            {
                if (_destroyObjectOnTrigger)
                    Destroy(this.gameObject);
                else
                    Destroy(this);
            }
        }


        #region Saving Functions

        public void BindExisting(ObjectSaveData saveData)
        {
            this._saveData = saveData;
            _saveData.ID = ID;

            ISaveableObject.PerformBindingChecks(this._saveData, this, () =>
            {
                if (_destroyObjectOnTrigger)
                {
                    Destroy(this.gameObject);
                }
                else
                {
                    Destroy(this);
                }
            });
        }
        public ObjectSaveData BindNew()
        {
            if (this._saveData == null || !this._saveData.Exists)
            {
                this._saveData = new ObjectSaveData()
                {
                    ID = this.ID,
                    Exists = true
                };
            }

            ISaveableObject.UpdatePositionAndRotationInformation(this._saveData, this);

            return this._saveData;
        }

        protected virtual void OnEnable() => ISaveableObject.DefaultOnEnableSetting(this._saveData, this);
        protected virtual void OnDestroy() => _saveData.DisabledState = DisabledState.Destroyed;
        protected virtual void OnDisable() => ISaveableObject.DefaultOnDisableSetting(this._saveData, this);

        #endregion



        #region Call Presets
#if UNITY_EDITOR
        [ContextMenu("Presets/Immediate/Add Chase End")]
        private void ImmediatePreset_TriggerChaseEnd() => OnTriggerActivated.AddPersistentCall((System.Action)Entities.Mimic.ChaseMimic.EndAllChases);
        
        [ContextMenu("Presets/Delayed/Add Chase End")]
        private void DelayedPreset_TriggerChaseEnd() => OnDelayedTriggerActivated.AddPersistentCall((System.Action)Entities.Mimic.ChaseMimic.EndAllChases);
#endif
#endregion
    }
}
