using Entities.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ScriptedEvents.Triggers
{
    public class VisibilityTrigger : ScriptedEventTrigger
    {
        [Header("VisibilityTrigger Settings")]
        [SerializeField] [Range(0.0f, 1.0f)] private float _horizontalScreenActivationBounds = 0.75f;
        [SerializeField] [Range(0.0f, 1.0f)] private float _verticalScreenActivationBounds = 0.75f;

        [Space(5)]
        [SerializeField] private float _maxActivationDistance = 10.0f;

        [Space(5)]
        [SerializeField] private bool _onlyActivateIfUnobstructed = true;
        [SerializeField] private LayerMask _obstructionLayers = 1 << 6 | 1 << 7; // Defaults: Wall + Ground.


        // References.
        private Camera _playerCamera;


        private void Awake()
        {
            _playerCamera = PlayerManager.Instance.GetPlayerCamera();
        }

        private void Update()
        {
            if (IsInPlayerView())
            {
                ActivateTrigger();
            }
        }


        private bool IsInPlayerView()
        {
            if (Vector3.Distance(transform.position, _playerCamera.transform.position) >= _maxActivationDistance)
            {
                // Outwith our maximum distance.
                return false;
            }

            if (_onlyActivateIfUnobstructed && Physics.Linecast(transform.position, _playerCamera.transform.position, _obstructionLayers))
            {
                // There is an obstruction between this and the player's camera.
                return false;
            }

            if (Vector3.Dot(_playerCamera.transform.forward, transform.position - _playerCamera.transform.position) <= 0.0f)
            {
                // Outwith our camera's view by over 90'.
                return false;
            }

            Vector2 screenCoordinates = _playerCamera.WorldToScreenPoint(transform.position);
            float horizontalPercentageFromCentre = Mathf.Abs((screenCoordinates.x / _playerCamera.pixelWidth) - 0.5f) * 2.0f;
            float verticalPercentageFromCentre = Mathf.Abs((screenCoordinates.y / _playerCamera.pixelHeight) - 0.5f) * 2.0f;

            if (horizontalPercentageFromCentre > _horizontalScreenActivationBounds)
            {
                // Outwith our horizontal activation distance
                return false;
            }
            if (verticalPercentageFromCentre > _verticalScreenActivationBounds)
            {
                // Outwith our vertical activation distance.
                return false;
            }

            // Within player view.
            return true;
        }
    }
}
