using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace AI.States
{
    public class SearchState : State
    {
        public override string Name => "Searching";


        [Header("References")]
        [SerializeField] private NavMeshAgent _agent;
        [SerializeField] private EntitySenses _entitySenses;


        public override void OnEnter()
        {
            _agent.SetDestination(_entitySenses.CurrentPointOfInterest.Value);
        }
        public override void OnLogic()
        {
            if (_agent.remainingDistance > 0.5f)
            {
                // Update our destination to the POI.
                _agent.SetDestination(_entitySenses.CurrentPointOfInterest.Value);
            }
            else
            {
                // We have reached the POI.
                _entitySenses.ClearPointOfInterest();
            }
        }
    }
}