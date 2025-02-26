using Environment.Buttons;
using Items.Collectables;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Items.KeyItem
{
	public class KeyItemManager : MonoBehaviour
	{
		public static KeyItemManager Instance;

		[SerializeField] private Transform keyItemSlot; // Assign this to your empty GameObject in the player's hand
		[SerializeField] private GameObject[] keyItemPrefabs;

		private Dictionary<string, GameObject> keyItemDictionary = new Dictionary<string, GameObject>();
		private GameObject currentEquippedItem;

		private void Awake()
		{
			if (Instance == null)
			{
				Instance = this;
			}
			else
			{
				Destroy(gameObject);
				return;
			}

			// Populate the dictionary for quick lookup
			foreach (var item in keyItemPrefabs)
			{
				keyItemDictionary[item.name] = item;
			}
		}

		public void EquipKeyItem(string itemName)
		{
			if (!keyItemDictionary.ContainsKey(itemName))
			{
				Debug.LogWarning($"Key item '{itemName}' not found in dictionary!");
				return;
			}

			// Remove the currently equipped item (if any)
			foreach (Transform child in keyItemSlot)
			{
				Destroy(child.gameObject);
			}

			// Instantiate and attach the new item to keyItemSlot
			currentEquippedItem = Instantiate(keyItemDictionary[itemName], keyItemSlot.position, keyItemSlot.rotation);
			currentEquippedItem.transform.SetParent(keyItemSlot);
			currentEquippedItem.SetActive(true);
		}
	}
}
