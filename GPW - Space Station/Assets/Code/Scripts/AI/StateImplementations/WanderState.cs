using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace AI.States
{
    public class WanderState : State
    {
        public override string Name => "Wander";


        [Header("References")]
        [SerializeField] private NavMeshAgent _agent;


        [Header("Wander Settings")]
        [SerializeField] private float _reachedDestinationDistance = 0.5f;

        [Space(5)]
        [SerializeField] private Vector3 _mapCentre;
        [SerializeField] private Vector3 _mapExtents;

        [Header("Wander Decision Settings")]
        [SerializeField] private float _minWanderDecisionTime = 1.0f;
        [SerializeField] private float _maxWanderDecisionTime = 15.0f;
        private float _wanderDecisionTimeRemaining = 0.0f;

        [Space(5)]
        [SerializeField] private float _enterVentChance = 0.2f;
        [SerializeField] private float _setTrapChance = 0.2f;

        public float VentChance => _enterVentChance;
        public float SetTrapChance => _setTrapChance;


        public bool ShouldMakeNewDecision() => _wanderDecisionTimeRemaining <= 0.0f;


        public override void OnEnter()
        {
            _wanderDecisionTimeRemaining = Random.Range(_minWanderDecisionTime, _maxWanderDecisionTime);
        }
        public override void OnLogic()
        {
            if (_agent.remainingDistance <= _reachedDestinationDistance)
            {
                // We've reached our desired wander destination.
                // Pick a new destination.
                if (_agent.TryFindRandomPoint(_mapCentre, _mapExtents, out Vector3 result))
                {
                    _agent.SetDestination(result);
                }
            }

            _wanderDecisionTimeRemaining -= Time.deltaTime;
        }


        private void OnDrawGizmosSelected()
        {
            Gizmos.DrawWireCube(_mapCentre, _mapExtents);
        }
    }
}