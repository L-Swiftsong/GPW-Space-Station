using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightRegister : MonoBehaviour
{
	private Light[] _lights;

	private void Awake()
	{
		_lights = GetComponentsInChildren<Light>(true);
	}

	private void OnEnable()
	{
		if (LightManager.Instance != null)
		{
			foreach (var light in _lights)
			{
				LightManager.Instance.AddLights(light);
			}
		}
	}

	private void OnDisable()
	{
		if(LightManager.Instance != null)
		{
			foreach (var light in _lights)
			{
				LightManager.Instance.RemoveLights(light);
			}
		}
	}
}
