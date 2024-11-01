using UnityEngine;
using UnityEngine.InputSystem;

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
        playerInteraction.Inventory.AddKeyCard(this.KeyCardID);
        Debug.Log($"Picked up {this.KeyCardID}");
        Destroy(this.gameObject);
    }
}