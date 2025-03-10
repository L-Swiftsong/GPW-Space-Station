using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Interaction;
using Items.KeyItem;
using Items.Collectables;

public class RepairSpotManager : MonoBehaviour
{
    [SerializeField] private List<UseKeyItem> _repairSpots;
    private UseKeyItem _activeRepairSpot;


    void Start()
    {
        foreach (var repairSpot in _repairSpots)
        {
            repairSpot.OnSuccessfulInteraction += HandleSuccessfulInteraction;
            repairSpot.OnFailedInteraction += HandleFailedInteraction;
        }
    }

	private void OnDestroy()
	{
		foreach (var repairSpot in _repairSpots)
        {
            repairSpot.OnSuccessfulInteraction -= HandleSuccessfulInteraction;
            repairSpot.OnFailedInteraction -= HandleFailedInteraction;
        }
	}

    public void InteractWithRepairSpot(IInteractable interactable)
    {
        UseKeyItem repairSpot = interactable as UseKeyItem;

        if (repairSpot != null)
        {
            _activeRepairSpot = repairSpot;
            Debug.Log($"Interacted with repair spot {repairSpot.gameObject.name}");
        }
    }

    public void TryUseKeyItem(KeyItemData keyItemData)
    {
        if (_activeRepairSpot != null)
        {
            if (_activeRepairSpot.IsKeyItemCorrect(keyItemData))
            {
                _activeRepairSpot.TryUseKeyItem(keyItemData);
            }
            else
            {
                Debug.Log("wrong key item");
                _activeRepairSpot.FailInteraction();
            }
        }
        else
        {
            Debug.Log("no active repair spot");
        }
    }

    private void HandleSuccessfulInteraction()
    {
        Debug.Log("Repair spot interaction succeeded.");
    }
    private void HandleFailedInteraction()
    {
        Debug.Log("Repair spot interaction failed.");
    }
}
