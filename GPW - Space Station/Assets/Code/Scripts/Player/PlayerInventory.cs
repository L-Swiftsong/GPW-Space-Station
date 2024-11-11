using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerInventory : MonoBehaviour
{
    public List<ItemType> inventorySlots = new List<ItemType>(new ItemType[8]);

    public List<int> keyCards = new List<int>();

    public List<Button> inventoryButtons;

    public GameObject inventoryMenu;
    public GameObject flashLight;
    public GameObject blueKeyCard;
    public GameObject greenKeyCard;
    public GameObject redKeyCard;
    public GameObject blueKeyCard2;

    private PlayerHealth playerHealth;

    private ItemType currentItem = ItemType.None;

    private int equippedSlotIndex = -1;

    public bool inventoryMenuOpen = false;
    [SerializeField] private bool _toggleInventory = false;

    public bool hasFlashLight = false;
    public bool flashLightPickedUp = false;

    //enum of possible item types that can be held in inventory
    public enum ItemType
    {
        None,
        Flashlight,
        BlueKeyCard,
        GreenKeyCard,
        RedKeyCard,
        BlueKeyCard2,
        HealthPack
    }

    private void Start()
    {
        // References.
        playerHealth = FindObjectOfType<PlayerHealth>();

        // Start with all items unequipped.
        UnequipItems();

        // Setup the UI buttons.
        for(int i = 0; i < inventoryButtons.Count; i++)
        {
            int slotIndex = i;
            inventoryButtons[i].onClick.AddListener(() => EquipItem(slotIndex));
        }
    }


    #region Input Events

    private void OnEnable()
    {
        PlayerInput.OnOpenInventoryPerformed += PlayerInput_OnOpenInventoryPerformed;
        PlayerInput.OnOpenInventoryStarted += PlayerInput_OnOpenInventoryStarted;
        PlayerInput.OnOpenInventoryCancelled += PlayerInput_OnOpenInventoryCancelled;
    }
    private void OnDisable()
    {
        PlayerInput.OnOpenInventoryPerformed -= PlayerInput_OnOpenInventoryPerformed;
        PlayerInput.OnOpenInventoryStarted -= PlayerInput_OnOpenInventoryStarted;
        PlayerInput.OnOpenInventoryCancelled -= PlayerInput_OnOpenInventoryCancelled;
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
        //If condition to check when the player picks up flashlight
        if (hasFlashLight && !flashLightPickedUp)
        {
            flashLight.SetActive(false); //Set flashlight to unequipped by default
            PickupFlashlight(); //Add flashlight item type to inventory list
            flashLightPickedUp = true; //bool to control when flashlight can be added to inventory again
        }

        UpdateInventoryButtonText(); //Updates UI
    }

    private void OpenInventory()
    {
        inventoryMenu.SetActive(true);
        inventoryMenuOpen = true; //Bool used in player controller script to prevent moving camera when menu is open

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    private void CloseInventory()
    {
        inventoryMenu.SetActive(false);
        inventoryMenuOpen = false;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }


    //Functions called to add items to inventory list
    public void PickupFlashlight() => AddItem(ItemType.Flashlight);
    public void PickupHealthPack() => AddItem(ItemType.HealthPack);
    public void PickupBlueKeyCard() => AddItem(ItemType.BlueKeyCard);
    public void PickupGreenKeyCard() => AddItem(ItemType.GreenKeyCard);
    public void PickupRedKeyCard() => AddItem(ItemType.RedKeyCard);
    public void PickupBlueKeyCard2() => AddItem(ItemType.BlueKeyCard2);


    //Finds an empty slot in inventory and occupies it with given item type
    public void AddItem(ItemType itemType)
    {
        for (int i = 0; i < inventorySlots.Count; i++)
        {
            if (inventorySlots[i] == ItemType.None)
            {
                inventorySlots[i] = itemType;
                break;
            }
        }
    }

    public void EquipItem(int slotIndex)
    {
        if (slotIndex >= 0 && slotIndex < inventorySlots.Count && !playerHealth.isHealing) //Finds a valid slot index corresponding to item slot pressed, cant change current item if player is healing
        {
            //Update currently equipped item
            currentItem = inventorySlots[slotIndex];
            equippedSlotIndex = slotIndex;

            if (currentItem == ItemType.HealthPack) //Checks if curent item is healthpack
            {
                //UnEquips all items
                UnequipItems();

                //Equip heal
                playerHealth.EquipHeal();
            }
            else if (currentItem == ItemType.Flashlight) //Checks if current item is flashlight
            {
                UnequipItems();

                //Equip flashlight
                flashLight.SetActive(true);
            }
            else if (currentItem == ItemType.BlueKeyCard) //Checks if current item is blue keycard
            {
                UnequipItems();

                //Equip blue keycard
                blueKeyCard.SetActive(true);
            }
            else if (currentItem == ItemType.GreenKeyCard) //Checks if current item is green keycard
            {
                UnequipItems();

                //Equip green keycard
                greenKeyCard.SetActive(true);
            }
            else if (currentItem == ItemType.RedKeyCard) //Checks if current item is red keycard
            {
                UnequipItems();

                //Equip red keycard
                redKeyCard.SetActive(true);
            }
            else if (currentItem == ItemType.BlueKeyCard2) //Checks if current item is second blue keycard
            {
                UnequipItems();

                //Equip second blue keycard
                blueKeyCard2.SetActive(true);
            }
            else if (currentItem == ItemType.None) //Checks if current item is null
            {
                UnequipItems();
            }
        }

        // Close the inventory after equipping an item.
        CloseInventory();
    }

    public void UnequipItems()
    {
        //Unequip flashlight
        if (hasFlashLight)
        {
            flashLight.SetActive(false);
        }

        //Unequip heal
        playerHealth.healthPack.SetActive(false);

        //Unequip keycards
        blueKeyCard.SetActive(false);
        greenKeyCard.SetActive(false);
        redKeyCard.SetActive(false);
        blueKeyCard2.SetActive(false);
    }

    //Removes health pack from inventory when used
    public void RemoveHealthPack()
    {
        //Removes health pack from equipped slot index
        if (equippedSlotIndex >= 0 && equippedSlotIndex < inventorySlots.Count)
        {
            if (inventorySlots[equippedSlotIndex] == ItemType.HealthPack)
            {
                inventorySlots[equippedSlotIndex] = ItemType.None;
                UpdateInventoryButtonText();
            }
        }
    }

    //Removes flashlight from inventory when put in recharge station
    public void RemoveFlashLight()
    {
        //Finds slot index containing flashlight and returns it to null
        for (int i = 0; i < inventorySlots.Count; i++)
        {
            if (inventorySlots[i] == ItemType.Flashlight)
            {
                inventorySlots[i] = ItemType.None;
                UpdateInventoryButtonText();
                break;
            }
        }
    }


    //Updates Button text based on item type
    private void UpdateInventoryButtonText()
    {
        for (int i = 0; i < inventorySlots.Count; i++)
        {
            string itemText = inventorySlots[i] == ItemType.None ? "-" : inventorySlots[i].ToString();
            inventoryButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = itemText;
        }
    }

    public void AddKeyCard(int keyCardId)
    {
        if (!keyCards.Contains(keyCardId)) 
        {
            keyCards.Add(keyCardId);
            Debug.Log($"Picked up keycard: {keyCardId}");
        }
    }

    public bool HasKeyCard(int keyCardId)
    {
        return keyCards.Contains(keyCardId); 
    }
}
