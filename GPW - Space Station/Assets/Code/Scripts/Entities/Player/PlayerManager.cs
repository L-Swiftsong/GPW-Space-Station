using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Items;
using Saving;
using Items.Collectables;

namespace Entities.Player
{
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

        public CameraFocusLook CameraFocusLook { get; private set; }
        public CameraShake CameraShake { get; private set; }


        private void Awake()
        {
            Instance = this;
            CameraFocusLook = Player.GetComponent<CameraFocusLook>();
            CameraShake = Player.GetComponentInChildren<CameraShake>();
        }


        public void SetPlayerPositionAndRotation(Vector3 desiredPosition, float desiredRotation)
        {
            _player.position = desiredPosition;
            _player.GetComponent<PlayerController>().SetYRotation(desiredRotation);
            Physics.SyncTransforms();
        }
        public void LoadFromPlayerData(PlayerSaveData setupData)
        {
            PlayerController playerController = _player.GetComponent<PlayerController>();

            // Root Position.
            _player.position = setupData.RootPosition;
            playerController.SetYRotation(setupData.YRotation);
            Physics.SyncTransforms();

            // Camera Rotation.
            playerController.SetCameraRotation(setupData.CameraXRotation);


            // Standing State.
            MovementState startingMovementState = setupData.PlayerStandingState switch {
                PlayerSaveData.StandingState.Crouching => MovementState.Crouching,
                PlayerSaveData.StandingState.Crawling => MovementState.Crawling,
                _ => MovementState.Walking,
            };
            playerController.InitialiseMovementState(startingMovementState);
        }
        public PlayerSaveData GetCurrentPlayerData()
        {
            PlayerSaveData setupData = new PlayerSaveData();
            PlayerController playerController = _player.GetComponent<PlayerController>();

            // Root Position.
            setupData.RootPosition = _player.position;
            setupData.YRotation = playerController.GetYRotation();

            // Camera Rotation.
            setupData.CameraXRotation = playerController.GetCameraRotation();


            // Standing State.
            setupData.PlayerStandingState = playerController.GetCurrentMovementState() switch {
                MovementState.Crouching => PlayerSaveData.StandingState.Crouching,
                MovementState.Crawling => PlayerSaveData.StandingState.Crawling,
                _ => PlayerSaveData.StandingState.Standing,
            };


            // Return the filled PlayerSetupData.
            return setupData;
        }


        public InventorySaveData GetInventorySaveData() => InventorySaveData.FromInventoryData(_playerInventory);
        public void LoadInventorySaveData(InventorySaveData saveData)
        {
            // Player Items.
            _playerInventory.SetHasObtainedFlashlight(saveData.FlashlightObtained, saveData.FlashlightBattery);
            _playerInventory.SetHasObtainedKeycardDecoder(saveData.DecoderObtained, saveData.DecoderSecurityLevel);
            _playerInventory.SetMedkits(saveData.MedkitCount);

            // Key Items.


            // Collectables.
            CollectableManager.PrepareForLoad();
            foreach (CollectableSaveData collectableSaveData in saveData.CollectablesSaveData)
            {
                CollectableManager.LoadObtainedCollectables(collectableSaveData.CollectableType.ToSystemType(), collectableSaveData.CollectablesObtained);
            }
        }



        /// <summary> To-do: Remove.</summary>
        public Transform Player => _player;

        public Camera GetPlayerCamera() => _playerMainCamera;
        public Transform GetPlayerCameraTransform() => _playerMainCamera.transform;
    }
}