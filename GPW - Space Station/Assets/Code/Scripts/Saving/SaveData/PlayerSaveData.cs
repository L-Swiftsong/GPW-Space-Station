using UnityEngine;

[System.Serializable]
public struct PlayerSaveData
{
    // Position & Rotation Information.
    public Vector3 RootPosition;
    public float YRotation;
    public float CameraXRotation;

    [System.Serializable] public enum StandingState { Standing, Crouching, Crawling };
    public StandingState PlayerStandingState;


    public static PlayerSaveData Default => new PlayerSaveData()
    {
        RootPosition = Vector3.zero,
        YRotation = 0.0f,
        CameraXRotation = 0.0f,

        PlayerStandingState = StandingState.Standing
    };
}