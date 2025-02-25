using UnityEngine;
using Interaction;

namespace Items.Keycards
{
    public class KeycardPickup : ItemPickup
    {
        [SerializeField] private int m_securityLevel;
        [SerializeField] private AudioClip pickupSound; // Assign in Inspector
        private AudioSource audioSource;

        private void Awake()
        {
            // Ensure there's an AudioSource component
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
            }

            audioSource.playOnAwake = false; // Prevent unintended playback
        }

        public void SetupKeycard(int securityLevel) => m_securityLevel = securityLevel;


        protected override bool PerformInteraction(PlayerInteraction interactingScript)
        {

            if (pickupSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(pickupSound);
            }

            interactingScript.Inventory.GetKeycardDecoder().SetSecurityLevel(m_securityLevel);
            Debug.Log($"Picked up {m_securityLevel}");

            return true;
        }
    }
}