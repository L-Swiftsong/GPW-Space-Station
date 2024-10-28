using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public List<int> keyCards = new List<int>(); 

    public void AddKeyCard(int keyCardId)
    {
        if (!keyCards.Contains(keyCardId)) 
        {
            keyCards.Add(keyCardId);
            Debug.Log($"Picked up keycard: {keyCardId}");
        }
    }

    public bool HasKeyCard(int keyCardId)
    {
        return keyCards.Contains(keyCardId); 
    }
}
