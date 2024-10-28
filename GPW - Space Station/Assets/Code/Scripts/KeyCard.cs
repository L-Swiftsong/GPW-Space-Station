using UnityEngine;
using UnityEngine.InputSystem;

public class KeyCard : MonoBehaviour, IInteractable
{
    [SerializeField]
    private int keyCardId;

    public int KeyCardId 
    {
        get => keyCardId; 
        set => keyCardId = value;
    }

    [SerializeField]
    private Color keyCardColour; 

    public Color KeyCardColour 
    {
        get => keyCardColour;
        set => keyCardColour = value;
    }

    private void Start()
    {
        Renderer renderer = GetComponent<Renderer>();

        if (renderer != null)
        {
            Material materialInstance = new Material(renderer.sharedMaterial);

            renderer.material = materialInstance;            
            
            materialInstance.SetColor("_Color", keyCardColour);
        }
    }


    public void Interact(PlayerInteraction playerInteraction)
    {
        playerInteraction.Inventory.AddKeyCard(this.KeyCardId);
        Debug.Log($"Picked up {this.KeyCardId}");
        Destroy(this.gameObject);
    }
}