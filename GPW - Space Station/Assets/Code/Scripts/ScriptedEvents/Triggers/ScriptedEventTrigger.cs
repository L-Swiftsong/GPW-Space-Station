using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UltEvents;

namespace ScriptedEvents.Triggers
{
    public abstract class ScriptedEventTrigger : MonoBehaviour
    {
        [Header("Trigger Settings")]
        [SerializeField] private bool _onlyTriggerOnce = true;

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



        protected virtual void ActivateTrigger()
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

                _activateTriggerAfterDelayCoroutine = StartCoroutine(ActivateTriggerAfterDelay());
            }
            else if (_onlyTriggerOnce)
            {
                Destroy(this.gameObject);
            }
        }
        private IEnumerator ActivateTriggerAfterDelay()
        {
            yield return new WaitForSeconds(_delayedTriggerDelayTime);
            OnDelayedTriggerActivated?.Invoke();

            if (_onlyTriggerOnce)
            {
                Destroy(this.gameObject);
            }
        }



#region Call Presets
#if UNITY_EDITOR
        [ContextMenu("Presets/Immediate/Add Chase End")]
        private void ImmediatePreset_TriggerChaseEnd() => OnTriggerActivated.AddPersistentCall((System.Action)Entities.Mimic.ChaseMimic.EndChase);
        
        [ContextMenu("Presets/Delayed/Add Chase End")]
        private void DelayedPreset_TriggerChaseEnd() => OnDelayedTriggerActivated.AddPersistentCall((System.Action)Entities.Mimic.ChaseMimic.EndChase);
#endif
#endregion
    }
}
