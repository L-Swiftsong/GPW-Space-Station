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
        [SerializeField] protected ObjectSaveData SaveData;

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
            this.SaveData = saveData;
            SaveData.ID = ID;

            ISaveableObject.PerformBindingChecks(this.SaveData, this, () =>
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
            if (this.SaveData == null || !this.SaveData.Exists)
            {
                this.SaveData = new ObjectSaveData()
                {
                    ID = this.ID,
                    Exists = true,
                    DisabledState = ISaveableObject.DetermineDisabledState(this),
                };
            }

            ISaveableObject.UpdatePositionAndRotationInformation(this.SaveData, this);

            return this.SaveData;
        }

        protected virtual void OnEnable() => ISaveableObject.DefaultOnEnableSetting(this.SaveData, this);
        protected virtual void OnDestroy() => SaveData.DisabledState = DisabledState.Destroyed;
        protected virtual void OnDisable() => ISaveableObject.DefaultOnDisableSetting(this.SaveData, this);

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
