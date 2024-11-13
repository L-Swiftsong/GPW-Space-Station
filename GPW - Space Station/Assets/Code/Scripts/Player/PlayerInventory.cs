using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Inventory.Data;
using Inventory.Items;

namespace Inventory
{
    public class PlayerInventory : MonoBehaviour
    {
        private const int MAX_INVENTORY_SIZE = 8;


        [Header("References")]
        [SerializeField] private PlayerHealth _playerHealth;
        public PlayerHealth PlayerHealth => _playerHealth;

        
        [Header("Inventory")]
        [SerializeField] private Transform _inventoryItemContainer;
        [SerializeField] private bool _toggleInventory = false;


        private InventoryItem[] _inventoryItems = new InventoryItem[MAX_INVENTORY_SIZE];
        private int _equippedItemIndex = -1;
        private InventoryItem _currentItem => _equippedItemIndex == -1 ? null : _inventoryItems[_equippedItemIndex];


        [Header("Inventory Item Prefab References")]
        [SerializeField] private InventoryKeycard _inventoryKeycardPrefab;

        public event System.Action<InventoryItem[]> OnInventoryChanged;


        private void Awake()
        {
            // Start with all items unequipped.
            UnequipItems();
        }

        private void Start() => OnInventoryChanged?.Invoke(_inventoryItems);



        #region Input Events

        private void OnEnable()
        {
            PlayerInput.OnUseItemStarted += PlayerInput_OnUseItemStarted;
            PlayerInput.OnUseItemCancelled += PlayerInput_OnUseItemCancelled;
            PlayerInput.OnAltUseItemStarted += PlayerInput_OnAltUseItemStarted;
            PlayerInput.OnAltUseItemCancelled += PlayerInput_OnAltUseItemCancelled;
        }
        private void OnDisable()
        {
            PlayerInput.OnUseItemStarted -= PlayerInput_OnUseItemStarted;
            PlayerInput.OnUseItemCancelled -= PlayerInput_OnUseItemCancelled;
            PlayerInput.OnAltUseItemStarted -= PlayerInput_OnAltUseItemStarted;
            PlayerInput.OnAltUseItemCancelled -= PlayerInput_OnAltUseItemCancelled;
        }


        private void PlayerInput_OnUseItemStarted()
        {
            if (_currentItem != null)
            {
                _currentItem.StartUse();
            }
        }
        private void PlayerInput_OnUseItemCancelled()
        {
            if (_currentItem != null)
            {
                _currentItem.StopUse();
            }
        }
        private void PlayerInput_OnAltUseItemStarted()
        {
            if (_currentItem != null)
            {
                _currentItem.StartAltUse();
            }
        }
        private void PlayerInput_OnAltUseItemCancelled()
        {
            if (_currentItem != null)
            {
                _currentItem.StopAltUse();
            }
        }

        #endregion


        #region Item Adding

        // Finds an empty slot in inventory and occupies it with the given item.
        public bool AddItem(InventoryItemDataSO itemData, float[] itemValues = null)
        {
            if (!TryGetFirstFreeSlotIndex(out int firstFreeIndex))
            {
                // We failed to add the item (No free slots).
                return false;
            }

            // We have found a free slot.
            // Instantiate the Inventory Item.
            InventoryItem inventoryItem = Instantiate<InventoryItem>(itemData.ItemPrefab, _inventoryItemContainer);

            // Setup the inventory item.
            inventoryItem.Initialise(this, itemData, itemValues);
            inventoryItem.Unequip();

            // Add the item to the inventory.
            _inventoryItems[firstFreeIndex] = inventoryItem;

            OnInventoryChanged?.Invoke(_inventoryItems);
            return true;
        }
        public bool AddInstantiatedItem(InventoryItem inventoryItem, float[] itemValues = null)
        {
            if (!TryGetFirstFreeSlotIndex(out int firstFreeIndex))
            {
                // We failed to add the item (No free slots).
                return false;
            }

            // We have found a free slot.
            // Re-parent the inventory item.
            inventoryItem.transform.SetParent(_inventoryItemContainer, worldPositionStays: false);

            // Setup the inventory item.
            inventoryItem.Initialise(this, inventoryItem.GetItemData(), itemValues);
            inventoryItem.Unequip();

            // Add the inventory item to the inventory.
            _inventoryItems[firstFreeIndex] = inventoryItem;

            OnInventoryChanged?.Invoke(_inventoryItems);
            return true;
        }
        public bool AddItemToIndex(int index, InventoryItemDataSO itemData, float[] itemValues = null)
        {
            // Instantiate the Inventory Item.
            InventoryItem inventoryItem = Instantiate<InventoryItem>(itemData.ItemPrefab, _inventoryItemContainer);

            // Setup the inventory item.
            inventoryItem.Initialise(this, itemData, itemValues);
            inventoryItem.Unequip();

            // Add the item to the inventory.
            _inventoryItems[index] = inventoryItem;

            OnInventoryChanged?.Invoke(_inventoryItems);
            return true;
        }

        private bool TryGetFirstFreeSlotIndex(out int firstFreeIndex)
        {
            for (int i = 0; i < _inventoryItems.Length; i++)
            {
                if (_inventoryItems[i] == null)
                {
                    // This slot is empty.
                    firstFreeIndex = i;
                    return true;
                }
            }

            // There were no empty slots.
            firstFreeIndex = -1;
            return false;
        }

        #endregion

        #region Item Removal

        public void RemoveInventoryItem(InventoryItem inventoryItem)
        {
            for (int i = 0; i < _inventoryItems.Length; i++)
            {
                if (_inventoryItems[i] == inventoryItem)
                {
                    _inventoryItems[i] = null;
                }
            }
        }
        public InventoryItem RemoveInventoryItem(int itemIndex)
        {
            InventoryItem inventoryItem = _inventoryItems[itemIndex];
            _inventoryItems[itemIndex] = null;
            return inventoryItem;
        }
        public bool TryRemoveInventoryItemByType<T>(out T itemInstance) where T : InventoryItem
        {
            for(int i = 0; i < _inventoryItems.Length; i++)
            {
                if (_inventoryItems[i].GetType() == typeof(T))
                {
                    // This inventory item is of the type we are wanting to remove.
                    itemInstance = _inventoryItems[i] as T;
                    _inventoryItems[i] = null;
                    return true;
                }
            }

            // We failed to find an instance of the item.
            itemInstance = null;
            return false;
        }

        #endregion


        #region Item Equipping/Unequipping

        public void EquipItem(int slotIndex)
        {
            // Finds a valid slot index corresponding to item slot pressed.
            if (slotIndex >= 0 && slotIndex < _inventoryItems.Length)
            {
                // Update currently equipped item.
                _equippedItemIndex = slotIndex;

                UnequipItems();

                if (_inventoryItems[slotIndex] != null)
                {
                    _inventoryItems[slotIndex].Equip();
                }
            }
        }

        public void UnequipItems()
        {
            for (int i = 0; i < _inventoryItems.Length; i++)
            {
                if (_inventoryItems[i] == null)
                {
                    continue;
                }

                _inventoryItems[i].Unequip();
            }
        }

        #endregion

        public bool HasKeycardEquipped(int keycardID)
        {
            if (_currentItem != null && _currentItem is InventoryKeycard)
            {
                return (_inventoryItems[_equippedItemIndex] as InventoryKeycard).GetKeycardID() == keycardID;
            }

            return false;
        }


        public InventoryItem[] GetAllItems() => _inventoryItems;
        public void SetInventoryItems(ItemSaveData[] itemSaveData, int equippedItemIndex)
        {
            if (itemSaveData.Length > MAX_INVENTORY_SIZE)
            {
                Debug.LogError("Error: We are trying to add too many items to the inventory when loading from a save");
                throw new System.ArgumentOutOfRangeException();
            }

            // Populate the inventory from our save data.
            _inventoryItems = new InventoryItem[MAX_INVENTORY_SIZE];
            for (int i = 0; i < itemSaveData.Length; i++)
            {
                if (itemSaveData[i].ItemData == null)
                {
                    // No item in this slot.
                    continue;
                }

                AddItemToIndex(i, itemSaveData[i].ItemData, itemSaveData[i].ItemValues);
            }

            // Equip our desired item.
            EquipItem(equippedItemIndex);
        }
        public InventoryItem GetEquippedItem() => _currentItem;
        public int GetEquippedItemIndex() => _equippedItemIndex;
    }
}