using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flare : MonoBehaviour
{
    [SerializeField] private ParticleSystem _flareParticleSystem;
    [SerializeField] private float _particleLifetime;

    private Coroutine _lifetimeCoroutine;


    public event System.Action<Flare> OnFlareLifetimeElapsed;


    private void OnEnable()
    {
        _lifetimeCoroutine = StartCoroutine(FlareLifetime());
    }
    private void OnDisable()
    {
        if(_lifetimeCoroutine != null)
        {
            StopCoroutine(_lifetimeCoroutine);
        }
    }
    private IEnumerator FlareLifetime()
    {
        _flareParticleSystem.Play();

        yield return new WaitForSeconds(_particleLifetime);

        OnFlareLifetimeElapsed?.Invoke(this);
        _flareParticleSystem.Stop();
    }
}
