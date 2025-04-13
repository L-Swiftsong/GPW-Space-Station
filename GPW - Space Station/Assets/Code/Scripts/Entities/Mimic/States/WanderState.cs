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
        [SerializeField] private Transform _wanderBoundsReference;
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
            _entityMovement.SetSpeed(2f);
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
                Debug.Log(validWanderBounds[i].GetCentre(_wanderBoundsReference));
            }


            if (_entityMovement.TryFindRandomPointInBounds(validWanderBounds[randomIndex].GetCentre(_wanderBoundsReference), validWanderBounds[randomIndex].GetExtents(), out Vector3 result, (int)_validWanderTargetLayers))
            {
                _entityMovement.SetDestination(result);
            }
        }

        private void UpdateWanderBounds()
        {
            _wanderBoundsUpdateDelayRemaining = _wanderBoundsUpdateDelay;

            for (int i = 0; i < _wanderBounds.Length; ++i)
            {
                _wanderBounds[i].UpdateCanReach(_wanderBoundsReference, _entityMovement);
            }
        }


        private void OnDrawGizmosSelected()
        {
            if (_wanderBoundsReference == null)
                return;

            for(int i = 0; i < _wanderBounds.Length; ++i)
            {
                _wanderBounds[i].DrawGizmos(_wanderBoundsReference);
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

            public Vector3 GetCentre(Transform parent) => parent.TransformPoint(_centre);
            public Vector3 GetExtents() => _extents;
            public bool CanReach => _canReach;

            private Vector3 GetTestPosition(Transform parent) => parent.TransformPoint(_testPosition);

            public void UpdateCanReach(Transform parent, EntityMovement entityMovement)
            {
                Vector3 worldTestPosition = GetTestPosition(parent);
                if (entityMovement.CalculatePath(worldTestPosition, out UnityEngine.AI.NavMeshPath path))
                {
                    _canReach = (path.corners[path.corners.Length - 1] - worldTestPosition).sqrMagnitude < TRAVERSE_CHECK_SQR_RADIUS;
                }
                else
                {
                    _canReach = false;
                }
            }

            public void DrawGizmos(Transform parent)
            {
                Gizmos.color = _canReach ? Color.green : Color.red;
                Gizmos.DrawWireCube(GetCentre(parent), GetExtents());

                Gizmos.color = Color.black;
                Gizmos.DrawSphere(GetTestPosition(parent), 0.5f);
            }
        }
    }
}