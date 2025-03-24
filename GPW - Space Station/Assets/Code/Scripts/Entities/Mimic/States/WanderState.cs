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

        [Space(5)]
        [SerializeField] private int _maxWanderAttempts = 10;
        [SerializeField] private float _wanderPauseDelay = 3.0f;
        private int _currentWanderAttempts;
        private float _pauseDelayRemaining;


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
        }
        public override void OnLogic()
        {
            if (_pauseDelayRemaining > 0.0f)
            {
                _pauseDelayRemaining -= Time.deltaTime;

                if (_pauseDelayRemaining <= 0.0f)
                {
                    _entityMovement.SetIsStopped(false);
                    _currentWanderAttempts = 0;
                }
                else
                {
                    return;
                }
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
            if (_entityMovement.TryFindRandomPointInBounds(_mapCentre, _mapExtents, out Vector3 result, (int)_validWanderTargetLayers))
            {
                UnityEngine.AI.NavMeshPath path = new UnityEngine.AI.NavMeshPath();
                if (_entityMovement.CalculatePath(result, ref path) && path.status == UnityEngine.AI.NavMeshPathStatus.PathComplete)
                {
                    _entityMovement.SetPath(path);
                    _currentWanderAttempts = 0;
                }
                else
                {
                    _currentWanderAttempts++;
                    if (_currentWanderAttempts >= _maxWanderAttempts)
                    {
                        _entityMovement.SetIsStopped(true);
                        _pauseDelayRemaining = _wanderPauseDelay;
                    }
                }
            }
        }


        private void OnDrawGizmosSelected()
        {
            Gizmos.DrawWireCube(_mapCentre, _mapExtents);

            /*
            for(int i = 0; i < _wanderBounds.Length; ++i)
            {
                Gizmos.Color = _wanderBounds[i].CanReach ? Color.green : Color.red;
                Debug.DrawWireCube(_wanderBounds[i].Centre, _wanderBounds[i].Extents);
            }
             */
        }


        [System.Serializable]
        private struct WanderBounds
        {
            [SerializeField] private Vector3 _centre;
            [SerializeField] private Vector3 _extents;

            [SerializeField, ReadOnly] private bool _canReach;
            private const float TRAVERSE_CHECK_RADIUS = 2.0f;
            private const int TRAVERSE_CHECK_ATTEMPT_COUNT = 5;

            public Vector3 Centre => _centre;
            public Vector3 Extents => _extents;
            public bool CanReach => _canReach;


            public void UpdateCanReach(EntityMovement entityMovement, NavMeshLayers walkableLayers)
            {
                _canReach = entityMovement.TryFindRandomPointInBounds(_centre, _extents, out Vector3 result, (int)walkableLayers, TRAVERSE_CHECK_ATTEMPT_COUNT);
            }
        }
    }
}