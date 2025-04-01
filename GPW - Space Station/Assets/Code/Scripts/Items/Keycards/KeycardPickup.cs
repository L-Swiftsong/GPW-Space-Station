using UnityEngine;
using Interaction;
using Audio;

namespace Items.Keycards
{
    public class KeycardPickup : ItemPickup
    {
        [SerializeField] private int m_securityLevel;

        public void SetupKeycard(int securityLevel) => m_securityLevel = securityLevel;


        protected override bool PerformInteraction(PlayerInteraction interactingScript)
        {
            interactingScript.Inventory.GetKeycardDecoder().SetSecurityLevel(m_securityLevel);
            Debug.Log($"Picked up {m_securityLevel}");

            return true;
        }
    }
}