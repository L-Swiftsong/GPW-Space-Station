using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Effects.Mimicry.PassiveMimicry;

namespace Entities.Mimic.States
{
    public class PreparingToChaseState : State
    {
        public override string Name => "Set Trap";


        [Header("References")]
        [SerializeField] private EntityMovement _entityMovement;
        [SerializeField] private PassiveMimicryController _passiveMimicryController;


        [Header("Settings")]
        [SerializeField] private float _chaseStartTime = 0.75f;
        private float _chaseStartTimeRemaining;

        public bool CanStartChase() => _chaseStartTimeRemaining <= 0.0f;


        public override void OnEnter()
        {
            _chaseStartTimeRemaining = _chaseStartTime;
            _entityMovement.SetIsStopped(true);

            // Set mimicry strength.
            _passiveMimicryController.SetMimicryStrengthTarget(0.0f);
        }
        public override void OnLogic()
        {
            _chaseStartTimeRemaining -= Time.deltaTime;
        }
        public override void OnExit()
        {
            _entityMovement.SetIsStopped(false);
        }
    }
}