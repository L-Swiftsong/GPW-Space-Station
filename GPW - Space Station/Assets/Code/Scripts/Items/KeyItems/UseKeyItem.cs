using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Items.Collectables;
using Items.KeyItem;
using Items.Flashlight;
using TMPro.EditorUtilities;
using UI.ItemDisplay;
using Interaction;
using Items;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class UseKeyItem : MonoBehaviour, IInteractable
{
	public event System.Action OnSuccessfulInteraction;
	public event System.Action OnFailedInteraction;

	[Header("Key Item")]
	[SerializeField] private KeyItemData _requiredKeyItem;
	[SerializeField] private Transform _keyItemPlacement;

	[Header("Player Tablet")]
	[SerializeField] private PlayerTablet _playerTablet;
	[SerializeField] private GameObject _itemsTab;

	[Header("UI Feedback")]
	[SerializeField] private Text _feedbackText;
	[SerializeField] private string _incorrectItemMessage = "Incorrect item!";

	private bool _isInteracted;

	public bool IsKeyItemCorrect(KeyItemData selectedKeyItem)
	{
		return _requiredKeyItem == selectedKeyItem;
	}

	private void Awake()
	{
		if (_playerTablet == null)
		{
			_playerTablet = FindObjectOfType<PlayerTablet>();
		}
	}

	public void TryUseKeyItem(KeyItemData selectedKeyItem)
	{
		if (_isInteracted)
		{
			if (IsKeyItemCorrect(selectedKeyItem))
			{
				KeyItemManager.Instance.PlaceItemAtLocation(_keyItemPlacement);
				OnSuccessfulInteraction?.Invoke();

				KeyItemEntryUI[] allUIEntries = FindObjectsOfType<KeyItemEntryUI>();

				foreach (var uiEntry in allUIEntries)
				{
					if (uiEntry._currentKeyItem == selectedKeyItem)
					{
						uiEntry.RemoveItemFromUI(selectedKeyItem);
						break; 
					}
				}

				_playerTablet.Unequip();
			}
			else
			{
				OnFailedInteraction?.Invoke();
			}
		}
	}

	public void Interact(PlayerInteraction interaction)
	{
		if (!_isInteracted)
		{
			_isInteracted = true;

			_playerTablet.Equip();
			//_itemsTab.SetActive(true);

			var repairSpotManager = FindObjectOfType<RepairSpotManager>();

			if (repairSpotManager != null)
			{
				repairSpotManager.InteractWithRepairSpot(this);
			}

			OnSuccessfulInteraction.Invoke();
		}
	}

	public void FailInteraction()
	{
		OnFailedInteraction?.Invoke();
		ShowErrorMessage(_incorrectItemMessage);
	}

	private void ShowErrorMessage(string message)
	{
		if (_feedbackText != null)
		{
			_feedbackText.text = message;
			_feedbackText.gameObject.SetActive(true);
			StartCoroutine(HideFeedbackAfterDelay(2f));
		}
	}

	private IEnumerator HideFeedbackAfterDelay(float delay)
	{
		yield return new WaitForSeconds(delay);
		_feedbackText.gameObject.SetActive(false);
	}

}


