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

        [field: SerializeField] public SerializableGuid ID { get; set; } = SerializableGuid.NewGuid();
        [SerializeField] private ObjectSaveData _saveData;
        [SerializeField] private bool _triggerEventIfDestroyedOnLoad = false;

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


        protected virtual void ActivateTrigger(bool forceDestruction = false)
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

            Debug.Log("Activate Trigger");
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

            if (_saveData.WasDestroyed)
            {
                if (_triggerEventIfDestroyedOnLoad)
                {
                    ActivateTrigger(forceDestruction: true);
                }
                else
                {
                    Destroy(this.gameObject);
                }
            }
        }
        public ObjectSaveData BindNew()
        {
            if (this._saveData == null || !this._saveData.Exists)
            {
                this._saveData = new ObjectSaveData()
                {
                    ID = this.ID,
                    Exists = true,
                    WasDestroyed = false
                };
            }

            return this._saveData;
        }
        private void OnDestroy() => _saveData.WasDestroyed = true;

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
