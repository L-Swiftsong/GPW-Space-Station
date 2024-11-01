using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Environment.Doors
{
    public class DoorOpener : MonoBehaviour
    {
        private Door _door;
        private Coroutine _handleOpenStateChangeCoroutine;
        private bool _wasPreviouslyOpenedFromFacingDirection = false;

        [SerializeField] private DoorFrame[] _doorFrames;


        private void Awake()
        {
            // Assign door reference.
            if (!this.TryGetComponentThroughParents<Door>(out _door))
            {
                Debug.LogError("Error: Failed to get Door reference for the DoorOpener: " + this + ". Ensure that a parent object contains a 'Door' instance.");
            }


            // Subscribe to Door Events.
            _door.OnOpenStateChanged += Door_OnOpenStateChanged;
        }
        private void OnDestroy() => _door.OnOpenStateChanged -= Door_OnOpenStateChanged;
        


        private void Door_OnOpenStateChanged(bool isOpen)
        {
            if (_handleOpenStateChangeCoroutine != null)
            {
                StopCoroutine(_handleOpenStateChangeCoroutine);
            }

            _handleOpenStateChangeCoroutine = StartCoroutine(HandleOpenStateChange(isOpen));
        }

        private IEnumerator HandleOpenStateChange(bool isOpen)
        {
            bool allComplete = false;

            // If we are opening the door from the facing direction, or are closing it after doing so, alter the DoorFrames.
            bool openedFromFacingDirection = isOpen ? (_door is InteractableDoor) && (_door as InteractableDoor).WasOpenedFromFacingDirection : _wasPreviouslyOpenedFromFacingDirection;
            _wasPreviouslyOpenedFromFacingDirection = openedFromFacingDirection;

            while (!allComplete)
            {
                // Loop through each doorframe. If all have completed their transition, we'll exit at the start of the next frame.
                allComplete = true;
                for(int i = 0; i < _doorFrames.Length; i++)
                {
                    if (_doorFrames[i].HandleOpeningTick(isOpen, openedFromFacingDirection) == false)
                    {
                        allComplete = false;
                    }
                }

                yield return null;
            }
        }


        [System.Serializable]
        private class DoorFrame
        {
            [Header("References & Settings")]
            [SerializeField] private Transform _frameTransform = null;
            
            [SerializeField] private float _openingDuration = 0.5f;
            private float _elapsedTime = 0.0f;

            private Collider[] _colliders; // Lazily Initialised.

            
            [Header("Position Information")]
            [SerializeField] private bool _affectPosition = false;

            [Space(5)]
            [SerializeField] private Vector3 _closedPosition = Vector3.zero;
            [SerializeField] private Vector3 _openPosition = Vector3.zero;

            [Space(5)]
            [SerializeField] private AnimationCurve _positionCurve = AnimationCurve.Linear(0.0f, 0.0f, 1.0f, 1.0f);



            [Header("Rotation Information")]
            [SerializeField] private bool _affectRotation = false;
            [SerializeField] private bool _flipRotationWhenOpeningFromBehind = false;
            
            [Space(5)]
            [SerializeField] private Vector3 _closedRotation = Vector3.zero;
            [SerializeField] private Vector3 _openRotation = Vector3.zero;
            
            [Space(5)]
            [SerializeField] private AnimationCurve _rotationCurve = AnimationCurve.Linear(0.0f, 0.0f, 1.0f, 1.0f);



            private DoorFrame() { }

            public void ToggleColliders(bool isOpen)
            {
                if (_colliders == null)
                {
                    _colliders = _frameTransform.GetComponentsInChildren<Collider>();
                }

                for (int i = 0; i < _colliders.Length; i++)
                {
                    // If the door is open, the colliders are disabled. If it is closed, they are enabled.
                    _colliders[i].enabled = !isOpen;
                }
            }
            
            
            /// <summary> Handle a single frame of opening logic.</summary>
            /// <returns> True if completed transition. False if not.</returns>
            public bool HandleOpeningTick(bool isOpening, bool openedFromFacingDirection)
            {
                if (isOpening && _elapsedTime >= _openingDuration)
                {
                    // Finished (Positive).
                    _elapsedTime = _openingDuration;
                    return true;

                }
                else if (!isOpening && _elapsedTime <= 0.0f)
                {
                    // Finished (Negative).
                    _elapsedTime = 0.0f;
                    return true;
                }

                if (isOpening)
                    _elapsedTime += Time.deltaTime;
                else
                    _elapsedTime -= Time.deltaTime;


                float lerpTime = _elapsedTime / _openingDuration;
                HandlePositionTick(lerpTime);
                HandleRotationTick(lerpTime, openedFromFacingDirection);

                return false;
            }

            private void HandlePositionTick(float lerpTime)
            {
                if (!_affectPosition)
                {
                    return;
                }

                // Handle position change.
                _frameTransform.localPosition = Vector3.Lerp(_closedPosition, _openPosition, _positionCurve.Evaluate(lerpTime));
            }
            private void HandleRotationTick(float lerpTime, bool openedFromFacingDirection)
            {
                if (!_affectRotation)
                {
                    return;
                }

                // Handle rotation change.
                _frameTransform.localRotation = Quaternion.Lerp(
                    a: Quaternion.Euler(_closedRotation),
                    b: Quaternion.Euler((_flipRotationWhenOpeningFromBehind && !openedFromFacingDirection) ? -_openRotation : _openRotation),
                    t: _rotationCurve.Evaluate(lerpTime));
            }
        }
    }
}