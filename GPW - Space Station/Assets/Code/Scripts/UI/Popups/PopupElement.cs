using Entities.Player;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Popups
{
    public class PopupElement : MonoBehaviour
    {
        [SerializeField] private Transform _pivotTransform;
        [SerializeField] private Transform _offsetTransform;


        [Space(5)]
        [SerializeField] private TMP_Text _preText;
        [SerializeField] private Image _image;
        [SerializeField] private TMP_Text _postText;


        [Header("Deactivation")]
        [SerializeField, ReadOnly] private float _maxLifetime;
        [SerializeField, ReadOnly] private float _currentLifetime;

        [SerializeField, ReadOnly] private float _maxPlayerDistance;
        [SerializeField, ReadOnly] private bool _disableIfObstructed;
        [SerializeField] private LayerMask _obstructionLayers;

        private Func<bool> _customDeactivationFunc;
        private Action _onDisableCallback;


        private void Update()
        {
            if (ShouldDeactivate())
            {
                Deactivate();
                return;
            }

            RotateToFacePlayerCamera();
        }


        private bool ShouldDeactivate()
        {
            if (_maxLifetime > 0)
            {
                _currentLifetime += Time.deltaTime;

                if (_currentLifetime > _maxLifetime)
                {
                    // Lifetime Elapsed.
                    return true;
                }
            }

            if (_maxPlayerDistance > 0)
            {
                if (Vector3.Distance(_offsetTransform.position, PlayerManager.Instance.GetPlayerCameraTransform().position) >= _maxPlayerDistance)
                {
                    // Outwith Max Distance.
                    return true;
                }
            }

            if (_disableIfObstructed)
            {
                if (Physics.Linecast(_offsetTransform.position, PlayerManager.Instance.GetPlayerCameraTransform().position, _obstructionLayers, QueryTriggerInteraction.Ignore))
                {
                    // Obstruction.
                    return true;
                }
            }

            if (_customDeactivationFunc())
            {
                return true;
            }


            return false;
        }

        private void Deactivate() => _onDisableCallback?.Invoke();


        private void RotateToFacePlayerCamera()
        {
            Transform playerCameraTransform = PlayerManager.Instance.GetPlayerCameraTransform();

            // Rotate Pivot around the Y-Axis.
            Vector3 pivotLookDirection = _pivotTransform.position - playerCameraTransform.position;
            float yDegrees = Mathf.Atan2(pivotLookDirection.x, pivotLookDirection.z) * Mathf.Rad2Deg;
            _pivotTransform.LookAt(new Vector3(playerCameraTransform.position.x, transform.position.y, playerCameraTransform.position.z));

            // Rotate Offset around the X-Axis.
            Vector3 offsetLookDirection = _offsetTransform.position - playerCameraTransform.position;
            float xDegrees = Mathf.Atan2(offsetLookDirection.x, offsetLookDirection.y) * Mathf.Rad2Deg;
            _offsetTransform.LookAt(playerCameraTransform.position);
            _offsetTransform.localRotation = Quaternion.Euler(-_offsetTransform.localRotation.eulerAngles.x, -180.0f, 0.0f);
        }


        public PopupElement SetPosition(Transform pivot, Vector3 offset, bool rotateInPlace)
        {
            if (rotateInPlace)
            {
                _pivotTransform.position = pivot.position + offset;
                _offsetTransform.localPosition = Vector3.zero;
            }
            else
            {
                _pivotTransform.position = pivot.position;
                _offsetTransform.localPosition = offset;
            }

            // Temp: Deactivate if the pivot is destroyed (Replace this with a custom deactivation func parameter in the SetDeactivationParameters() method).
            _customDeactivationFunc = () => pivot == null;

            return this;
        }
        public PopupElement SetInformation(string preText, Sprite sprite, string postText)
        {
            _preText.text = preText;
            _image.sprite = sprite;
            _postText.text = postText;

            return this;
        }
        public PopupElement SetDeactivationPerameters(Action onDisableCallback, float lifetime = -1, float distanceToPlayer = -1, bool disableIfObstructed = false)
        {
            // Callback.
            _onDisableCallback = onDisableCallback;

            // Deactivation Parameters.
            _maxLifetime = lifetime;
            _currentLifetime = 0;

            _maxPlayerDistance = distanceToPlayer;
            _disableIfObstructed = disableIfObstructed;


            // Fluent Interface.
            return this;
        }
    }
}
