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

public class UseKeyItem : MonoBehaviour, IInteractable
{
	public event System.Action OnSuccessfulInteraction;
	public event System.Action OnFailedInteraction;

	[SerializeField] private KeyItemData _requiredKeyItem;
	[SerializeField] private Transform _keyItemPlacement;

	[SerializeField] private PlayerTablet _playerTablet;
	[SerializeField] private GameObject _itemsTab;

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

				KeyItemEntryUI.Instance.RemoveItemFromUI(selectedKeyItem);
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
	}
}


