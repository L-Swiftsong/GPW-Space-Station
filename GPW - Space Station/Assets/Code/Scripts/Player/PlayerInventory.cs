using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static Inventory.PlayerInventory;

namespace Inventory
{
    public class PlayerInventory : MonoBehaviour
    {
        private const int MAX_INVENTORY_SIZE = 8;



        [Header("References")]
        [SerializeField] private List<Button> _inventoryButtons;
        [SerializeField] private GameObject _inventoryMenu;
        
        
        [Space(5)]
        [SerializeField] private PlayerHealth _playerHealth;
        public PlayerHealth PlayerHealth => _playerHealth;

        



        [Header("Inventory")]
        [SerializeField] private Transform _inventoryItemContainer;
        [SerializeField] private bool _toggleInventory = false;
        public bool inventoryMenuOpen = false;


        private InventoryItem[] _inventoryItems = new InventoryItem[MAX_INVENTORY_SIZE];
        private int _equippedItemIndex = -1;
        private InventoryItem _currentItem => _equippedItemIndex == -1 ? null : _inventoryItems[_equippedItemIndex];


        [Header("Inventory Item Prefab References")]
        [SerializeField] private InventoryKeycard _inventoryKeycardPrefab;


        //enum of possible item types that can be held in inventory
        public enum ItemType
        {
            None,
            Flashlight,
            Keycard,
            HealthPack,
        }

        private void Awake()
        {
            // Start with all items unequipped.
            UnequipItems();

            // Setup the UI buttons.
            for(int i = 0; i < _inventoryButtons.Count; i++)
            {
                int slotIndex = i;
                _inventoryButtons[i].onClick.AddListener(() => EquipItem(slotIndex));
            }
        }


        #region Input Events

        private void OnEnable()
        {
            PlayerInput.OnUseItemStarted += PlayerInput_OnUseItemStarted;
            PlayerInput.OnUseItemCancelled += PlayerInput_OnUseItemCancelled;
            PlayerInput.OnAltUseItemStarted += PlayerInput_OnAltUseItemStarted;
            PlayerInput.OnAltUseItemCancelled += PlayerInput_OnAltUseItemCancelled;

            PlayerInput.OnOpenInventoryPerformed += PlayerInput_OnOpenInventoryPerformed;
            PlayerInput.OnOpenInventoryStarted += PlayerInput_OnOpenInventoryStarted;
            PlayerInput.OnOpenInventoryCancelled += PlayerInput_OnOpenInventoryCancelled;
        }
        private void OnDisable()
        {
            PlayerInput.OnUseItemStarted -= PlayerInput_OnUseItemStarted;
            PlayerInput.OnUseItemCancelled -= PlayerInput_OnUseItemCancelled;
            PlayerInput.OnAltUseItemStarted -= PlayerInput_OnAltUseItemStarted;
            PlayerInput.OnAltUseItemCancelled -= PlayerInput_OnAltUseItemCancelled;

            PlayerInput.OnOpenInventoryPerformed -= PlayerInput_OnOpenInventoryPerformed;
            PlayerInput.OnOpenInventoryStarted -= PlayerInput_OnOpenInventoryStarted;
            PlayerInput.OnOpenInventoryCancelled -= PlayerInput_OnOpenInventoryCancelled;
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


        private void PlayerInput_OnOpenInventoryPerformed()
        {
            if (!_toggleInventory)
            {
                return;
            }

            // Toggle the inventory open state.
            if (inventoryMenuOpen)
            {
                CloseInventory();
            }
            else
            {
                OpenInventory();
            }
        }
        private void PlayerInput_OnOpenInventoryStarted()
        {
            if (!_toggleInventory)
            {
                OpenInventory();
            }
        }
        private void PlayerInput_OnOpenInventoryCancelled()
        {
            if (!_toggleInventory)
            {
                CloseInventory();
            }
        }

        #endregion


        void Update()
        {
            // Update the UI.
            UpdateInventoryButtonText(); 
        }


        #region UI

        private void OpenInventory()
        {
            _inventoryMenu.SetActive(true);
            inventoryMenuOpen = true; //Bool used in player controller script to prevent moving camera when menu is open

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        private void CloseInventory()
        {
            _inventoryMenu.SetActive(false);
            inventoryMenuOpen = false;

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        // Updates the button text based on item type.
        private void UpdateInventoryButtonText()
        {
            for (int i = 0; i < _inventoryButtons.Count; i++)
            {
                string itemText = _inventoryItems[i] == null ? "-" : _inventoryItems[i].GetItemName();
                _inventoryButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = itemText;
            }
        }

        #endregion


        #region Item Adding

        // Finds an empty slot in inventory and occupies it with the given item.
        public InventoryItem AddItem(InventoryItem inventoryItem) => AddInstantiatedItem(Instantiate<InventoryItem>(inventoryItem, _inventoryItemContainer));
        public InventoryItem AddInstantiatedItem(InventoryItem inventoryItem)
        {
            for (int i = 0; i < _inventoryItems.Length; i++)
            {
                if (_inventoryItems[i] == null)
                {
                    _inventoryItems[i] = inventoryItem;

                    inventoryItem.transform.SetParent(_inventoryItemContainer, worldPositionStays: false);
                    _inventoryItems[i].Initialise(this);
                    _inventoryItems[i].Unequip();

                    return _inventoryItems[i];
                }
            }

            return null;
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
                //Update currently equipped item
                _equippedItemIndex = slotIndex;

                UnequipItems();

                if (_inventoryItems[slotIndex] != null)
                {
                    _inventoryItems[slotIndex].Equip();
                }
            }

            // Close the inventory after equipping an item.
            CloseInventory();
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


        

        public void AddKeycard(int keycardID)
        {
            // Instantiate the keycard and set it up with it's ID.
            InventoryKeycard inventoryKeycardScript = Instantiate<InventoryKeycard>(_inventoryKeycardPrefab, _inventoryItemContainer);
            inventoryKeycardScript.SetKeycardID(keycardID);

            // Add the instantiated keycard to the inventory.
            AddInstantiatedItem(inventoryKeycardScript);

            // Debug.
            Debug.Log($"Picked up keycard: {keycardID}");
        }

        public bool HasKeycardEquipped(int keycardID)
        {
            if (_currentItem != null && _currentItem is InventoryKeycard)
            {
                return (_inventoryItems[_equippedItemIndex] as InventoryKeycard).GetKeycardID() == keycardID;
            }

            return false;
        }


        public InventoryItem[] GetAllItems() => _inventoryItems;
        public void SetInventoryItems(InventoryItem[] newItems)
        {
            _inventoryItems = new InventoryItem[MAX_INVENTORY_SIZE];
            for (int i = 0; i < MAX_INVENTORY_SIZE; i++)
            {
                if (newItems.Length < i)
                {
                    // We've added all the items that were in the save data.
                    return;
                }

                _inventoryItems[i] = newItems[i];
            }
        }
        public InventoryItem GetEquippedItem() => _currentItem;
    }
}