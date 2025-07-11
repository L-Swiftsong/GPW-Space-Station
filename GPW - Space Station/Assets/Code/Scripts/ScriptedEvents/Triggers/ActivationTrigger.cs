using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ScriptedEvents.Triggers
{
    public class ActivationTrigger : ScriptedEventTrigger
    {
        [Header("Activation Trigger")]
        [SerializeField] private bool _startInactive = true;


        private void Awake()
        {
            if (_startInactive)
                this.gameObject.SetActive(false);
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            if(!SaveData.DisabledState.HasFlag(Saving.LevelData.DisabledState.Destroyed))
            {
                // This trigger shouldn't have been destroyed via saving, so we're good.
                // Activate the trigger.
                ActivateTrigger();
            }
        }
    }
}
