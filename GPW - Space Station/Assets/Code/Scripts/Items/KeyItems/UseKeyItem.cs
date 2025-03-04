using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Items.Collectables;
using Items.KeyItem;
using Items.Flashlight;
using TMPro.EditorUtilities;
using UI.ItemDisplay;

public class UseKeyItem : MonoBehaviour
{
	public static UseKeyItem Instance {  get; private set; }

	[SerializeField] private KeyItemData _requiredKeyItem;
	[SerializeField] private Transform _keyItemPlacement;
	[SerializeField] private EscapePodInteraction _escapePodInteraction;
	[SerializeField] private PlayerTablet _playerTablet;
	[SerializeField] private GameObject _itemsTab;

	private bool _hasInteracted = false;
	private bool _hasPlacedItem = false;

	private void Awake()
	{
		if (Instance == null)
			Instance = this;
		else
			Destroy(gameObject);
	}

	private void Start()
	{
		_escapePodInteraction.OnSuccessfulInteraction += HandlePlayerInteraction;

		_playerTablet = KeyItemManager.Instance.GetPlayerTablet();
	}
	private void OnDestroy()
	{
		_escapePodInteraction.OnSuccessfulInteraction -= HandlePlayerInteraction;
	}

	private void HandlePlayerInteraction()
	{
		if (_hasInteracted) return;
		_hasInteracted = true;

		if (_playerTablet != null)
		{
			_playerTablet?.Equip();
			_itemsTab.SetActive(true);
		}
	}


	public void TryUseKeyItem(KeyItemData selectedKeyItem)
	{
		if (!_hasInteracted || _hasPlacedItem) return;

		if (selectedKeyItem != _requiredKeyItem)
		{
			Debug.Log($"Selected key item doesn't match the required item. Required: {_requiredKeyItem.name}");
			_escapePodInteraction.FailInteraction();
			_escapePodInteraction.ResetInteraction();
			return;
		}

		if (!KeyItemManager.Instance.GetHeldItem()) return;
		
		KeyItemManager.Instance.EquipKeyItem(selectedKeyItem);
		KeyItemManager.Instance.PlaceItemAtLocation(_keyItemPlacement);
		KeyItemManager.Instance.UnequipKeyItem();
		KeyItemManager.Instance.ResetKeyItemEquip();

		_playerTablet?.Unequip();
		KeyItemEntryUI.Instance.RemoveItemFromUI(selectedKeyItem);

		_hasInteracted = false;
		_escapePodInteraction.ResetInteraction();
	}
}


