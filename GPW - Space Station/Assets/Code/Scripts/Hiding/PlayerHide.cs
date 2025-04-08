using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Entities.Player;
using Interaction;
using UnityEngine.InputSystem.XR;

namespace Hiding
{
    public class PlayerHide : MonoBehaviour
    {
        [Header("References")]
        private CharacterController _controller;
        private PlayerController _playerController;
        private PlayerInteraction _playerInteraction;
        private Camera _playerCamera;

        private HidingSpot _currentHidingSpot;


        [Header("Settings")]
        [SerializeField] private float _minimumExitAngle = 45.0f;


        private Coroutine _hidingCoroutine;
        public bool isHiding = false;
        public bool isTransitioning = false;


        private void Start()
        {
            _controller = GetComponent<CharacterController>();
            _playerController = GetComponent<PlayerController>();
            _playerInteraction = GetComponent<PlayerInteraction>();
            _playerCamera = Camera.main;

            _currentHidingSpot = null;
        }
        private void OnEnable() => HidingSpot.OnAnyHidingSpotInteracted += HidingSpot_OnAnyHidingSpotInteracted;
        private void OnDisable() => HidingSpot.OnAnyHidingSpotInteracted -= HidingSpot_OnAnyHidingSpotInteracted;

        private void HidingSpot_OnAnyHidingSpotInteracted(HidingSpot hidingSpot)
        {
            if (_currentHidingSpot == hidingSpot)
            {
                // Exit the hiding spot.
                StopHiding();
            }
            else
            {
                // Enter the hiding spot.
                StartHiding(hidingSpot);
            }
        }

        private void StartHiding(HidingSpot hidingSpot)
        {
            // Stop the current hiding coroutine.
            if (_hidingCoroutine != null)
            {
                StopCoroutine(_hidingCoroutine);
            }

            _currentHidingSpot = hidingSpot;

            // Override the player's currently selected interactable so that when they next interact they'll stop hiding.
            PlayerInteraction.SetCurrentInteractableOverride(_currentHidingSpot);

            // Start hiding.
            _hidingCoroutine = StartCoroutine(HideCoroutine(_currentHidingSpot));
        }
        private void StopHiding()
        {
            if (!_currentHidingSpot.TryGetExitPosition(_playerCamera.transform.forward, _minimumExitAngle, out Vector3 exitPosition))
            {
                // We aren't looking close enough to an exit spot to exit.
                return;
            }


            // Stop the current hiding coroutine.
            if (_hidingCoroutine != null)
            {
                StopCoroutine(_hidingCoroutine);
            }

            // Now that we're exiting the hiding spot, stop overriding the player's current interactable so that they can interact again.
            PlayerInteraction.ResetCurrentInteractableOverride();

            // Add our skin width as a vertical offset to prevent accidentally clipping with the ground and falling through.
            exitPosition += Vector3.up * _controller.skinWidth;

            // Stop hiding.
            _hidingCoroutine = StartCoroutine(StopHidingCoroutine(_currentHidingSpot, exitPosition));
        }


        private IEnumerator HideCoroutine(HidingSpot hidingSpot)
        {
            isTransitioning = true;
            _playerController.SetHiding(true);


            Vector3 startPosition = transform.position;
            Vector3 targetPosition = hidingSpot.HidingPosition;

            float elapsedTime = 0.0f;
            while (elapsedTime < 1.0f)
            {
                // Position Change.
                transform.position = Vector3.Lerp(startPosition, targetPosition, hidingSpot.PositionChangeCurve.Evaluate(elapsedTime));

                // Height Change.
                _controller.height = Mathf.Lerp(_playerController.GetDefaultHeight(), hidingSpot.HidingHeight, hidingSpot.HeightChangeCurve.Evaluate(elapsedTime));
                _controller.center = new Vector3(0.0f, _controller.height / 2.0f, 0.0f);

                // Camera height change.
                _playerCamera.transform.localPosition = new Vector3(
                    _playerCamera.transform.localPosition.x,
                    Mathf.Lerp(_playerController.GetDefaultCameraHeight(), hidingSpot.CameraHeight, hidingSpot.HeightChangeCurve.Evaluate(elapsedTime)),
                    _playerCamera.transform.localPosition.z);

                yield return null;
                elapsedTime += Time.deltaTime / hidingSpot.HidingTime;
            }

            transform.position = targetPosition;
            _controller.height = hidingSpot.HidingHeight;
            _controller.center = new Vector3(0.0f, _controller.height / 2.0f, 0.0f);
            _playerCamera.transform.localPosition = new Vector3(_playerCamera.transform.localPosition.x, hidingSpot.CameraHeight, _playerCamera.transform.localPosition.z);

            isHiding = true;
            isTransitioning = false;
            Debug.Log("Hiding under the table.");
        }

        private IEnumerator StopHidingCoroutine(HidingSpot hidingSpot, Vector3 exitPosition)
        {
            isTransitioning = false;


            Vector3 hidingPosition = hidingSpot.HidingPosition;
            float hidingHeight = hidingSpot.HidingHeight;
            float hidingCameraHeight = hidingSpot.CameraHeight;

            float elapsedTime = 1.0f;
            while (elapsedTime > 0.0f)
            {
                // Position Change.
                transform.position = Vector3.Lerp(exitPosition, hidingPosition, hidingSpot.PositionChangeCurve.Evaluate(elapsedTime));

                // Height Change.
                _controller.height = Mathf.Lerp(_playerController.GetDefaultHeight(), hidingHeight, hidingSpot.HeightChangeCurve.Evaluate(elapsedTime));
                _controller.center = new Vector3(0.0f, _controller.height / 2.0f, 0.0f);

                // Camera height change.
                _playerCamera.transform.localPosition = new Vector3(
                    _playerCamera.transform.localPosition.x,
                    Mathf.Lerp(_playerController.GetDefaultCameraHeight(), hidingCameraHeight, hidingSpot.HeightChangeCurve.Evaluate(elapsedTime)),
                    _playerCamera.transform.localPosition.z);

                yield return null;
                elapsedTime -= Time.deltaTime / hidingSpot.HidingTime;
            }

            transform.position = exitPosition;
            _controller.height = _playerController.GetDefaultHeight();
            _controller.center = new Vector3(0.0f, _controller.height / 2.0f, 0.0f);
            _playerCamera.transform.localPosition = new Vector3(_playerCamera.transform.localPosition.x, _playerController.GetDefaultCameraHeight(), _playerCamera.transform.localPosition.z);

            isTransitioning = false;

            isHiding = false;
            _playerController.SetHiding(false);

            _currentHidingSpot = null;
            Debug.Log("Stopped Hiding");
        }
    }
}