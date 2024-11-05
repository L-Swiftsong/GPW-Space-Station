using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace AI.States
{
    public class ChaseState : State
    {
        public override string Name => "Chase";


        [Header("References")]
        [SerializeField] private NavMeshAgent _agent;
        [SerializeField] private EntitySenses _entitySenses;
        [SerializeField] private PassiveMimicryController _passiveMimicryController;


        [Header("Settings")]
        [SerializeField] private float _chaseMovementSpeed;
        [SerializeField] private float _chaseAcceleration;
        private (float Speed, float Acceleration) _previousMovementValues;

        [Space(5)]
        [SerializeField] private float _playerCatchRadius = 0.5f;


        public override void OnEnter()
        {
            // Cache current movement values.
            _previousMovementValues = (_agent.speed, _agent.acceleration);

            // Override agent movement values with chase-specific values.
            _agent.speed = _chaseMovementSpeed;
            _agent.acceleration = _chaseAcceleration;
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
            _agent.SetDestination(_entitySenses.TargetPosition);

            if ((transform.position - _entitySenses.TargetPosition).sqrMagnitude <= (_playerCatchRadius * _playerCatchRadius))
            {
                // We are close enough to catch the player.
                Debug.Log("The player has been caught!");
                UI.GameOver.GameOverUI.Instance.ShowGameOverUI();
            }
        }
        public override void OnExit()
        {
            // Reset agent movement values.
            _agent.speed = _previousMovementValues.Speed;
            _agent.acceleration = _previousMovementValues.Acceleration;


            // Reset mimicry strength.
            _passiveMimicryController.SetMimicryStrengthTarget(1.0f);
        }


        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, _playerCatchRadius);
        }
    }
}