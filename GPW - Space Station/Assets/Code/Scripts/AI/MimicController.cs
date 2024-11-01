using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


namespace AI.Mimic
{
    [RequireComponent(typeof(NavMeshAgent), typeof(EntitySenses))]
    public class MimicController : MonoBehaviour
    {
        private NavMeshAgent _agent;
        private EntitySenses _entitySenses;


        private void Awake()
        {
            _agent = GetComponent<NavMeshAgent>();
            _entitySenses = GetComponent<EntitySenses>();
        }


        private void Update()
        {
            if (_entitySenses.HasTarget)
            {
                // We can see the player.
                _agent.SetDestination(_entitySenses.TargetPosition);
            }
            else if (_entitySenses.CurrentPointOfInterest.HasValue)
            {
                // We cannot see the player but have heard something.
                _agent.SetDestination(_entitySenses.CurrentPointOfInterest.Value);

                if (_agent.remainingDistance < 0.5f)
                {
                    // Don't keep pathing to the same POI if we've discovered it.
                    _entitySenses.ClearPointOfInterest();
                }
            }
            else
            {
                // We cannot see the player and don't currently have a set POI.
                
            }
        }
    }
}