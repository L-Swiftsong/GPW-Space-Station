using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightManager : MonoBehaviour
{
    [Header("Lights")]
    [SerializeField]private Light[] _allLights;
    [SerializeField] private List<Light> _activeLights = new List<Light>();

    [Header("Flicker Settings")]
    [SerializeField] private float flickerDuration = 3f;
    [SerializeField] private float minFlickerInterval = 0.1f;
    [SerializeField] private float maxFlickerInterval = 0.3f;

    void Start()
    {
        _allLights = FindObjectsOfType<Light>();

        foreach (Light light in _allLights)
        {
            if(light.enabled)
            {
                _activeLights.Add(light);
            }
        }
    }

    public void StartLightsOut()
    {
        StartCoroutine(FlickerAndBlackout());
    }

    private IEnumerator FlickerAndBlackout()
    {
        float elapsedTime = 0f;

        while (elapsedTime < flickerDuration)
        {
            foreach (Light light in _activeLights)
            {
                if (Random.value > 0.5f)
                {
                    light.enabled = !light.enabled;
                }
            }

            float flickerTime = Random.Range(minFlickerInterval, maxFlickerInterval);
            yield return new WaitForSeconds(flickerTime);
            elapsedTime += flickerTime;
        }

        foreach (Light light in _activeLights)
        {
            light.enabled = false;
        }
    }

}
