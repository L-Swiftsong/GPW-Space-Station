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
        public PlayerInventory _playerInventory;

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


        /*public InventorySaveData GetInventorySaveData() => InventorySaveData.FromInventoryData(_playerInventory);
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
                CollectableManager.LoadObtainedCollectables(collectableSaveData.CollectableType.ToSystemType(), collectableSaveData.KeyItemsObtained);
            }
        }*/



        /// <summary> To-do: Remove.</summary>
        public Transform Player => _player;

        public Camera GetPlayerCamera() => _playerMainCamera;
        public Transform GetPlayerCameraTransform() => _playerMainCamera.transform;
    }
}