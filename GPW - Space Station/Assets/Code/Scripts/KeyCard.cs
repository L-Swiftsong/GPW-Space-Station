using UnityEngine;
using UnityEngine.InputSystem;
using Inventory;

public class KeyCard : MonoBehaviour, IInteractable
{
    [SerializeField] private int m_keyCardID;

    public int KeyCardID
    {
        get => m_keyCardID;
        set => m_keyCardID = value;
    }

    public void SetupKeycard(int id, Material material)
    {
        this.KeyCardID = id;
        
        Renderer renderer = GetComponent<Renderer>();
        renderer.material = material;
    }

    public void Interact(PlayerInteraction playerInteraction)
    {
        playerInteraction.Inventory.AddKeycard(this.KeyCardID);
        Debug.Log($"Picked up {this.KeyCardID}");

        Destroy(this.gameObject);
    }
}