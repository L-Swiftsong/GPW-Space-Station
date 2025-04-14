using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;

public class PlayerInput : MonoBehaviour
{
    private static PlayerInputActions s_playerInput;


    #region Input Events

    public static event Action OnPauseGamePerformed;

    public static event Action OnOpenJournalPerformed;
    public static event Action OnOpenItemsPerformed;


    public static event Action OnSelectNextTabPerformed;
    public static event Action OnSelectPreviousTabPerformed;


    public static event Action OnCrouchPerformed;
    public static event Action OnCrouchStarted;
    public static event Action OnCrouchCancelled;

    public static event Action OnSprintPerformed;
    public static event Action OnSprintStarted;
    public static event Action OnSprintCancelled;


    public static event Action OnLeanLeftStarted;
    public static event Action OnLeanLeftCancelled;

    public static event Action OnLeanRightStarted;
    public static event Action OnLeanRightCancelled;


    public static event Action OnInteractPerformed;


    public static event Action OnToggleFlashlightPerformed;

    public static event Action OnFocusFlashlightStarted;
    public static event Action OnFocusFlashlightCancelled;


    public static event Action OnUseHealingItemStarted;
    public static event Action OnUseHealingItemCancelled;


    #region UI

    public static event Action OnUINavigateCancelled;

    public static event Action OnUISubmitPerformed;

    public static event Action OnUICancelPerformed;

    public static event Action OnUILeftClickPerformed;

    #endregion

    #endregion

    #region Input Values

    private const string MOUSE_AND_KEYBOARD_CONTROL_SCHEME_NAME = "MnK";
    private const string GAMEPAD_CONTROL_SCHEME_NAME = "Gamepad";

    private static InputControlScheme s_currentControlScheme = default;
    public static InputControl CurrentInputDevice { get; private set; }

    public enum DeviceType { MnK, Gamepad }
    public static DeviceType LastUsedDevice => s_currentControlScheme.name == MOUSE_AND_KEYBOARD_CONTROL_SCHEME_NAME ? DeviceType.MnK : DeviceType.Gamepad;


    // Movement Input.
    private static Vector2 s_movementInput;
    public static Vector2 MovementInput => s_movementInput;


    // Look Input (Mouse).
    private static Vector2 s_mouseLookInput;
    public static Vector2 MouseLookInput => s_mouseLookInput;
    public static float MouseLookX => s_mouseLookInput.x;
    public static float MouseLookY => s_mouseLookInput.y;


    // Look Input (Gamepad).
    private static Vector2 s_gamepadLookInput;
    public static Vector2 GamepadLookInput => s_gamepadLookInput;
    public static float GamepadLookX => s_gamepadLookInput.x;
    public static float GamepadLookY => s_gamepadLookInput.y;

    public static Vector2 GetLookInputWithSensitivity =>LastUsedDevice switch {
        // Device is Mouse & Keyboard.
        DeviceType.MnK => new Vector2(
            x: MouseLookX * PlayerSettings.MouseHorizontalSensititvity,
            y: (PlayerSettings.MouseInvertY ? -1 : 1) * MouseLookY * PlayerSettings.MouseVerticalSensititvity),
        // Device is not Mouse & Keyboard (Assume Gamepad).
        _ => new Vector2(
            x: GamepadLookX * PlayerSettings.GamepadHorizontalSensititvity,
            y: (PlayerSettings.GamepadInvertY ? -1 : 1) * GamepadLookY * PlayerSettings.GamepadVerticalSensititvity)
    };


    // Gamepad Select.
    private static Vector2 s_gamepadInventorySelect;
    public static Vector2 GamepadInventorySelect => s_gamepadInventorySelect;


    // UI Elements.
    private static float s_sliderHorizontal;
    public static float SliderHorizontal => s_sliderHorizontal;


    private static Vector2 s_uiNavigate;
    public static Vector2 UINavigate => s_uiNavigate;

    #endregion


    private void Awake()
    {
        CreateInputActions();

        InputSystem.onDeviceChange += InputSystem_onDeviceChange;
        InputUser.onUnpairedDeviceUsed += InputUser_onUnpairedDeviceUsed;
        InputUser.listenForUnpairedDeviceActivity = 1;
    }
    private void OnEnable()
    {
        if (s_playerInput == null)
        {
            CreateInputActions();
        }
    }
    private void OnDisable() => DestroyInputActions();
    private void OnDestroy()
    {
        DestroyInputActions();

        InputSystem.onDeviceChange -= InputSystem_onDeviceChange;
        InputUser.onUnpairedDeviceUsed -= InputUser_onUnpairedDeviceUsed;
    }


    #if UNITY_EDITOR

    [ContextMenu(itemName: "Display Active Locks")]
    private void DisplayLocks()
    {
        string movementPreventingTypes = string.Concat(s_typeToMovementPreventionCountDictionary.Keys);
        Debug.Log(s_typeToMovementPreventionCountDictionary.Count + "\n" + movementPreventingTypes);

        string cameraPreventingTypes = string.Concat(s_typeToCameraPreventionCountDictionary.Keys);
        Debug.Log(s_typeToCameraPreventionCountDictionary.Count + "\n" + cameraPreventingTypes);

        string interactionPreventingTypes = string.Join(", ", s_typeToInteractionPreventionCountDictionary.Keys);
        Debug.Log(s_typeToInteractionPreventionCountDictionary.Count + "\n" + interactionPreventingTypes);
    }

    #endif

    private void CreateInputActions()
    {
        // Create the InputActionMap.
        s_playerInput = new PlayerInputActions();


        // Subscribe to events (Global).
        s_playerInput.Global.PauseGame.performed += PauseGame_performed;

        s_playerInput.Global.OpenJournal.performed += OpenJournal_performed;
        s_playerInput.Global.OpenItems.performed += OpenItems_performed;

        // Subscribe to events (UI).
        s_playerInput.UI.SelectNextTab.performed += SelectNextTab_performed;
        s_playerInput.UI.SelectPreviousTab.performed += SelectPreviousTab_performed;

        s_playerInput.UI.Navigate.canceled += Navigate_cancelled;

        s_playerInput.UI.Submit.performed += Submit_performed;
        s_playerInput.UI.Cancel.performed += Cancel_performed;
        s_playerInput.UI.LeftClick.performed += LeftClick_performed;


        // Subscribe to events (Movement).
        s_playerInput.Movement.Crouch.performed += Crouch_performed;
        s_playerInput.Movement.Crouch.started += Crouch_started;
        s_playerInput.Movement.Crouch.canceled += Crouch_cancelled;

        s_playerInput.Movement.Sprint.performed += Sprint_performed;
        s_playerInput.Movement.Sprint.started += Sprint_started;
        s_playerInput.Movement.Sprint.canceled += Sprint_cancelled;

        s_playerInput.Movement.LeanLeft.started += LeanLeft_started;
        s_playerInput.Movement.LeanLeft.canceled += LeanLeft_cancelled;
        s_playerInput.Movement.LeanRight.started += LeanRight_started;
        s_playerInput.Movement.LeanRight.canceled += LeanRight_cancelled;


        // Subscribe to events (Interaction).
        s_playerInput.Interaction.Interact.performed += Interact_performed;

        s_playerInput.Interaction.ToggleFlashlight.performed += ToggleFlashlight_performed;

        s_playerInput.Interaction.FocusFlashlight.started += FocusFlashlight_started;
        s_playerInput.Interaction.FocusFlashlight.canceled += FocusFlashlight_cancelled;

        s_playerInput.Interaction.UseHealingItem.started += UseHealingItem_started;
        s_playerInput.Interaction.UseHealingItem.canceled += UseHealingItem_cancelled;


        // Enable maps.
        s_playerInput.Global.Enable();
        s_playerInput.UI.Enable();
        s_playerInput.Movement.Enable();
        s_playerInput.Camera.Enable();
        s_playerInput.Interaction.Enable();

        s_playerInput.Enable();

        // Disable maps that have elements wishing them disabled.
        UpdateDisabledState();
    }
    private void DestroyInputActions()
    {
        // Ensure that a playerInput instance exists.
        if (s_playerInput == null)
        {
            return;
        }


        // Unsubscribe from events (Global).
        s_playerInput.Global.PauseGame.performed -= PauseGame_performed;

        s_playerInput.Global.OpenJournal.performed -= OpenJournal_performed;
        s_playerInput.Global.OpenItems.performed -= OpenItems_performed;


        // Unsubscribe from events (UI).
        s_playerInput.UI.SelectNextTab.performed -= SelectNextTab_performed;
        s_playerInput.UI.SelectPreviousTab.performed -= SelectPreviousTab_performed;

        s_playerInput.UI.Submit.performed -= Submit_performed;
        s_playerInput.UI.Cancel.performed -= Cancel_performed;
        s_playerInput.UI.LeftClick.performed -= LeftClick_performed;


        // Unsubscribe from events (Movement).
        s_playerInput.Movement.Crouch.performed -= Crouch_performed;
        s_playerInput.Movement.Crouch.started -= Crouch_started;
        s_playerInput.Movement.Crouch.canceled -= Crouch_cancelled;

        s_playerInput.Movement.Sprint.performed -= Sprint_performed;
        s_playerInput.Movement.Sprint.started -= Sprint_started;
        s_playerInput.Movement.Sprint.canceled -= Sprint_cancelled;

        s_playerInput.Movement.LeanLeft.started -= LeanLeft_started;
        s_playerInput.Movement.LeanLeft.canceled -= LeanLeft_cancelled;
        s_playerInput.Movement.LeanRight.started -= LeanRight_started;
        s_playerInput.Movement.LeanRight.canceled -= LeanRight_cancelled;


        // Unsubscribe from events (Interaction).
        s_playerInput.Interaction.Interact.performed -= Interact_performed;

        s_playerInput.Interaction.ToggleFlashlight.performed -= ToggleFlashlight_performed;

        s_playerInput.Interaction.FocusFlashlight.started -= FocusFlashlight_started;
        s_playerInput.Interaction.FocusFlashlight.canceled -= FocusFlashlight_cancelled;

        s_playerInput.Interaction.UseHealingItem.started -= UseHealingItem_started;
        s_playerInput.Interaction.UseHealingItem.canceled -= UseHealingItem_cancelled;


        // Dispose of the PlayerInputAction instance.
        s_playerInput.Dispose();
    }


    #region Input Functions

    private void PauseGame_performed(InputAction.CallbackContext context) => OnPauseGamePerformed?.Invoke();

    private void OpenJournal_performed(InputAction.CallbackContext context) => OnOpenJournalPerformed?.Invoke();
    private void OpenItems_performed(InputAction.CallbackContext context) => OnOpenItemsPerformed?.Invoke();

    private void SelectNextTab_performed(InputAction.CallbackContext context) => OnSelectNextTabPerformed?.Invoke();
    private void SelectPreviousTab_performed(InputAction.CallbackContext context) => OnSelectPreviousTabPerformed?.Invoke();


    private void Crouch_performed(InputAction.CallbackContext context) => OnCrouchPerformed?.Invoke();
    private void Crouch_started(InputAction.CallbackContext context) => OnCrouchStarted?.Invoke();
    private void Crouch_cancelled(InputAction.CallbackContext context) => OnCrouchCancelled?.Invoke();


    private void Sprint_performed(InputAction.CallbackContext context) => OnSprintPerformed?.Invoke();
    private void Sprint_started(InputAction.CallbackContext context) => OnSprintStarted?.Invoke();
    private void Sprint_cancelled(InputAction.CallbackContext context) => OnSprintCancelled?.Invoke();


    private void LeanLeft_started(InputAction.CallbackContext context) => OnLeanLeftStarted?.Invoke();
    private void LeanLeft_cancelled(InputAction.CallbackContext context) => OnLeanLeftCancelled?.Invoke();
    private void LeanRight_started(InputAction.CallbackContext context) => OnLeanRightStarted?.Invoke();
    private void LeanRight_cancelled(InputAction.CallbackContext context) => OnLeanRightCancelled?.Invoke();


    private void Interact_performed(InputAction.CallbackContext context) => OnInteractPerformed?.Invoke();


    private void ToggleFlashlight_performed(InputAction.CallbackContext context) => OnToggleFlashlightPerformed?.Invoke();

    private void FocusFlashlight_started(InputAction.CallbackContext context) => OnFocusFlashlightStarted?.Invoke();
    private void FocusFlashlight_cancelled(InputAction.CallbackContext context) => OnFocusFlashlightCancelled?.Invoke();

    private void UseHealingItem_started(InputAction.CallbackContext context) => OnUseHealingItemStarted?.Invoke();
    private void UseHealingItem_cancelled(InputAction.CallbackContext context) => OnUseHealingItemCancelled?.Invoke();


    private void Navigate_cancelled(InputAction.CallbackContext obj) => OnUINavigateCancelled?.Invoke();

    private void Submit_performed(InputAction.CallbackContext obj) => OnUISubmitPerformed?.Invoke();
    private void Cancel_performed(InputAction.CallbackContext obj) => OnUICancelPerformed?.Invoke();

    private void LeftClick_performed(InputAction.CallbackContext obj) => OnUILeftClickPerformed?.Invoke();


    #endregion


    #region Active Device Detection

    private void InputUser_onUnpairedDeviceUsed(InputControl inputControl, UnityEngine.InputSystem.LowLevel.InputEventPtr inputEventPtr)
    {
        // Get the control scheme associated with the used device.
        InputControlScheme deviceControlScheme = s_playerInput.controlSchemes.Where(t => t.SupportsDevice(inputControl.device)).FirstOrDefault();
        CurrentInputDevice = inputControl.device;

        if (deviceControlScheme != default && deviceControlScheme != s_currentControlScheme)
        {
            // A input device belonging to a different control scheme has been used.
            s_currentControlScheme = deviceControlScheme;
            Debug.Log("New Used. Scheme Name: " + deviceControlScheme.name);
        }
    }
    private void InputSystem_onDeviceChange(InputDevice inputDevice, InputDeviceChange inputDeviceChange)
    {
        if (inputDeviceChange == InputDeviceChange.Disconnected)
        {
            Debug.Log("User's device was disconnected");
        }
    }

    #endregion



    private void Update()
    {
        // Detect input.
        s_movementInput = s_playerInput.Movement.Movement.ReadValue<Vector2>();
        s_mouseLookInput = s_playerInput.Camera.MouseLookInput.ReadValue<Vector2>();
        s_gamepadLookInput = s_playerInput.Camera.GamepadLookInput.ReadValue<Vector2>();

        s_sliderHorizontal = s_playerInput.UI.SliderHorizontal.ReadValue<float>();
        s_uiNavigate = s_playerInput.UI.Navigate.ReadValue<Vector2>();
    }



    #region Action Prevention

    public static void ResetInputPrevention()
    {
        s_typeToMovementPreventionCountDictionary = new Dictionary<Type, int>();
        s_typeToInteractionPreventionCountDictionary = new Dictionary<Type, int>();
        s_typeToCameraPreventionCountDictionary = new Dictionary<Type, int>();
        s_typeToCameraPreventionCountDictionary = new Dictionary<Type, int>();
    }


    [System.Serializable] [System.Flags]
    public enum ActionTypes
    {
        None = 0,
        Movement = 1 << 0,
        Camera = 1 << 1,
        Interaction = 1 << 2,
        Global = 1 << 3,
        
        Custscene = ActionTypes.Movement | ActionTypes.Camera | ActionTypes.Interaction,
        CustsceneNoCamPrevention = ActionTypes.Custscene & ~ActionTypes.Camera,

        Everything = ~0,
    }
    public static void PreventActions(Type lockingType, ActionTypes actionTypes)
    {
        if (actionTypes.HasFlag(ActionTypes.Movement))
            PreventMovementActions(lockingType);
        if (actionTypes.HasFlag(ActionTypes.Camera))
            PreventCameraActions(lockingType);
        if (actionTypes.HasFlag(ActionTypes.Interaction))
            PreventInteractionActions(lockingType);
        if (actionTypes.HasFlag(ActionTypes.Global))
            PreventGlobalActions(lockingType);
    }
    public static void RemoveActionPrevention(Type lockingType, ActionTypes actionTypes)
    {
        if (actionTypes.HasFlag(ActionTypes.Movement))
            RemoveMovementActionPrevention(lockingType);
        if (actionTypes.HasFlag(ActionTypes.Camera))
            RemoveCameraActionPrevention(lockingType);
        if (actionTypes.HasFlag(ActionTypes.Interaction))
            RemoveInteractionActionPrevention(lockingType);
        if (actionTypes.HasFlag(ActionTypes.Global))
            RemoveGlobalActionPrevention(lockingType);
    }


    public static void PreventInputForCutscene(bool allowCameraMovement = false) => PreventActions(typeof(PlayerInput), allowCameraMovement ? ActionTypes.CustsceneNoCamPrevention : ActionTypes.Custscene);
    public static void RemoveInputPreventionForCutscene() => RemoveActionPrevention(typeof(PlayerInput), ActionTypes.Custscene);


    // Movement Map.
    private static Dictionary<Type, int> s_typeToMovementPreventionCountDictionary = new Dictionary<Type, int>();
    public static void PreventMovementActions(Type lockingType)
    {
        if (!s_typeToMovementPreventionCountDictionary.TryAdd(lockingType, 1))
        {
            // We already have a kvp with key 'lockingType', so increment it instead.
            s_typeToMovementPreventionCountDictionary[lockingType]++;
        }

        if (s_playerInput != null)
        {
            // Disable our 'Movement' map.
            s_playerInput.Movement.Disable();
        }
    }
    public static void RemoveMovementActionPrevention(Type lockingType)
    {
        if (s_typeToMovementPreventionCountDictionary.ContainsKey(lockingType))
        {
            s_typeToMovementPreventionCountDictionary[lockingType]--;

            if (s_typeToMovementPreventionCountDictionary[lockingType] <= 0)
            {
                s_typeToMovementPreventionCountDictionary.Remove(lockingType);
            }
        }

        if (s_typeToMovementPreventionCountDictionary.Count == 0 && s_playerInput != null)
        {
            // There is no longer anything wishing to disable our movement controls.
            // Enable the 'Movement' map.
            s_playerInput.Movement.Enable();
        }
    }


    // Camera Map.
    private static Dictionary<Type, int> s_typeToCameraPreventionCountDictionary = new Dictionary<Type, int>();
    public static void PreventCameraActions(Type lockingType)
    {
        if (!s_typeToCameraPreventionCountDictionary.TryAdd(lockingType, 1))
        {
            // We already have a kvp with key 'lockingType', so increment it instead.
            s_typeToCameraPreventionCountDictionary[lockingType]++;
        }

        if (s_playerInput != null)
        {
            // Disable our 'Camera' map.
            s_playerInput.Camera.Disable();
        }
    }
    public static void RemoveCameraActionPrevention(Type lockingType)
    {
        if (s_typeToCameraPreventionCountDictionary.ContainsKey(lockingType))
        {
            s_typeToCameraPreventionCountDictionary[lockingType]--;

            if (s_typeToCameraPreventionCountDictionary[lockingType] <= 0)
            {
                s_typeToCameraPreventionCountDictionary.Remove(lockingType);
            }
        }

        if (s_typeToCameraPreventionCountDictionary.Count == 0 && s_playerInput != null)
        {
            // There is no longer anything wishing to disable our camera controls.
            // Enable the 'Camera' map.
            s_playerInput.Camera.Enable();
        }
    }


    // Interaction Map.
    private static Dictionary<Type, int> s_typeToInteractionPreventionCountDictionary = new Dictionary<Type, int>();
    public static void PreventInteractionActions(Type lockingType)
    {
        if (!s_typeToInteractionPreventionCountDictionary.TryAdd(lockingType, 1))
        {
            // We already have a kvp with key 'lockingType', so increment it instead.
            s_typeToInteractionPreventionCountDictionary[lockingType]++;
        }

        if (s_playerInput != null)
        {
            // Disable our 'Interaction' map.
            s_playerInput.Interaction.Disable();
        }
    }
    public static void RemoveInteractionActionPrevention(Type lockingType)
    {
        if (s_typeToInteractionPreventionCountDictionary.ContainsKey(lockingType))
        {
            s_typeToInteractionPreventionCountDictionary[lockingType]--;

            if (s_typeToInteractionPreventionCountDictionary[lockingType] <= 0)
            {
                s_typeToInteractionPreventionCountDictionary.Remove(lockingType);
            }
        }

        if (s_typeToInteractionPreventionCountDictionary.Count == 0 && s_playerInput != null)
        {
            // There is no longer anything wishing to disable our interaction controls.
            // Enable the 'Interaction' map.
            s_playerInput.Interaction.Enable();
        }
    }


    // Global Map.
    private static Dictionary<Type, int> s_typeToGlobalPreventionCountDictionary = new Dictionary<Type, int>();
    private static void PreventGlobalActions(Type lockingType)
    {
        if (!s_typeToGlobalPreventionCountDictionary.TryAdd(lockingType, 1))
        {
            // We already have a kvp with key 'lockingType', so increment it instead.
            s_typeToGlobalPreventionCountDictionary[lockingType]++;
        }

        if (s_playerInput != null)
        {
            // Disable our 'Global' map.
            s_playerInput.Global.Disable();
        }
    }
    private static void RemoveGlobalActionPrevention(Type lockingType)
    {
        if (s_typeToGlobalPreventionCountDictionary.ContainsKey(lockingType))
        {
            s_typeToGlobalPreventionCountDictionary[lockingType]--;

            if (s_typeToGlobalPreventionCountDictionary[lockingType] <= 0)
            {
                s_typeToGlobalPreventionCountDictionary.Remove(lockingType);
            }
        }

        if (s_typeToGlobalPreventionCountDictionary.Count == 0 && s_playerInput != null)
        {
            // There is no longer anything wishing to disable our global controls (E.g. Pause Menu).
            // Enable the 'Global' map.
            s_playerInput.Global.Enable();
        }
    }


    // All Maps.
    public static void PreventAllActions(Type lockingType, bool disableGlobalMaps = false)
    {
        PreventActions(lockingType, disableGlobalMaps ? ActionTypes.Everything : ActionTypes.Everything & ~ActionTypes.Global);
    }
    public static void RemoveAllActionPrevention(Type lockingType)
    {
        RemoveActionPrevention(lockingType, ActionTypes.Everything);
    }

    private static void UpdateDisabledState()
    {
        // Movement.
        if (s_typeToMovementPreventionCountDictionary.Count > 0)
            s_playerInput.Movement.Disable();
        else
            s_playerInput.Movement.Enable();

        // Camera.
        if (s_typeToCameraPreventionCountDictionary.Count > 0)
            s_playerInput.Camera.Disable();
        else
            s_playerInput.Camera.Enable();

        // Interaction.
        if (s_typeToInteractionPreventionCountDictionary.Count > 0)
            s_playerInput.Interaction.Disable();
        else
            s_playerInput.Interaction.Enable();

        // Global.
        if (s_typeToGlobalPreventionCountDictionary.Count > 0)
            s_playerInput.Global.Disable();
        else
            s_playerInput.Global.Enable();
    }

    #endregion
}
