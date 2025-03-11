using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Interaction;
using System;

namespace Hiding
{
    public class HidingSpot : MonoBehaviour, IInteractable
    {
        [SerializeField] private float _hidingTime = 0.75f;

        [Space(5)]
        [SerializeField] private Vector3 _hidingPosition = Vector3.zero;
        [SerializeField] private AnimationCurve _positionChangeCurve;

        [Space(5)]
        [SerializeField] private float _hidingHeight;
        [SerializeField] private float _cameraHeight;
        [SerializeField] private AnimationCurve _heightChangeCurve;


        [Header("Exiting")]
        [SerializeField] private List<Vector3> _exitSpots = new List<Vector3>() { new Vector3(0.0f, 0.0f, 1.0f) };

        [Space(5)]
        [SerializeField] private bool _automaticallyRemoveObstructedSpots = false;
        [SerializeField] private LayerMask _exitSpotObstructionLayers = 1 << 0 | 1 << 7;


        #region Properties

        public float HidingTime => _hidingTime;

        public Vector3 HidingPosition => transform.TransformPoint(_hidingPosition);
        public AnimationCurve PositionChangeCurve => _positionChangeCurve;

        public float HidingHeight => _hidingHeight;
        public float CameraHeight => _cameraHeight;
        public AnimationCurve HeightChangeCurve => _heightChangeCurve;

        #endregion


        public static event Action<HidingSpot> OnAnyHidingSpotInteracted;

        #region IInteractable Properties & Events

        private int _previousLayer;

        public event System.Action OnSuccessfulInteraction;
        public event System.Action OnFailedInteraction;

        #endregion


#if UNITY_EDITOR
        private void OnValidate()
        {
            if (_exitSpots == null || _exitSpots.Count == 0)
            {
                Debug.LogError($"Error: The HidingSpot instance '{this.name}' has an invalid number of exit spots (Valid Minimum Count = 1)");
            }
        }
#endif

        private void Awake()
        {
            if (_automaticallyRemoveObstructedSpots)
            {
                RemoveObstructedSpots();
            }
        }
        private void RemoveObstructedSpots()
        {
            for(int i = 0; i < _exitSpots.Count; ++i)
            {
                if (Physics.Linecast(transform.position, GetExitSpot(i)))
                {
                    // There is an obstruction leading to this spot.
                    Debug.Log($"Exit Spot {i} {_exitSpots[i]} was obstructed. Removing");
                    _exitSpots.RemoveAt(i);
                    --i;
                }
            }
        }

        

        public void Interact(PlayerInteraction interactingScript)
        {
            OnAnyHidingSpotInteracted?.Invoke(this);
            OnSuccessfulInteraction?.Invoke();
        }
        public void Highlight() => IInteractable.StartHighlight(this.gameObject, ref _previousLayer);
        public void StopHighlighting() => IInteractable.StopHighlight(this.gameObject, _previousLayer);


        private Vector3 GetExitSpot(int index) => transform.TransformPoint(_exitSpots[index]);
        public bool TryGetExitPosition(Vector3 forward, float minimuimUnhideAngle, out Vector3 exitSpot)
        {
            int bestExitIndex = -1;
            float currentBestAngle = minimuimUnhideAngle;
            for(int i = 0; i < _exitSpots.Count; ++i)
            {
                float angle = Vector3.Angle((GetExitSpot(i) - HidingPosition).normalized, forward);
                if (angle <= currentBestAngle)
                {
                    bestExitIndex = i;
                    currentBestAngle = angle;
                }
            }

            if (bestExitIndex == -1)
            {
                exitSpot = Vector3.zero;
                return false;
            }

            exitSpot = GetExitSpot(bestExitIndex);
            return true;
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(HidingPosition + (Vector3.up * _hidingHeight / 2.0f), new Vector3(0.5f, _hidingHeight, 0.5f));

            Gizmos.color = Color.white;
            Gizmos.DrawSphere(HidingPosition + Vector3.up * _cameraHeight, 0.1f);

            for(int i = 0; i < _exitSpots.Count; ++i)
            {
                // Make the gizmo red if we are removing spots and the spot is obstructed so that we can see what spots will be removed. Otherwise, make it green.
                RaycastHit hitInfo = new RaycastHit();
                Gizmos.color = _automaticallyRemoveObstructedSpots && Physics.Linecast(transform.position, GetExitSpot(i), _exitSpotObstructionLayers, QueryTriggerInteraction.Ignore) ? Color.red : Color.green;
                if (_automaticallyRemoveObstructedSpots && hitInfo.transform != null)
                    Debug.Log(hitInfo.transform.name);
                Gizmos.DrawLine(transform.position, GetExitSpot(i));
                Gizmos.DrawWireCube(GetExitSpot(i), Vector3.one * 0.5f);
            }
        }
    }
}