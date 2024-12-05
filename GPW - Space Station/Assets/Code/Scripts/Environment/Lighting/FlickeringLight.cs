using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Environment.Lighting
{
    public class FlickeringLight : MonoBehaviour
    {
        [SerializeField] private Light _light;
        private Coroutine _flickerCoroutine;


        [Header("Settings")]
        [SerializeField] private float _minIntensity;
        [SerializeField] private float _maxIntensity;
        
        [Space(5)]
        [SerializeField] private float _minDelay = 0.01f;
        [SerializeField] private float _maxDelay = 0.2f;


        private void Awake() => _light.intensity = _minIntensity + ((_maxIntensity - _minIntensity) / 2.0f);
        private void OnEnable()
        {
            if (_flickerCoroutine == null)
            {
                StartCoroutine(Flicker());
            }
        }
        private void OnDisable()
        {
            if (_flickerCoroutine != null)
            {
                StopCoroutine(_flickerCoroutine);
            }
        }

        private IEnumerator Flicker()
        {
            while(true)
            {
                _light.intensity = Random.Range(_minIntensity, _maxIntensity);
                yield return new WaitForSeconds(Random.Range(_minDelay, _maxDelay));
            }
        }
    }
}