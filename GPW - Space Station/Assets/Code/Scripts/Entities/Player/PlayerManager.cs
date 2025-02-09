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
            MovementState startingMovementState = setupData.PlayerStandingState switch {
                PlayerSetupData.StandingState.Crouching => MovementState.Crouching,
                PlayerSetupData.StandingState.Crawling => MovementState.Crawling,
                _ => MovementState.Walking,
            };
            _player.GetComponent<PlayerController>().InitialiseMovementState(startingMovementState);
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
                MovementState.Crouching => PlayerSetupData.StandingState.Crouching,
                MovementState.Crawling => PlayerSetupData.StandingState.Crawling,
                _ => PlayerSetupData.StandingState.Standing,
            };


            // Return the filled PlayerSetupData.
            return setupData;
        }


        public ItemSaveData GetInventorySaveData() => ItemSaveData.FromInventoryData(_playerInventory);
        public void LoadInventorySaveData(ItemSaveData saveData)
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


        [System.Serializable]
        public struct PlayerSetupData
        {
            // Position & Rotation Information.
            public Vector3 RootPosition;
            public Quaternion RootRotation;
            public float CameraXRotation;

            [System.Serializable] public enum StandingState { Standing, Crouching, Crawling };
            public StandingState PlayerStandingState;


            public static PlayerSetupData Default => new PlayerSetupData() {
                RootPosition = Vector3.zero,
                RootRotation = Quaternion.identity,
                CameraXRotation = 0.0f,

                PlayerStandingState = StandingState.Standing
            };
        }
    }
}