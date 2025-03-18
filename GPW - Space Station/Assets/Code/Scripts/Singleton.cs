using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Singleton<T> : MonoBehaviour where T : Component
{
    private static T _instance = null;

    public static bool HasInstance => Instance != null;
    public static T Instance
    {
        get => _instance;
        set
        {
            if (_instance != null)
            {
                Debug.LogError($"Error: A SceneLoader instance already exists: {_instance.name}.\n Destroying {value.name}", value);
                Destroy(value);
                return;
            }

            _instance = value;
        }
    }


    protected virtual void Awake()
    {
        Instance = this as T;
    }
}
