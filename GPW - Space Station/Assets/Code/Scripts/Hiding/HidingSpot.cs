using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Interaction;

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
        [SerializeField] private List<Vector3> _exitSpots;


        #region Properties

        public float HidingTime => _hidingTime;

        public Vector3 HidingPosition => transform.TransformPoint(_hidingPosition);
        public AnimationCurve PositionChangeCurve => _positionChangeCurve;

        public float HidingHeight => _hidingHeight;
        public float CameraHeight => _cameraHeight;
        public AnimationCurve HeightChangeCurve => _heightChangeCurve;

        #endregion


        public static event System.Action<HidingSpot> OnAnyHidingSpotInteracted;


        public void Interact(PlayerInteraction interactingScript)
        {
            OnAnyHidingSpotInteracted?.Invoke(this);
        }
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

            Gizmos.color = Color.red;
            for(int i = 0; i < _exitSpots.Count; ++i)
            {
                Gizmos.DrawWireCube(GetExitSpot(i), Vector3.one * 0.5f);
            }
        }
    }
}