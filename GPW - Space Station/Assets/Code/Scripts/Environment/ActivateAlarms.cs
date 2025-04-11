using UnityEngine;
using Environment.Lighting;

namespace Environment
{
    public class ActivateAlarms : MonoBehaviour
    {
        public bool alarmStarted = false;

        private AudioSource audioSource;
        public bool colliderTriggered = false;

        void Start()
        {
            audioSource = GetComponentInChildren<AudioSource>();
        }

        public void StartAlarms()
        {
            alarmStarted = true;

            audioSource.Play();
        }

        public void StopAlarms()
        {
            alarmStarted = false;

            audioSource.loop = false;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.tag == "Player" && alarmStarted)
            {
                audioSource.loop = false;
            }
        }
    }
}