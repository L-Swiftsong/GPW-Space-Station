using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Entities.Mimic.States
{
    public class WanderState : State
    {
        public override string Name => "Wander";


        [Header("References")]
        [SerializeField] private EntityMovement _entityMovement;


        [Header("Wander Settings")]
        [SerializeField] private Vector3 _mapCentre;
        [SerializeField] private Vector3 _mapExtents;

        [Space(5)]
        [SerializeField] private NavMeshLayers _validWanderTargetLayers = NavMeshLayers.Walkable;


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
            ChooseNewDestination();
        }
        public override void OnLogic()
        {
            if (_entityMovement.HasReachedDestination())
            {
                // We've reached our desired wander destination.
                ChooseNewDestination();
            }

            _wanderDecisionTimeRemaining -= Time.deltaTime;
        }


        private void ChooseNewDestination()
        {
            if (_entityMovement.TryFindRandomPointInBounds(_mapCentre, _mapExtents, out Vector3 result, (int)_validWanderTargetLayers))
            {
                _entityMovement.SetDestination(result);
            }
        }


        private void OnDrawGizmosSelected()
        {
            Gizmos.DrawWireCube(_mapCentre, _mapExtents);
        }
    }
}