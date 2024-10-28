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


    private void Awake()
    {
        Instance = this;
    }


    public void SetPlayerPositionAndRotation(Vector3 desiredPosition, Vector3 desiredRotationEulerAngles)
    {
        _player.SetPositionAndRotation(desiredPosition, Quaternion.Euler(desiredRotationEulerAngles));
        Physics.SyncTransforms();
    }



    /// <summary> To-do: Remove.</summary>
    public Transform Player => _player;
}
