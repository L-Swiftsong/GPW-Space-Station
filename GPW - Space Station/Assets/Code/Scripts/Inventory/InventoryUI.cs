using Inventory.Data;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Inventory.UI
{
    public class InventoryUI : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private GameObject _inventoryUIContainer;
        [SerializeField] private List<Button> _inventoryButtons;

        [SerializeField] private PlayerInventory _playerInventory;
        private int _selectedIndex = -1;


        [Header("Inventory Settings")]
        [SerializeField] private bool _toggleInventory = false;
        private bool _isOpen = false;


        private void Awake()
        {
            // Setup the UI buttons.
            for (int i = 0; i < _inventoryButtons.Count; i++)
            {
                int slotIndex = i;
                _inventoryButtons[i].onClick.AddListener(() => EquipItem(slotIndex));
            }

            // Start with the UI hidden.
            _inventoryUIContainer.SetActive(false);
            _isOpen = false;
        }
        private void OnEnable()
        {
            PlayerInput.OnOpenInventoryPerformed += PlayerInput_OnOpenInventoryPerformed;
            PlayerInput.OnOpenInventoryStarted += PlayerInput_OnOpenInventoryStarted;
            PlayerInput.OnOpenInventoryCancelled += PlayerInput_OnOpenInventoryCancelled;

            _playerInventory.OnInventoryChanged += UpdateInventoryButtonText;
        }
        private void OnDisable()
        {
            PlayerInput.OnOpenInventoryPerformed -= PlayerInput_OnOpenInventoryPerformed;
            PlayerInput.OnOpenInventoryStarted -= PlayerInput_OnOpenInventoryStarted;
            PlayerInput.OnOpenInventoryCancelled -= PlayerInput_OnOpenInventoryCancelled;

            _playerInventory.OnInventoryChanged -= UpdateInventoryButtonText;
        }


        private void Update()
        {
            if (!_isOpen)
            {
                return;
            }

            if (PlayerInput.GamepadInventorySelect != Vector2.zero)
            {
                // Determine the selected index.
                float segmentSize = 360.0f / _inventoryButtons.Count;
                float angle = Vector2.SignedAngle(PlayerInput.GamepadInventorySelect, Vector2.up) + (segmentSize / 2.0f);
                if (angle < 0.0f)
                    angle += 360.0f;
                else if (angle > 360.0f)
                    angle -= 360.0f;

                _selectedIndex = Mathf.Max(Mathf.FloorToInt(angle / segmentSize), 0);


                // Highlight the selected button.
                _inventoryButtons[_selectedIndex].Select();
                
            }
            else
            {
                _selectedIndex = -1;
            }
        }


        private void PlayerInput_OnOpenInventoryPerformed()
        {
            if (!_toggleInventory)
            {
                return;
            }

            // Toggle the inventory open state.
            if (_isOpen)
            {
                Hide();
            }
            else
            {
                Show();
            }
        }
        private void PlayerInput_OnOpenInventoryStarted()
        {
            if (!_toggleInventory)
            {
                Show();
            }
        }
        private void PlayerInput_OnOpenInventoryCancelled()
        {
            if (!_toggleInventory)
            {
                Hide();
            }
        }


        private void EquipItem(int slotIndex)
        {
            // Notify the inventory to select the slot index.
            _playerInventory.EquipItem(slotIndex);

            // Hide the UI after selecting an item.
            HideFromSelect();
        }
        private void UpdateInventoryButtonText(InventoryItem[] inventoryItems)
        {
            // Update the inventory buttons.
            for (int i = 0; i < inventoryItems.Length; i++)
            {
                string itemText = inventoryItems[i] == null ? "-" : inventoryItems[i].GetItemName();
                _inventoryButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = itemText;
            }
        }


        private void Show()
        {
            _inventoryUIContainer.SetActive(true);
            _isOpen = true;

            Cursor.lockState = CursorLockMode.Confined;
        }
        private void Hide()
        {
            if (_selectedIndex != -1)
            {
                // We are wanting to select an item.
                _playerInventory.EquipItem(_selectedIndex);
            }

            _inventoryUIContainer.SetActive(false);
            _isOpen = false;

            Cursor.lockState = CursorLockMode.Locked;
        }
        private void HideFromSelect()
        {
            _inventoryUIContainer.SetActive(false);
            _isOpen = false;

            Cursor.lockState = CursorLockMode.Locked;
        }
    }
}
