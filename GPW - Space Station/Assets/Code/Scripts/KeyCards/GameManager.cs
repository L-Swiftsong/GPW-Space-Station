using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public List<KeyCard> keyCardsInScene; 
    public List<Door> doorsInScene;

    private void Start()
    {
        keyCardsInScene = new List<KeyCard>(FindObjectsOfType<KeyCard>());
        doorsInScene = new List<Door>(FindObjectsOfType<Door>());

        AssignKeyCardIds(keyCardsInScene);
        AssignDoorIds(doorsInScene);
        AssignKeyCardColours(keyCardsInScene);
        AssignDoorColours(doorsInScene);

        foreach (KeyCard keyCard in keyCardsInScene)
        {
            Debug.Log($"Key Card ID: {keyCard.KeyCardId}, Colour: {keyCard.KeyCardColour}");
        }

        foreach (Door door in doorsInScene)
        {
            Debug.Log($"Door ID: {door.RequiredKeyCardId}, Colour: {door.DoorColour}");
        }

    }

    private void AssignKeyCardIds(List<KeyCard> keyCards)
    {
        List<int> assignedIds = new List<int>(); 
        int currentId = 1; 

        foreach (KeyCard keyCard in keyCards)
        {
            if (keyCard.KeyCardId == 0) 
            {
                keyCard.KeyCardId = currentId;
            }

            while (assignedIds.Contains(keyCard.KeyCardId))
            {
                currentId++; 
                keyCard.KeyCardId = currentId; 
            }

            assignedIds.Add(keyCard.KeyCardId);
            currentId++; 
        }
    }

    private void AssignDoorIds(List<Door> doors)
    {
        List<int> assignedIds = new List<int>();
        int currentId = 1; 

        foreach (Door door in doors)
        {
            if (door.RequiredKeyCardId == 0) 
            {
                door.RequiredKeyCardId = currentId; 
            }

            while (assignedIds.Contains(door.RequiredKeyCardId))
            {
                currentId++; 
                door.RequiredKeyCardId = currentId; 
            }

            assignedIds.Add(door.RequiredKeyCardId); 
            currentId++; 
        }
    }

    private void AssignKeyCardColours(List<KeyCard> keyCards)
    {
        Color[] defaultColours = { Color.red, Color.green, Color.blue, Color.yellow }; 

        for (int i = 0; i < keyCards.Count; i++)
        {
            if (keyCards[i].KeyCardColour == Color.clear) 
            {       
                keyCards[i].KeyCardColour = defaultColours[i % defaultColours.Length]; 

                Renderer renderer = keyCards[i].GetComponent<Renderer>();

                if (renderer != null)
                {
                    renderer.material.SetColor("_Color", keyCards[i].KeyCardColour);
                }
            }
        }
    }

    private void AssignDoorColours(List<Door> doors)
    {
        Color[] defaultColours = { Color.red, Color.green, Color.blue, Color.yellow }; 

        for (int i = 0; i < doors.Count; i++)
        {
            if (doors[i].DoorColour == Color.clear) 
            {
                doors[i].DoorColour = defaultColours[i % defaultColours.Length]; 

                Renderer renderer = doors[i].GetComponent<Renderer>();

                if (renderer != null)
                {
                    renderer.material.SetColor("_Color", doors[i].DoorColour);
                }
            }
        }
    }

}
