using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Entities.Mimic.States
{
    public class WanderState : State
    {
        public override string Name => "Wander";


        [Header("References")]
        [SerializeField] private EntityMovement _entityMovement;


        [Header("Wander Settings")]
        [SerializeField] private WanderBounds[] _wanderBounds;

        [Space(5)]
        [SerializeField] private NavMeshLayers _validWanderTargetLayers = NavMeshLayers.Walkable;

        [Space(5)]
        [SerializeField] private float _wanderBoundsUpdateDelay = 5.0f;
        private float _wanderBoundsUpdateDelayRemaining;


        [Header("Wander Decision Settings")]
        [SerializeField] private float _minWanderDecisionTime = 1.0f;
        [SerializeField] private float _maxWanderDecisionTime = 15.0f;
        private float _wanderDecisionTimeRemaining = 0.0f;

        [Space(5)]
        [SerializeField] private float _setTrapChance = 0.2f;

        public float SetTrapChance => _setTrapChance;


        public bool ShouldMakeNewDecision() => _wanderDecisionTimeRemaining <= 0.0f;
        public void AttemptingDecision() => _wanderDecisionTimeRemaining = 0.0f;


        public override void OnEnter()
        {
            _wanderDecisionTimeRemaining = Random.Range(_minWanderDecisionTime, _maxWanderDecisionTime);
            ChooseNewDestination();
            UpdateWanderBounds();
        }
        public override void OnLogic()
        {
            if (_wanderBoundsUpdateDelayRemaining > 0.0f)
            {
                _wanderBoundsUpdateDelayRemaining -= Time.deltaTime;
            }
            else
            {
                UpdateWanderBounds();
            }

            if (_entityMovement.HasReachedDestination())
            {
                // We've reached our desired wander destination.
                ChooseNewDestination();
            }

            _wanderDecisionTimeRemaining -= Time.deltaTime;
        }


        private void ChooseNewDestination()
        {
            WanderBounds[] validWanderBounds = _wanderBounds.Where(t => t.CanReach).ToArray();

            if (validWanderBounds.Length == 0)
            {
                // No wander bounds can be traversed to.
                Debug.Log("Failed to find valid bounds");
                return;
            }
            int randomIndex = Random.Range(0, validWanderBounds.Length);
            for (int i = 0; i < validWanderBounds.Length; ++i)
            {
                Debug.Log(validWanderBounds[i].Centre);
            }


            if (_entityMovement.TryFindRandomPointInBounds(validWanderBounds[randomIndex].Centre, validWanderBounds[randomIndex].Extents, out Vector3 result, (int)_validWanderTargetLayers))
            {
                _entityMovement.SetDestination(result);
            }
        }

        private void UpdateWanderBounds()
        {
            _wanderBoundsUpdateDelayRemaining = _wanderBoundsUpdateDelay;

            for (int i = 0; i < _wanderBounds.Length; ++i)
            {
                _wanderBounds[i].UpdateCanReach(_entityMovement);
            }
        }


        private void OnDrawGizmosSelected()
        {
            for(int i = 0; i < _wanderBounds.Length; ++i)
            {
                _wanderBounds[i].DrawGizmos();
            }
        }


        [System.Serializable]
        private struct WanderBounds
        {
            [SerializeField] private Vector3 _centre;
            [SerializeField] private Vector3 _extents;

            [Space(5)]
            [SerializeField] private Vector3 _testPosition;
            [SerializeField, ReadOnly] private bool _canReach;
            private const float TRAVERSE_CHECK_SQR_RADIUS = 2.0f * 2.0f;

            public Vector3 Centre => _centre;
            public Vector3 Extents => _extents;
            public bool CanReach => _canReach;


            public void UpdateCanReach(EntityMovement entityMovement)
            {
                if (entityMovement.CalculatePath(_testPosition, out UnityEngine.AI.NavMeshPath path))
                {
                    _canReach = (path.corners[path.corners.Length - 1] - _testPosition).sqrMagnitude < TRAVERSE_CHECK_SQR_RADIUS;
                }
                else
                {
                    _canReach = false;
                }
            }

            public void DrawGizmos()
            {
                Gizmos.color = _canReach ? Color.green : Color.red;
                Gizmos.DrawWireCube(_centre, _extents);

                Gizmos.color = Color.black;
                Gizmos.DrawSphere(_testPosition, 0.5f);
            }
        }
    }
}