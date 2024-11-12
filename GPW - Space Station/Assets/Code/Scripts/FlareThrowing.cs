using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class FlareThrowing : MonoBehaviour
{
    [Header("Flare Throwing Settings")]
    [SerializeField] private Transform _flareOrigin;
    [SerializeField] private Vector3 _localStartingVelocity;

    [Space(5)]
    [SerializeField] private GameObject _flarePrefab;

    

    private const string FLARE_CONTAINER_NAME = "SignalFlarePool";
    private static Transform s_flareContainer;
    private Transform _flareContainer
    {
        get
        {
            if (s_flareContainer == null)
            {
                s_flareContainer = new GameObject(FLARE_CONTAINER_NAME).transform;
            }

            return s_flareContainer;
        }
    }

    private ObjectPool<GameObject> m_flarePool; // Lazily initialised.
    private ObjectPool<GameObject> _flarePool
    {
        get
        {
            if (m_flarePool == null)
            {
                // Setup the flare object pool.
                m_flarePool = new ObjectPool<GameObject>(createFunc: CreatePooledFlare,
                    actionOnGet: OnGetPooledFlare, actionOnRelease: OnReleasePooledFlare,
                    defaultCapacity: _maxFlares, maxSize: MAX_FLARES_IN_SCENE);
            }

            // Return the object pool.
            return m_flarePool;
        }
    }


    [Header("Flare Settings")]
    [SerializeField] private int _maxFlares = 20;
    private int _remainingFlares;


    private const int MAX_FLARES_IN_SCENE = 50;

    private void Awake()
    {
        _remainingFlares = _maxFlares;
    }
    private void OnEnable()
    {
        //PlayerInput.OnThrowFlarePerformed += PlayerInput_OnThrowFlarePerformed;
    }
    private void OnDisable()
    {
        //PlayerInput.OnThrowFlarePerformed -= PlayerInput_OnThrowFlarePerformed;
    }


    #region Object Pool Functions

    private GameObject CreatePooledFlare()
    {
        // Create & setup the flare instance.
        GameObject flareInstance = Instantiate<GameObject>(_flarePrefab, _flareOrigin.position, Quaternion.identity, parent: _flareContainer);

        // Return the flare instance.
        return flareInstance;
    }
    private void OnGetPooledFlare(GameObject flare)
    {
        flare.transform.SetPositionAndRotation(_flareOrigin.position, _flareOrigin.rotation);
        flare.SetActive(true);
    }
    private void OnReleasePooledFlare(GameObject flare)
    {
        flare.SetActive(false);
    }

    #endregion


    private void PlayerInput_OnThrowFlarePerformed() => TryThrowFlare();
    

    private bool TryThrowFlare()
    {
        if (_remainingFlares <= 0)
        {
            return false;
        }

        _remainingFlares--;
        ThrowFlare();
        return true;
    }
    private void ThrowFlare()
    {
        // Get a flare from our object pool.
        GameObject flareInstance = _flarePool.Get();

        // Set the flare's initial velocity.
        Rigidbody flareRB = flareInstance.GetComponent<Rigidbody>();
        flareRB.velocity = _flareOrigin.TransformDirection(_localStartingVelocity);

        // Release the flare to the object pool once its lifetime has elapsed.
        flareInstance.GetComponent<Flare>().OnFlareLifetimeElapsed += FlareInstance_OnFlareLifetimeElapsed;
    }

    private void FlareInstance_OnFlareLifetimeElapsed(Flare instance)
    {
        // Unsubscribe from event.
        instance.OnFlareLifetimeElapsed -= FlareInstance_OnFlareLifetimeElapsed;

        // Intantiate flare gfx object?

        // Release into the object pool.
        _flarePool.Release(instance.gameObject);
    }


    public bool AddFlare()
    {
        if (_remainingFlares >= _maxFlares)
        {
            // We are at our maximum number of flares.
            return false;
        }

        _remainingFlares++;
        return true;
    }
    public int AddFlares(int flaresToAdd)
    {
        if (_remainingFlares >= _maxFlares)
        {
            // We are at our maximum number of flares and cannot add any more.
            return flaresToAdd;
        }
        else if (_remainingFlares + flaresToAdd > _maxFlares)
        {
            // We cannot add all of these new flares, but can add some.
            flaresToAdd = _maxFlares - _remainingFlares;
            _remainingFlares = _maxFlares;
            return flaresToAdd;
        }
        else
        {
            // We can add all of these flares.
            _remainingFlares += flaresToAdd;
            return 0;
        }
    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawRay(_flareOrigin.position, _flareOrigin.TransformDirection(_localStartingVelocity.normalized));
    }
}
