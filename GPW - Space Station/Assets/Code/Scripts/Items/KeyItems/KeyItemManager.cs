using Environment.Buttons;
using Items.Collectables;
using Items.Flashlight;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Items.KeyItem
{
	public class KeyItemManager : MonoBehaviour
	{
		public static KeyItemManager Instance { get; private set; }

		[SerializeField] private Transform _keyItemSlot;
		[SerializeField] private PlayerTablet _playerTablet;
		[SerializeField] private PlayerInventory _playerInventory;
		[SerializeField] private FlashlightController _flashlightController;

		private GameObject _currentItem;
		private KeyItemData _currentKeyItem;

		private float _currentBattery;

		private bool _allowManualEquip = false;
		private bool _interactionTriggered = false;

		private UseKeyItem _activeRepairSpot;

		private void Awake()
		{
			if (Instance == null)
				Instance = this;
			else
				Destroy(gameObject);

			_playerTablet = FindAnyObjectByType<PlayerTablet>();

			_currentBattery = _flashlightController.GetCurrentBattery();
		}

		public PlayerTablet GetPlayerTablet() { return _playerTablet; }
		public void SetManualEquip(bool enable) => _allowManualEquip = enable;

		public void AllowKeyItemEquip() => _interactionTriggered = true;
		public void ResetKeyItemEquip() => _interactionTriggered = false;

		public bool CanEquipKeyItem() => _allowManualEquip || _interactionTriggered;

		public void EquipKeyItem(KeyItemData keyItemData)
		{
			if (!CanEquipKeyItem()) return;

			Debug.Log("Equipping Key Item: " + keyItemData.KeyItemPrefab.name);

			if (_currentKeyItem == keyItemData)
			{
				UnequipKeyItem();
				return;
			}

			if (_currentItem != null)
				Destroy(_currentItem);


			_currentItem = Instantiate(keyItemData.KeyItemPrefab, _keyItemSlot);
			_currentItem.transform.localPosition = Vector3.zero;
			_currentItem.transform.localRotation = Quaternion.identity;

			if (_currentItem.TryGetComponent(out CollectablePickup collectablePickup))
				Destroy(collectablePickup);

			if (_currentItem.TryGetComponent(out Collider itemCollider))
				itemCollider.enabled = false;

			_currentKeyItem = keyItemData;

			_playerTablet?.Unequip();
			//_playerInventory?.RemoveFlashlight();
		}

		public void UnequipKeyItem()
		{
			if (_currentItem != null)
			{
				Destroy(_currentItem);
				_currentItem = null;
				_currentKeyItem = null;

				//_playerInventory.AddFlashlight(_currentBattery);
			}
		}

		public GameObject GetHeldItem() => _currentItem;

		public void SetActiveRepairSpot(UseKeyItem repairSpot)
		{
			_activeRepairSpot = repairSpot;
		}

		public UseKeyItem GetActiveRepairSpot() { return _activeRepairSpot; }

		public void PlaceItemAtLocation(Transform location)
		{
			if (_currentItem != null)
			{
				_currentItem.transform.SetParent(null);
				_currentItem.transform.SetPositionAndRotation(location.position, location.rotation);
			}

			ResetKeyItemEquip();
		}
	}
}
