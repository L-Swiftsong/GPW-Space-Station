using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Effects.Mimicry.PassiveMimicry;

namespace Entities.Mimic.States
{
    public class StunnedState : State
    {
        public override string Name => "Stunned";


        [Header("Referenecs")]
        [SerializeField] private NavMeshAgent _agent;
        [SerializeField] private PassiveMimicryController _passiveMimicryController;


        public override void OnEnter()
        {
            _agent.isStopped = true;
            _passiveMimicryController.SetMimicryStrengthTarget(0.0f);
        }
        public override void OnExit()
        {
            _agent.isStopped = false;
            _passiveMimicryController.SetMimicryStrengthTarget(1.0f);
        }
    }
}
