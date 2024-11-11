using UnityEngine;
using UnityEngine.InputSystem;

public class KeyCard : MonoBehaviour, IInteractable
{
    [SerializeField] private int m_keyCardID;

    private PlayerInventory playerInventory;

    public void Start()
    {
        //References
        playerInventory = FindObjectOfType<PlayerInventory>();
    }

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

        //Add coloured keycard to inventory depending on keycard ID
        if (this.KeyCardID == 0)
        {
            playerInventory.PickupBlueKeyCard();
        }
        else if (this.KeyCardID == 1)
        {
            playerInventory.PickupGreenKeyCard();
        }
        else
        {
            Debug.Log($"Keycard ID has not been assigned in inventory");
        }

        Destroy(this.gameObject);
    }
}