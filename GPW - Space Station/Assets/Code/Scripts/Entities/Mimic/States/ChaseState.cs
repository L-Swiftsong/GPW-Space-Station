using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Effects.Mimicry.PassiveMimicry;

namespace Entities.Mimic.States
{
    public class ChaseState : State
    {
        public override string Name => "Chase";


        [Header("References")]
        [SerializeField] private EntityMovement _entityMovement;
        [SerializeField] private EntitySenses _entitySenses;
        [SerializeField] private MimicAttack _attackScript;
        [SerializeField] private PassiveMimicryController _passiveMimicryController;


        [Header("Settings")]
        [SerializeField] private float _chaseMovementSpeed = 4.0f;
        [SerializeField] private float _chaseAcceleration = 8.0f;
        [SerializeField] private float _chaseAngularSpeed = 720.0f;

        [Space(5)]
        [SerializeField] private float _chaseStoppingDistance = 1.0f;


        public override void OnEnter()
        {
            // Override movement values with chase-specific values.
            _entityMovement.SetSpeedOverride(_chaseMovementSpeed);
            _entityMovement.SetAccelerationOverride(_chaseAcceleration);
            _entityMovement.SetAngularSpeedOverride(_chaseAngularSpeed);
            _entityMovement.SetStoppingDistanceOverride(_chaseStoppingDistance);

            _attackScript.SetCanAttack(true);
        }
        public override void OnLogic()
        {
            if (!_entitySenses.HasTarget)
            {
                // The player is no longer in our LOS.
                return;
            }
            // The player is still in our LOS.

            // Move towards the player.
            _entityMovement.SetDestination(_entitySenses.TargetPosition);
        }
        public override void OnExit()
        {
            // Reset agent movement values.
            _entityMovement.ResetAllOverrides();

            _attackScript.SetCanAttack(true);

            // Reset mimicry strength.
            _passiveMimicryController.SetMimicryStrengthTarget(1.0f);
        }
    }
}