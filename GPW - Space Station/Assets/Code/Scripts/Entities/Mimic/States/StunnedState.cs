using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Effects.Mimicry.PassiveMimicry;

namespace Entities.Mimic.States
{
    public class StunnedState : State
    {
        public override string Name => "Stunned";


        [Header("Referenecs")]
        [SerializeField] private EntityMovement _entityMovement;
        [SerializeField] private PassiveMimicryController _passiveMimicryController;


        public override void OnEnter()
        {
            _entityMovement.SetIsStopped(true);
            _passiveMimicryController.SetMimicryStrengthTarget(0.0f);
        }
        public override void OnExit()
        {
            _entityMovement.SetIsStopped(false);
            _passiveMimicryController.SetMimicryStrengthTarget(1.0f);
        }
    }
}
