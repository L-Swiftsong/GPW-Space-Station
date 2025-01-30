using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Entities.Mimic.States
{
    public class SearchState : State
    {
        public override string Name => "Searching";


        [Header("References")]
        [SerializeField] private EntityMovement _entityMovement;
        [SerializeField] private EntitySenses _entitySenses;


        public override void OnEnter()
        {
            _entityMovement.SetDestination(_entitySenses.CurrentPointOfInterest.Value);
        }
        public override void OnLogic()
        {
            if (!_entityMovement.HasReachedDestination())
            {
                // Update our destination to the POI.
                _entityMovement.SetDestination(_entitySenses.CurrentPointOfInterest.Value);
            }
            else
            {
                // We have reached the POI.
                _entitySenses.ClearPointOfInterest();
            }
        }
    }
}