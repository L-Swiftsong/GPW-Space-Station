using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlarmLight : MonoBehaviour
{
    private bool alarmInProgress = false;
    [SerializeField] private ActivateAlarms activateAlarms;
    private Light[] alarmLights;
    [SerializeField] float flashFrequency = 1f; 

    void Start()
    {
        alarmLights = GetComponentsInChildren<Light>();

        foreach (Light light in alarmLights)
        {
            light.enabled = false;
        }
    }


    void Update()
    {
        if (activateAlarms.alarmStarted && !alarmInProgress)
        {
            alarmInProgress = true;
            StartCoroutine(AlarmSequence());
        }
    }

    IEnumerator AlarmSequence()
    {
        while (alarmInProgress)
        {
            foreach (Light light in alarmLights)
            {
                light.enabled = true;
            }

            yield return new WaitForSeconds(flashFrequency);

            foreach (Light light in alarmLights)
            {
                light.enabled = false;
            }

            yield return new WaitForSeconds(flashFrequency);
        }
    }
}
