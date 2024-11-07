using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    [SerializeField] private Transform _playerCamera;

    [Space(5)]
    [SerializeField] private PlayerInventory _playerInventory;
    [SerializeField] private PlayerFlashlightController _playerFlashlightController;


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
        _playerCamera.localEulerAngles = new Vector3(setupData.CameraXRotation, 0.0f, 0.0f); // Not working - PlayerController conflict?.


        // Flashlight.
        if (setupData.CurrentFlashlightPrefab != null)
        {
            _playerFlashlightController.AddFlashlight(setupData.CurrentFlashlightPrefab);
            // Flashlight battery.
        }


        // Set Collected Items.
        _playerInventory.keyCards = setupData.CollectedKeycardIDs;
    }
    public PlayerSetupData GetCurrentPlayerData()
    {
        PlayerSetupData setupData = new PlayerSetupData();

        // Root Position.
        setupData.RootPosition = _player.position;
        setupData.RootRotation = _player.rotation;

        // Camera Rotation.
        setupData.CameraXRotation = _playerCamera.localEulerAngles.x;


        // Flashlight.
        setupData.CurrentFlashlightPrefab = _playerFlashlightController.CurrentFlashlightPrefab;
        setupData.FlashlightBatteryRemaining = _playerFlashlightController.GetCurrentFlashlightController().GetCurrentBattery();


        // Collected Items.
        setupData.CollectedKeycardIDs = _playerInventory.keyCards;

        return setupData;
    }



    /// <summary> To-do: Remove.</summary>
    public Transform Player => _player;


    [System.Serializable]
    public struct PlayerSetupData
    {
        // Position & Rotation Information.
        public Vector3 RootPosition;
        public Quaternion RootRotation;
        public float CameraXRotation;


        // Flashlight Information.
        public GameObject CurrentFlashlightPrefab;
        public float FlashlightBatteryRemaining;


        // Collected Item Information.
        public int MedkitCount;
        public int FlareCount;
        public List<int> CollectedKeycardIDs;


        public static PlayerSetupData Default => new PlayerSetupData() {
            RootPosition = Vector3.zero,
            RootRotation = Quaternion.identity,
            CameraXRotation = 0.0f,

            CurrentFlashlightPrefab = null,
            FlashlightBatteryRemaining = 100.0f,

            MedkitCount = 0,
            FlareCount = 0,
            CollectedKeycardIDs = new List<int>(),
        };
    }
}
