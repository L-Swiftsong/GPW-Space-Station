using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Mimicry.PassiveMimicry;

namespace AI.States
{
    public class PreparingToChaseState : State
    {
        public override string Name => "Set Trap";


        [Header("References")]
        [SerializeField] private NavMeshAgent _agent;
        [SerializeField] private PassiveMimicryController _passiveMimicryController;


        [Header("Settings")]
        [SerializeField] private float _chaseStartTime = 0.75f;
        private float _chaseStartTimeRemaining;

        public bool CanStartChase() => _chaseStartTimeRemaining <= 0.0f;


        public override void OnEnter()
        {
            _chaseStartTimeRemaining = _chaseStartTime;
            _agent.isStopped = true;

            // Set mimicry strength.
            _passiveMimicryController.SetMimicryStrengthTarget(0.0f);
        }
        public override void OnLogic()
        {
            _chaseStartTimeRemaining -= Time.deltaTime;
        }
        public override void OnExit()
        {
            _agent.isStopped = false;
        }
    }
}