using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Interaction;
using Items.Collectables;
using Audio;
using System;
using UI.ItemDisplay;

public class RepairSpotManager : ProtectedSingleton<RepairSpotManager>
{
    [Header("References")]
    [SerializeField] private List<UseKeyItem> _repairSpots;
    private UseKeyItem _activeRepairSpot;

    [Header("Audio")]
	[SerializeField] private AudioClip incorrectItemSound;
	[SerializeField] private AudioSource audioSource;

    [Header("Win")]
    private int _successfulRepairs = 0;
    private int _totalRepairsNeeded = 3;

    public static event Action OnAllRepairsCompleted;

	void Start()
    {
        _totalRepairsNeeded = _repairSpots.Count;

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
                CompletedRepairs();
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
		Debug.Log("Repair spot interaction succeeded. " + _successfulRepairs);
		CheckForWinCondition();
    }
    private void HandleFailedInteraction()
    {
		SFXManager.Instance.PlayClipAtPosition(incorrectItemSound, transform.position, 1, 1, 0.5f);
		Debug.Log("Repair spot interaction failed.");
    }

    private void CompletedRepairs()
    {
        _successfulRepairs++;
		Debug.Log($"Completed repair spots: {_successfulRepairs}");

        CheckForWinCondition();
	}

    private void CheckForWinCondition()
    {
        if (_successfulRepairs == _totalRepairsNeeded)
        {
			Debug.Log("All repair spots completed! You win!");
            OnAllRepairsCompleted?.Invoke();
		}
    }

    public static bool[] GetRepairStates() => HasInstance ? Instance.GetRepairStates_Instance() : new bool[0];
    private bool[] GetRepairStates_Instance()
    {
        bool[] repairStates = new bool[_repairSpots.Count];
        for(int i = 0; i < _repairSpots.Count; ++i)
        {
            repairStates[i] = _repairSpots[i].GetHasPlacedItem();
        }
        return repairStates;
    }

    public static void LoadRepairStates(bool[] currentRepairStates)
    {
        if (HasInstance)
        {
            Instance.LoadRepairStates_Instance(currentRepairStates);
        }
    }
    private void LoadRepairStates_Instance(bool[] currentRepairStates)
    {
        for (int i = 0; i < currentRepairStates.Length; ++i)
        {
            _repairSpots[i].SetHasPlacedItem(currentRepairStates[i]);
            if (currentRepairStates[i] == true)
            {
                ++_successfulRepairs;
            }
        }

        Debug.Log($"Loaded completed repair spots: {_successfulRepairs}");
    }

}
