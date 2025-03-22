using UnityEngine;
using Entities.Player;

namespace Saving
{
    [System.Serializable]
    public class PlayerData
    {
        // Position & Rotation Information.
        public Vector3 RootPosition;
        public float YRotation;
        public float CameraXRotation;

        public MovementState MovementState;


        public CollectableSaveData CollectableSaveData;


        public void LoadToPlayer(PlayerController playerController)
        {
            playerController.transform.position = this.RootPosition;

            playerController.SetYRotation(this.YRotation);
            playerController.SetCameraRotation(this.CameraXRotation);
            
            Physics.SyncTransforms();
            
            playerController.InitialiseMovementState(this.MovementState);
        }
        public static PlayerData CreateFromPlayer(PlayerController playerController)
        {
            PlayerData playerData = new PlayerData();


            playerData.RootPosition = playerController.transform.position;

            playerData.YRotation = playerController.GetYRotation();
            playerData.CameraXRotation = playerController.GetCameraRotation();

            playerData.MovementState = playerController.GetCurrentMovementState();


            return playerData;
        }
    }
}