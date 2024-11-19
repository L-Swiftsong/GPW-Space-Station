using UnityEngine;

public class KeyCard : MonoBehaviour, IInteractable
{
    [SerializeField] private int m_keyCardID;

    public void SetupKeycard(int id) => m_keyCardID = id;

    public void Interact(PlayerInteraction playerInteraction)
    {
        playerInteraction.Inventory.GetKeycardDecoder().SetSecurityLevel(m_keyCardID);
        Debug.Log($"Picked up {m_keyCardID}");

        Destroy(this.gameObject);
    }
}