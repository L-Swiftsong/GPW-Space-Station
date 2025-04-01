using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Audio.Footsteps;
using Saving;
/*
 * CONTEXT:
 * 
 * This script handles the player's movement, jumping, gravity effects, crouching, tilting, and gravity. 
 */

namespace Entities.Player
{
    public class PlayerController : MonoBehaviour
    {
        [Header("General References")]
        [SerializeField] private Transform _rotationPivot;
        private PlayerHealth playerHealth;
        private CharacterController _controller;
        private CameraFocusLook cameraFocusLook;
        private CameraShake cameraShake;

        private MovementState _currentMovementState = MovementState.Walking;


        [Header("Movement Settings")]
        [SerializeField] private float moveSpeed = 4.0f;
        [SerializeField] private float sprintSpeed = 6.0f;


        [Header("Jumping Settings")]
        [SerializeField] private float jumpHeight = 1.2f;
        [SerializeField] private float _jumpCooldown = 0.1f;
        private bool _canJump = true;


        [Header("Gravity Settings")]
        [SerializeField] private float gravity = -15.0f;    
        [SerializeField] private float lowGravity = -2.0f;
        private float _verticalVelocity;
        private const float TERMINAL_VELOCITY = 53.0f;
        private bool _inLowGravityZone = false;


        [Header("Grounded Settings")]
        [SerializeField] private Transform groundCheck;
        [SerializeField] private float groundedRadius = 0.5f;
        [SerializeField] private LayerMask groundLayers;
        private bool _isGrounded = true; 


        [Header("Crouch Settings")]
        [SerializeField] private float _crouchSpeed = 2.0f;

        [Space(5)]
        [SerializeField] private float crouchHeight = 1.35f;
        [SerializeField] private float normalHeight = 1.8f;
        [SerializeField] private float _heightSpeedChange = 3.0f;

        [Space(5)]
        [SerializeField] private LayerMask _crouchObstacleLayers;


        [Header("Crawling Settings")]
        [SerializeField] private float _crawlingHeight = 0.9f;
        [SerializeField] private float _crawlingSpeed = 2.0f;
        [SerializeField] private float _crawlingCameraHeight = 0.75f;


        [Header("Tilt Settings")]
        [SerializeField] private float tiltSpeed = 5.0f;
        [SerializeField] private float tiltAngle = 15.0f;
        [SerializeField] private float peekOffset = 0.3f;
        private float _currentTilt = 0.0f;
        private float _currentPeekOffset = 0.0f;

        private Coroutine _tiltingCoroutine;

        [System.Serializable] enum CurrentTiltState { NotTilting, Left, Right };
        private CurrentTiltState _currentTiltState;


        [Header("Camera Settings")]
        [SerializeField] private GameObject _playerCamera;
        private float _rotationX = 0.0f;

        [Space(5)]
        [SerializeField] private float _defaultCameraHeight = 1.6f;
        [SerializeField] private float _crouchedCameraHeight = 1.0f;


        [Header("Footstep Settings")]
        [SerializeField] private EntityFootstepClips _footstepClips;
        private float _timeTillNextFootstep;

        [Space(5)]
        [SerializeField] private float _defaultStepRate = 2.0f;
        [SerializeField] private float _sprintingStepRate = 3.5f;
        [SerializeField] private float _crouchedStepRate = 0.75f;
        [SerializeField] private float _crawlingStepRate = 0.75f;

        [Space(5)]
        [SerializeField] private float _walkingFootstepDetectionRadius = 3.0f;
        [SerializeField] private float _sprintingFootstepDetectionRadius = 10.0f;
        [SerializeField] private float _crouchingFootstepDetectionRadius = 1.0f;
        [SerializeField] private float _crawlingFootstepDetectionRadius = 0.0f;

        [Space(5)]
        [SerializeField] private bool _drawDetectionRadiusGizmos = false;
        [SerializeField] private MovementState _detectionRadiusDebugState;

        private bool _wantsToSprint = false;
        private bool _wantsToCrouch = false;
        private bool _wantsToCrawl = false;
        private bool _isHiding = false;


        private float baseMoveSpeed;
        private float baseSprintSpeed;
        private float healMoveSpeed = 2f;
        private float healSprintSpeed = 3f;


        private void Awake()
        {
            // Get references.
            _controller = GetComponent<CharacterController>();
            playerHealth = GetComponent<PlayerHealth>();
            cameraFocusLook = GetComponent<CameraFocusLook>();
            cameraShake = GetComponentInChildren<CameraShake>();

            // Start walking.
            _currentMovementState = MovementState.Walking;
        }
        private void Start()
        {
            // Ensure that the cursor starts locked.
            Cursor.lockState = CursorLockMode.Locked;

            // Get player speed.
            baseMoveSpeed = moveSpeed;
            baseSprintSpeed = sprintSpeed;

        }
        private void OnEnable()
        {
            // Subscribe to PlayerInput events.
            PlayerInput.OnJumpPerformed += PlayerInput_OnJumpPerformed;

            PlayerInput.OnCrouchPerformed += PlayerInput_OnCrouchPerformed;
            PlayerInput.OnCrouchStarted += PlayerInput_OnCrouchStarted;
            PlayerInput.OnCrouchCancelled += PlayerInput_OnCrouchCancelled;

            PlayerInput.OnSprintPerformed += PlayerInput_OnSprintPerformed;
            PlayerInput.OnSprintStarted += PlayerInput_OnSprintStarted;
            PlayerInput.OnSprintCancelled += PlayerInput_OnSprintCancelled;

            PlayerInput.OnLeanLeftStarted += PlayerInput_OnLeanLeftStarted;
            PlayerInput.OnLeanLeftCancelled += PlayerInput_OnLeanLeftCancelled;
            PlayerInput.OnLeanRightStarted += PlayerInput_OnLeanRightStarted;
            PlayerInput.OnLeanRightCancelled += PlayerInput_OnLeanRightCancelled;
        }
        private void OnDisable()
        {
            // Unsubscribe from PlayerInput events.
            PlayerInput.OnJumpPerformed -= PlayerInput_OnJumpPerformed;

            PlayerInput.OnCrouchPerformed -= PlayerInput_OnCrouchPerformed;
            PlayerInput.OnCrouchStarted -= PlayerInput_OnCrouchStarted;
            PlayerInput.OnCrouchCancelled -= PlayerInput_OnCrouchCancelled;

            PlayerInput.OnSprintPerformed -= PlayerInput_OnSprintPerformed;
            PlayerInput.OnSprintStarted -= PlayerInput_OnSprintStarted;
            PlayerInput.OnSprintCancelled -= PlayerInput_OnSprintCancelled;

            PlayerInput.OnLeanLeftStarted -= PlayerInput_OnLeanLeftStarted;
            PlayerInput.OnLeanLeftCancelled -= PlayerInput_OnLeanLeftCancelled;
            PlayerInput.OnLeanRightStarted -= PlayerInput_OnLeanRightStarted;
            PlayerInput.OnLeanRightCancelled -= PlayerInput_OnLeanRightCancelled;
        }


        #region Input Functions

        private void PlayerInput_OnJumpPerformed() => PerformJump();

        private void PlayerInput_OnCrouchPerformed()
        {
            if (!PlayerSettings.ToggleCrouch)
            {
                // We aren't using toggle crouch.
                return;
            }

            if (_wantsToCrouch)
            {
                // We are currently crouching. Stop crouching.
                StopCrouching();
            }
            else
            {
                // We are currently not crouching. Start crouching.
                StartCrouching();
            }
        }
        private void PlayerInput_OnCrouchStarted()
        {
            if (!PlayerSettings.ToggleCrouch)
            {
                // Toggle Crouch is disabled.
                // Start crouching now the button is pressed.
                StartCrouching();
            }
        }
        private void PlayerInput_OnCrouchCancelled()
        {
            if (!PlayerSettings.ToggleCrouch)
            {
                // Toggle Crouch is disabled.
                // Stop crouching now the button is released.
                StopCrouching();
            }
        }

        private void PlayerInput_OnSprintPerformed()
        {
            if (PlayerSettings.ToggleSprint)
            {
                if (_wantsToSprint)
                {
                    StopSprinting();
                }
                else
                {
                    StartSprinting();
                }
            }
        }
        private void PlayerInput_OnSprintStarted()
        {
            if (!PlayerSettings.ToggleSprint)
            {
                StartSprinting();
            }
        }
        private void PlayerInput_OnSprintCancelled()
        {
            if (!PlayerSettings.ToggleSprint)
            {
                StopSprinting();
            }
        }

        private void PlayerInput_OnLeanLeftStarted() => StartTilting(tiltLeft: true);
        private void PlayerInput_OnLeanLeftCancelled()
        {
            if (_currentTiltState == CurrentTiltState.Left)
            {
                // We were leaning to the left. Stop leaning.
                StopTilting();
            }
        }

        private void PlayerInput_OnLeanRightStarted() => StartTilting(tiltLeft: false);
        private void PlayerInput_OnLeanRightCancelled()
        {
            if (_currentTiltState == CurrentTiltState.Right)
            {
                // We were leaning to the right. Stop leaning.
                StopTilting();
            }
        }


        #endregion


        private void Update()
        {
            GroundedCheck();

            if (!_isHiding)
            {
                HandleStateChange();
            
                HandleMovement();
                UpdateCameraTransform();
                HandleSprintToggleCheck();
                TickFootstepTime();

                HandleGravity();
                UpdateCharacterHeight();

                CameraShake();
            }
            else
            {
                StopCameraShake();
            }

            HandleLook();

        
            if (playerHealth.IsHealing)
            {
                moveSpeed = healMoveSpeed;
                sprintSpeed = healSprintSpeed;
            }
            else
            {
                moveSpeed = baseMoveSpeed;
                sprintSpeed = baseSprintSpeed;
            }
        }


        private void CameraShake()
        {
            if (PlayerInput.MovementInput != Vector2.zero)
            {
                if (_currentMovementState == MovementState.Walking)
                {
                    cameraShake.SetShakeState(isWalking: true, isRunning: false);
                }
                else if (_currentMovementState == MovementState.Sprinting)
                {
                    cameraShake.SetShakeState(isWalking: false, isRunning: true);
                }
            }
            else
            {
                StopCameraShake();
            }
        }
        private void StopCameraShake() => cameraShake.SetShakeState(isWalking: false, isRunning: false);


        private void HandleStateChange()
        {
            if (_isHiding)
            {
                // We are currently hiding.
                _currentMovementState = MovementState.Hiding;
                return;
            }
        

            if ((_currentMovementState == MovementState.Crouching || _wantsToCrouch) && _wantsToCrawl)
            {
                // We are wanting to crawl.
                _currentMovementState = MovementState.Crawling;
                return;
            }

            if (_wantsToCrouch)
            {
                // We are wanting to crouch.
                _currentMovementState = MovementState.Crouching;
                return;
            }


            if ((_currentMovementState == MovementState.Crouching || _currentMovementState == MovementState.Crawling) && !CanStopCrouching())
            {
                // We are currently crouching/crawling (Even though we don't want to be), and cannot stop doing so.
                return;
            }

            if (_wantsToSprint)
            {
                // We are wanting to sprint.
                _currentMovementState = MovementState.Sprinting;
                return;
            }


            // Default to the 'Walking' state.
            _currentMovementState = MovementState.Walking;
        }
    
    
        private void GroundedCheck()
        {
            _isGrounded = Physics.CheckSphere(groundCheck.position, groundedRadius, groundLayers, QueryTriggerInteraction.Ignore);
        }

        /// <summary>
        /// Handles player movement.
        /// </summary>
        private void HandleMovement()
        {
            // Determine our current Movement Speed based on our current Movement State.
            float targetSpeed = _currentMovementState switch
            {
                MovementState.Sprinting => sprintSpeed,
                MovementState.Crouching => _crouchSpeed,
                MovementState.Crawling => _crawlingSpeed,
                _ => moveSpeed,
            };

            // Calculate our desired movement direction.
            Vector2 movementInput = PlayerInput.MovementInput;
            Vector3 inputDirection = _rotationPivot.right * movementInput.x + _rotationPivot.forward * movementInput.y;
        
            // Apply movement.
            _controller.Move(inputDirection * targetSpeed * Time.deltaTime);
        }


        #region Character Height

        private void UpdateCharacterHeight()
        {
            float targetHeight = _currentMovementState switch
            {
                MovementState.Crouching => crouchHeight,
                MovementState.Crawling => _crawlingHeight,
                _ => normalHeight,
            };
            _controller.height = Mathf.MoveTowards(_controller.height, targetHeight, _heightSpeedChange * Time.deltaTime);

            _controller.center = new Vector3(0.0f, _controller.height / 2.0f, 0.0f);
        }
        private void UpdateCharacterHeightInstant()
        {
            float targetHeight = _currentMovementState switch
            {
                MovementState.Crouching => crouchHeight,
                MovementState.Crawling => _crawlingHeight,
                _ => normalHeight,
            };
            float heightOffset = targetHeight - _controller.height;
            _controller.height = targetHeight;

            _controller.center = new Vector3(0.0f, _controller.height / 2.0f, 0.0f);
            transform.position += Vector3.up * heightOffset;
        }

        public float GetDefaultHeight() => normalHeight;
        public float GetDefaultCameraHeight() => _defaultCameraHeight;

#endregion


        #region Camera

        /// <summary>
        /// Handles player look/rotation using clamping to avoid over-rotating.
        /// </summary>
        private void HandleLook()
        {
			if (cameraFocusLook != null && cameraFocusLook.IsFocusLookActive())
			{
				_rotationX = _playerCamera.transform.localEulerAngles.x;
                if (_rotationX > 180.0f)
                    _rotationX -= 360.0f;

                return;
			}
            
            // Get the current look input (Already has sensitivity & inversion applied).
            Vector2 lookInput = PlayerInput.GetLookInputWithSensitivity * Time.deltaTime;

			_rotationX -= lookInput.y;
            _rotationX = Mathf.Clamp(_rotationX, -90f, 90f);

            _playerCamera.transform.localRotation = Quaternion.Euler(_rotationX, 0.0f, _currentTilt);
            _rotationPivot.Rotate(Vector3.up * lookInput.x);
        }
        public void SetCameraRotation(float xRotation) => _rotationX = xRotation;
        public float GetCameraRotation() => _rotationX;


        /// <summary>
        /// Updates the camera's current height for crouching & peeking.
        /// </summary>
        private void UpdateCameraTransform()
        {
            float cameraHeight = Mathf.MoveTowards(_playerCamera.transform.localPosition.y, GetDesiredCameraHeight(), _heightSpeedChange * Time.deltaTime);
            _playerCamera.transform.localPosition = new Vector3(_currentPeekOffset, cameraHeight, 0.0f);
        }
        private void UpdateCameraTransformInstant()
        {
            float cameraHeight = GetDesiredCameraHeight();
            _playerCamera.transform.localPosition = new Vector3(_currentPeekOffset, cameraHeight, 0.0f);
        }

        private float GetDesiredCameraHeight()
        {
            return _currentMovementState switch
            {
                MovementState.Crouching => _crouchedCameraHeight,
                MovementState.Crawling => _crawlingCameraHeight,
                _ => _defaultCameraHeight,
            };
        }

        #endregion


        #region Sprinting

        private void HandleSprintToggleCheck()
        {
            if (!PlayerSettings.ToggleSprint || !_wantsToSprint)
            {
                // We aren't using Toggle Sprint (Or don't want to sprint), so this function shouldn't be run.
                return;
            }

            if (PlayerInput.MovementInput == Vector2.zero)
            {
                // We aren't moving. Stop Sprinting.
                _wantsToSprint = false;
            }
        }

        private void StartSprinting()
        {
            if (PlayerSettings.ToggleCrouch)
                _wantsToCrouch = false;

            _wantsToSprint = true;
        }
        private void StopSprinting() => _wantsToSprint = false;

        #endregion


        #region Jumping

        /// <summary> Handles jumping.</summary>
        private void PerformJump()
        {
            if (!_isGrounded)
            {
                // We aren't grounded.
                return;
            }
            if (!_canJump)
            {
                // Our jump hasn't reset.
                return;
            }
            if (_currentMovementState == MovementState.Crouching || _currentMovementState == MovementState.Crawling)
            {
                // We are crouching or crawling, so cannot jump.
                return;
            }

        
            // Calculate the velocity required to reach our jump height, assuming our gravity is unchanged.
            _verticalVelocity = Mathf.Sqrt(-2f * gravity * jumpHeight);
            _canJump = false;

            StartCoroutine(ResetJump());
        }

        /// <summary> Coroutine to reset the jump ability.</summary>
        private IEnumerator ResetJump()
        {
            yield return new WaitForSeconds(_jumpCooldown);
            _canJump = true;
        }

        #endregion


        #region Crouching

        /// <summary> Notify the controller that we want to start crouching.</summary>
        private void StartCrouching()
        {
            if (PlayerSettings.ToggleSprint)
                StopSprinting();

            _wantsToCrouch = true;
        }
        /// <summary> Notify the controller that we want to stop crouching. We'll stop crouching once we are able to.</summary>
        private void StopCrouching() => _wantsToCrouch = false;

        private bool CanStopCrouching()
        {
            if (Physics.Raycast(transform.position, Vector3.up, normalHeight, _crouchObstacleLayers))
            {
                // There is an obstacle stopping us from standing up.
                return false;
            }

            return true;
        }

        #endregion


        #region Crawling

        public void TryStartCrawling() => _wantsToCrawl = true;
        public void TryStopCrawling() => _wantsToCrawl = false;
    

        #endregion


        #region Tilting

        /// <summary> Starts the camera tilting in the desired direction.</summary>
        private void StartTilting(bool tiltLeft)
        {
            // Start tilting towards our desired tilt angle.
            if (_tiltingCoroutine != null)
            {
                StopCoroutine(_tiltingCoroutine);
            }

            float targetTilt = tiltLeft ? tiltAngle : -tiltAngle;
            float targetPeekOffset = tiltLeft ? -peekOffset : peekOffset;
            _tiltingCoroutine = StartCoroutine(LerpTilt(targetTilt, targetPeekOffset));

            _currentTiltState = tiltLeft ? CurrentTiltState.Left : CurrentTiltState.Right;
        }
        /// <summary> Reverts the camera tilt so that it is no longer tilting.</summary>
        private void StopTilting()
        {
            // Return our tilting towards 0.0f.
            if (_tiltingCoroutine != null)
            {
                StopCoroutine(_tiltingCoroutine);
            }
            _tiltingCoroutine = StartCoroutine(LerpTilt(0.0f, 0.0f));

            _currentTiltState = CurrentTiltState.NotTilting;
        }

        /// <summary> Lerp the value of '_currentTilt' towards the value of 'targetTilt'.</summary>
        private IEnumerator LerpTilt(float targetTilt, float targetOffset)
        {
            // Loop until we have reached our desired tilt.
            while(_currentTilt != targetTilt || _currentPeekOffset != targetOffset)
            {
                // Lerp the current tilt towards the target tilt.
                _currentTilt = Mathf.Lerp(_currentTilt, targetTilt, tiltSpeed * Time.deltaTime);
                _currentPeekOffset = Mathf.Lerp(_currentPeekOffset, targetOffset, tiltSpeed * Time.deltaTime);

                yield return null;
            }
        }

        #endregion


        #region Gravity

        /// <summary> Applies gravity based on the player's environment (normal or low-gravity).</summary>
        private void HandleGravity()
        {
            // Determine current vertical velocity using gravity.
            float appliedGravity = _inLowGravityZone ? lowGravity : gravity;

            if (_isGrounded && _verticalVelocity < 0.0f)
            {
                // We are grounded and are moving downwards.
                // Constrain our vertical velocity so that we don't eternally build up speed.
                _verticalVelocity = -2f;
            }
            // Accelerate the player downwards in regards to their current gravity.
            _verticalVelocity += appliedGravity * Time.deltaTime;

            // Clamp our current velocity to within our terminal velocity.
            _verticalVelocity = Mathf.Clamp(_verticalVelocity, -TERMINAL_VELOCITY, TERMINAL_VELOCITY);
        

            // Apply current vertical velocity (Mainly gravity).
            _controller.Move(Vector3.up * _verticalVelocity * Time.deltaTime);
        }

        /// <summary> For when the player enters a low-gravity zone.</summary>
        public void EnterLowGravityZone()
        {
            _inLowGravityZone = true;
        }

        /// <summary> For when the player exits a low-gravity zone.</summary>
        public void ExitLowGravityZone()
        {
            _inLowGravityZone = false;
        }

        #endregion


        #region Footsteps

        private void TickFootstepTime()
        {
            // To-do: Change to be intention-based rather than input based
            if (PlayerInput.MovementInput == Vector2.zero)
            {
                // We aren't moving.
                return;
            }
            /*if (_currentMovementState == MovementState.Hiding)
            {
                // We are hiding.
                return;
            }*/

            _timeTillNextFootstep -= Time.deltaTime * GetCurrentFootstepRate();

            if (_timeTillNextFootstep <= 0.0f)
            {
                PlayFootstep();
                _timeTillNextFootstep = 1.0f;
            }
        }
        private void PlayFootstep()
        {
            // Retrieve the values for our footstep clip based on our current material & movement state..
            (AudioClip FootstepClip, Vector2 PitchRange) footstepClipValues = _footstepClips.GetAudioSettings(_currentMovementState);

            // Determine our detectable radius based on our movement state.
            float detectionRadius = _currentMovementState switch {
                MovementState.Sprinting => _sprintingFootstepDetectionRadius,
                MovementState.Walking => _walkingFootstepDetectionRadius,
                MovementState.Crouching => _crouchingFootstepDetectionRadius,
                MovementState.Crawling => _crawlingFootstepDetectionRadius,
                _ => 0.0f,
            };

            // Play our FootstepClip via the SFXManager.
            Audio.SFXManager.Instance.PlayDetectableClipAtPosition(footstepClipValues.FootstepClip, transform.position, detectionRadius, minPitch: footstepClipValues.PitchRange.x, maxPitch: footstepClipValues.PitchRange.y);
        }

        private float GetCurrentFootstepRate()
        {
            return _defaultStepRate * (_currentMovementState switch
            {
                MovementState.Hiding => 0.0f,
                MovementState.Sprinting => _sprintingStepRate,
                MovementState.Crouching => _crouchedStepRate,
                MovementState.Crawling => _crawlingStepRate,
                _ => 1.0f
            });
        }

        #endregion


        public void SetHiding(bool hiding) => _isHiding = hiding;
        public bool GetHiding() => _isHiding;

        public MovementState GetCurrentMovementState() => _currentMovementState;
        public void InitialiseMovementState(MovementState movementState)
        {
            // Set the current movement state.
            switch (movementState)
            {
                case MovementState.Crouching:
                    _wantsToCrouch = true;
                    break;
                case MovementState.Crawling:
                    _wantsToCrouch = true;
                    _wantsToCrawl = true;
                    break;
            }
            _currentMovementState = movementState;

            // Update the controller & camera heights.
            UpdateCharacterHeightInstant();
            UpdateCameraTransformInstant();
        }
        public float GetYRotation() => _rotationPivot.localEulerAngles.y;
        public void SetYRotation(float yRotation) => _rotationPivot.localRotation = Quaternion.Euler(0.0f, yRotation, 0.0f);


        private void OnDrawGizmosSelected()
        {
            if (_drawDetectionRadiusGizmos)
            {
                Gizmos.color = Color.yellow;
                float detectionRadius = _detectionRadiusDebugState switch
                {
                    MovementState.Sprinting => _sprintingFootstepDetectionRadius,
                    MovementState.Walking => _walkingFootstepDetectionRadius,
                    MovementState.Crouching => _crouchingFootstepDetectionRadius,
                    MovementState.Crawling => _crawlingFootstepDetectionRadius,
                    _ => 0.0f
                };

                Gizmos.DrawWireSphere(transform.position, detectionRadius);
            }
        }
    }
}