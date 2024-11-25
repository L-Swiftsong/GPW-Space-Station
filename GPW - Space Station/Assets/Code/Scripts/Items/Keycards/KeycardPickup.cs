using UnityEngine;
using Interaction;

namespace Items.Keycards
{
    public class KeycardPickup : MonoBehaviour, IInteractable
    {
        [SerializeField] private int m_securityLevel;

        public void SetupKeycard(int securityLevel) => m_securityLevel = securityLevel;

        public void Interact(PlayerInteraction playerInteraction)
        {
            playerInteraction.Inventory.GetKeycardDecoder().SetSecurityLevel(m_securityLevel);
            Debug.Log($"Picked up {m_securityLevel}");

            Destroy(this.gameObject);
        }
    }
}