using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace GPW.Tests.AI.Stalking
{
    public class HidingSpot : MonoBehaviour
    {
        [SerializeField] private float _angleOfVisibility;

        public bool IsOccupied { get; private set; } = false;

        public bool IsVisible { get; private set; } = false;
        public bool IsStale { get; private set; } = false; // A hiding point is stale if the player is too far from it.
        public float SqrDistanceToPlayer { get; private set; } = 0.0f;

        public bool IsValid => !IsOccupied && !IsVisible;



        public event System.Action OnSpotInvalidated;
        public event System.Action OnSpotBecameStale;


        public void UpdateHidingSpot(Vector3 playerPosition, float maxDistance)
        {
            bool wasVisible = IsVisible;
            bool wasStale = IsStale;
            
            // Calculate the squared distance to the player.
            Vector3 nodeToPlayerVector = playerPosition - transform.position;
            SqrDistanceToPlayer = GetSquareDistanceFromVector(nodeToPlayerVector);


            // Determine if this spot is now stale.
            IsStale = SqrDistanceToPlayer > (maxDistance * maxDistance);
            
            // Determine the angle to the player is within this spot's angle of visibility.
            IsVisible = Vector3.Angle(transform.forward, nodeToPlayerVector.normalized) <= (_angleOfVisibility / 2.0f);


            // If we have changed validity, then notify listeners.
            if (wasVisible != IsVisible && IsVisible)
            {
                OnSpotInvalidated?.Invoke();
            }
            else if (wasStale != IsStale && IsStale)
            {
                OnSpotBecameStale?.Invoke();
            }
            
            float GetSquareDistanceFromVector(Vector3 v) => (v.x * v.x) + (v.y * v.y) + (v.z * v.z);
        }


        public void EnterNode()
        {
            IsOccupied = true;
            IsStale = false;
        }
        public void ExitNode() => IsOccupied = false;


        private void OnDrawGizmosSelected() => DrawGizmos();
        public void DrawGizmos()
        {
            // Green if valid. Yellow if stale. Red if in sight.
            Gizmos.color = IsVisible ? Color.red : (IsStale ? Color.yellow : Color.green);


            // Draw position information.
            Gizmos.DrawSphere(transform.position, 0.1f);

            // Draw visibility information.
            Vector3 leftRayDirection = Quaternion.AngleAxis(-(_angleOfVisibility / 2.0f), transform.up) * transform.forward;
            Vector3 rightRayDirection = Quaternion.AngleAxis(_angleOfVisibility / 2.0f, transform.up) * transform.forward;
            Gizmos.DrawRay(transform.position, transform.forward);
            Gizmos.DrawRay(transform.position, leftRayDirection);
            Gizmos.DrawRay(transform.position, rightRayDirection);
        }
    }
}