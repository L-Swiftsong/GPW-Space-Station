using Environment.Lighting;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightManager : MonoBehaviour
{
    public static LightManager Instance {  get; private set; }

    [Header("Lights")]
    [SerializeField] private List<Light> _activeLights = new List<Light>();
    private List<LightFlicker> _flickeringLights = new List<LightFlicker>();

    [Header("Flicker Settings")]
    [SerializeField] private float flickerDuration = 3f;
    [SerializeField] private float minFlickerInterval = 0.1f;
    [SerializeField] private float maxFlickerInterval = 0.3f;

	private void Awake()
	{
		Instance = this;
	}

	public void AddLights(Light light)
    {
        if (!_activeLights.Contains(light))
        {
            _activeLights.Add(light);
        }
    }

    public void RemoveLights(Light light)
    {
        _activeLights.Remove(light);
    }

    public void AddFlickeringLights(LightFlicker lightFlicker)
    {
        if (!_flickeringLights.Contains(lightFlicker))
            _flickeringLights.Add(lightFlicker);
    }

    public void RemoveFlickeringLights(LightFlicker lightFlicker)
    {
        _flickeringLights.Remove(lightFlicker);
    }

    public void StartLightsOut()
    {
        //foreach (var flicker in _flickeringLights)
        //    flicker.enabled = false;

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

    public void TurnOnLights()
    {
        foreach (Light light in _activeLights)
        {
            light.enabled = true;
        }


        //foreach (LightFlicker flicker in _flickeringLights)
        //    flicker.enabled = true;
    }
}
