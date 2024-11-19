using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Items;

public class PlayerManager : MonoBehaviour
{
    #region Singleton

    private static PlayerManager _instance;
    public static PlayerManager Instance
    {
        get => _instance;
        private set 
        {
            if (_instance != null)
            {
                Debug.LogError("Error: A PlayerManager instance already exists: " + Instance.name + ". \nDestroying " + value.name);
                Destroy(value.gameObject);
                return;
            }

            _instance = value;
        }
    }

    #endregion
    public static bool Exists => _instance != null;



    [Header("References")]
    [SerializeField] private Transform _player;
    [SerializeField] private Camera _playerMainCamera;

    [Space(5)]
    [SerializeField] private PlayerInventory _playerInventory;


    private void Awake()
    {
        Instance = this;
    }


    public void SetPlayerPositionAndRotation(Vector3 desiredPosition, Vector3 desiredRotationEulerAngles)
    {
        _player.SetPositionAndRotation(desiredPosition, Quaternion.Euler(desiredRotationEulerAngles));
        Physics.SyncTransforms();
    }
    public void LoadFromPlayerData(PlayerSetupData setupData)
    {
        // Root Position.
        _player.position = setupData.RootPosition;
        _player.rotation = setupData.RootRotation;
        Physics.SyncTransforms();

        // Camera Rotation.
        _playerMainCamera.transform.localEulerAngles = new Vector3(setupData.CameraXRotation, 0.0f, 0.0f); // Not working - PlayerController conflict?.


        // Standing State.
        PlayerController.MovementState startingMovementState = setupData.PlayerStandingState switch {
            PlayerSetupData.StandingState.Crouching => PlayerController.MovementState.Crouching,
            PlayerSetupData.StandingState.Crawling => PlayerController.MovementState.Crawling,
            _ => PlayerController.MovementState.Walking,
        };
        _player.GetComponent<PlayerController>().InitialiseMovementState(startingMovementState);


        // Setup the Player's Inventory.
        _playerInventory.SetHasObtainedFlashlight(setupData.HasFlashlight, setupData.FlashlightBattery);
        _playerInventory.SetHasObtainedKeycardDecoder(setupData.HasDecoder, setupData.DecoderLevel);
    }
    public PlayerSetupData GetCurrentPlayerData()
    {
        PlayerSetupData setupData = new PlayerSetupData();

        // Root Position.
        setupData.RootPosition = _player.position;
        setupData.RootRotation = _player.rotation;

        // Camera Rotation.
        setupData.CameraXRotation = _playerMainCamera.transform.localEulerAngles.x;


        // Standing State.
        setupData.PlayerStandingState = _player.GetComponent<PlayerController>().GetCurrentMovementState() switch {
            PlayerController.MovementState.Crouching => PlayerSetupData.StandingState.Crouching,
            PlayerController.MovementState.Crawling => PlayerSetupData.StandingState.Crawling,
            _ => PlayerSetupData.StandingState.Standing,
        };


        // Save Player Inventory.
        setupData.HasFlashlight = _playerInventory.HasFlashlight();
        setupData.FlashlightBattery = _playerInventory.GetFlashlightBattery();

        setupData.HasDecoder = _playerInventory.HasKeycardDecoder();
        setupData.DecoderLevel = _playerInventory.GetDecoderSecurityLevel();


        // Return the filled PlayerSetupData.
        return setupData;
    }



    /// <summary> To-do: Remove.</summary>
    public Transform Player => _player;

    public Camera GetPlayerCamera() => _playerMainCamera;
    public Transform GetPlayerCameraTransform() => _playerMainCamera.transform;


    [System.Serializable]
    public struct PlayerSetupData
    {
        // Position & Rotation Information.
        public Vector3 RootPosition;
        public Quaternion RootRotation;
        public float CameraXRotation;

        [System.Serializable] public enum StandingState { Standing, Crouching, Crawling };
        public StandingState PlayerStandingState;


        // Inventory Information.
        public bool HasFlashlight;
        public float FlashlightBattery;
        public bool HasDecoder;
        public int DecoderLevel;


        public static PlayerSetupData Default => new PlayerSetupData() {
            RootPosition = Vector3.zero,
            RootRotation = Quaternion.identity,
            CameraXRotation = 0.0f,

            PlayerStandingState = StandingState.Standing,

            HasFlashlight = false,
            FlashlightBattery = 0.0f,
            HasDecoder = false,
            DecoderLevel = 0,
        };
    }
}
