using UnityEngine;

public class KeyCard : MonoBehaviour
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
}



