using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Environment.Lighting
{
    public class MotionActivatedLight : MonoBehaviour
    {
        public Light[] lightSources; 
        public float lightDuration = 3f;

        private void Start()
        {
            foreach (Light light in lightSources)
            {
                if (light != null)
                {
                    light.enabled = false;
                }
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                ActivateLights();
            }
        }

        private void ActivateLights()
        {
            foreach (Light light in lightSources)
            {
                if (light != null)
                {
                    light.enabled = true;  
                    Invoke(nameof(TurnOffLights), lightDuration); 
                }
            }
        }

        private void TurnOffLights()
        {
            foreach (Light light in lightSources)
            {
                if (light != null)
                {
                    light.enabled = false;
                }
            }
        }
    }
}