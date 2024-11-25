using UnityEngine;
using Environment.Lighting;

namespace Environment
{
    public class ActivateAlarms : MonoBehaviour
    {
        public bool alarmStarted = false;

        [Header("Alarm Trigger")]
        [SerializeField] GameObject keyCard;

        private AudioSource audioSource;
        public bool colliderTriggered = false;

        void Start()
        {
            audioSource = GetComponentInChildren<AudioSource>();
        }

    
        void Update()
        {
            if (keyCard == null && !alarmStarted)
            {
                alarmStarted = true;

                audioSource.Play();
            }
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