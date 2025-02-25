using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Environment.Lighting
{
    public class LightFlicker : MonoBehaviour
    {
        [SerializeField] private Light[] _lights;

        [Space(5)]
        [SerializeField] private float _minIntensity = 0.1f;
        [SerializeField] private float _maxIntensity = 0.35f;

        [Space(5)]
        [SerializeField] private AnimationCurve _intensityFrequencyCurve = AnimationCurve.Linear(0.0f, 0.0f, 1.0f, 1.0f);
        [SerializeField] private float _minFlickerTime = 0.05f;
        [SerializeField] private float _maxFlickerTime = 0.1f;


        private Coroutine _lightFlickerCoroutine;
        private void OnEnable()
        {
            if (_lightFlickerCoroutine == null)
            {
                _lightFlickerCoroutine = StartCoroutine(HandleLightFlickering());
            }
        }


        private IEnumerator HandleLightFlickering()
        {
            while (true)
            {
                FlickerLights();
                yield return new WaitForSeconds(Random.Range(_minFlickerTime, _maxFlickerTime));
            }
        }

        private void FlickerLights()
        {
            float newIntensity = Mathf.Lerp(_minIntensity, _maxIntensity, _intensityFrequencyCurve.Evaluate(Random.Range(0.0f, 1.0f)));
            for (int i = 0; i < _lights.Length; ++i)
            {
                _lights[i].intensity = newIntensity;
            }
        }



#if UNITY_EDITOR

        private void Reset()
        {
            _lights = GetComponentsInChildren<Light>();

            float averageIntensity = 0;
            for(int i = 0; i < _lights.Length; ++i)
            {
                averageIntensity += _lights[i].intensity;

                _lights[i].lightmapBakeType = LightmapBakeType.Realtime;
            }
            averageIntensity /= _lights.Length;


            float defaultIntensityRange = 0.3f;
            _minIntensity = averageIntensity - (defaultIntensityRange / 2.0f);
            _maxIntensity = averageIntensity + (defaultIntensityRange / 2.0f);
        }

#endif
    }
}
