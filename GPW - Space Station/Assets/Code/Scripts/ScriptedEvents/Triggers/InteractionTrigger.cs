using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Interaction;

namespace ScriptedEvents.Triggers
{
    [RequireComponent(typeof(IInteractable))]
    public class InteractionTrigger : ScriptedEventTrigger
    {
        [Header("Interaction Trigger Settings")]
        [SerializeField] private bool _triggerOnSuccess = true;
        [SerializeField] private bool _triggerOnFailure = false;


        private void Awake()
        {
            foreach(IInteractable interactable in GetComponents<IInteractable>())
            {
                if (_triggerOnSuccess)
                    interactable.OnSuccessfulInteraction += ActivateTrigger;
                if (_triggerOnFailure)
                    interactable.OnFailedInteraction += ActivateTrigger;
            }
        }
        private void OnDestroy()
        {
            foreach (IInteractable interactable in GetComponents<IInteractable>())
            {
                if (_triggerOnSuccess)
                    interactable.OnSuccessfulInteraction += ActivateTrigger;
                if (_triggerOnFailure)
                    interactable.OnFailedInteraction += ActivateTrigger;
            }
        }
    }
}
