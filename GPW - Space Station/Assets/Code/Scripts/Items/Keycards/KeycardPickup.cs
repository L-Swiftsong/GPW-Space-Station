using UnityEngine;
using Interaction;
using Audio;

namespace Items.Keycards
{
    public class KeycardPickup : ItemPickup
    {
        [SerializeField] private int m_securityLevel;
        [SerializeField] private AudioClip pickupSound; // Assign in Inspector
        [SerializeField] private AudioSource audioSource;

        private void Awake()
        {
            //audioSource.playOnAwake = false; // Prevent unintended playback
        }

        public void SetupKeycard(int securityLevel) => m_securityLevel = securityLevel;


        protected override bool PerformInteraction(PlayerInteraction interactingScript)
        {
            SFXManager.Instance.PlayClipAtPosition(pickupSound, transform.position, 1, 1, 0.1f);
            
            interactingScript.Inventory.GetKeycardDecoder().SetSecurityLevel(m_securityLevel);
             //audioSource.PlayOneShot(pickupSound);
            Debug.Log($"Picked up {m_securityLevel}");

            return true;
        }
    }
}