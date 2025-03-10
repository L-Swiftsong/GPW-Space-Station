using UnityEngine;
using UnityEngine.UI;
using Items.Collectables;
using Items;
using Items.KeyItem;
using System.Collections.Generic;
using static UnityEngine.EventSystems.EventTrigger;

namespace UI.ItemDisplay
{
    public class KeyItemEntryUI : ItemDisplaySegmentUI
    {
        public static KeyItemEntryUI Instance {  get; private set; }

        private List<KeyItemEntryUI> _keyItemEntryUIs = new List<KeyItemEntryUI>();

        [SerializeField] private Image _keyItemImage;
        [SerializeField] private Button _UseButton;
        private KeyItemData _currentKeyItem;
        private RepairSpotManager _repairSpotManager;

		private void Awake()
		{
			if (Instance == null)
				Instance = this;

            _repairSpotManager = FindObjectOfType<RepairSpotManager>();
		}

		public override void SetupCollectableEntry(CollectableData collectableData)
        {
            try
            {
                base.SetupCollectableEntry(collectableData);
                SetupKeyItemEntry(collectableData as KeyItemData);
            }
            catch (System.InvalidCastException e)
            {
                Debug.LogError("You are trying to supply a CodexEntryUI element with a CollectableData that doesn't inherit from CodexData.\n" +
                    $"Passed Item: {collectableData.name}\n" +
                    e.Message);
            }
        }
        public void SetupKeyItemEntry(KeyItemData keyItemData)
        {
            if (keyItemData == null) return;

            _keyItemImage.sprite = keyItemData.KeyItemSprite;
            _currentKeyItem = keyItemData;

            _UseButton.onClick.RemoveAllListeners();
            _UseButton.onClick.AddListener(() => OnUseButtonClicked());

            RegisterKeyItemEntry(this);
        }

        private void OnUseButtonClicked()
        {
			//KeyItemManager.Instance.EquipKeyItem(_currentKeyItem);

            if (_repairSpotManager != null)
            {
                _repairSpotManager.TryUseKeyItem(_currentKeyItem);
            }
        }

        public void RemoveItemFromUI(KeyItemData keyItemData)
        {
            if (_currentKeyItem == keyItemData)
            {
                _keyItemImage.sprite = null;
                _currentKeyItem = null;
                _UseButton.gameObject.SetActive(false);

                UnregisterKeyItemEntry(this);

                Destroy(gameObject);
            }
			else
			{
				Debug.LogWarning("Item not found in UI for removal: " + keyItemData.name);
			}
		}     

        public void RegisterKeyItemEntry(KeyItemEntryUI keyItemEntryUI)
        {
			if (!_keyItemEntryUIs.Contains(keyItemEntryUI))
			{
				_keyItemEntryUIs.Add(keyItemEntryUI);
			}
		}

		public void UnregisterKeyItemEntry(KeyItemEntryUI entryUI)
		{
			if (_keyItemEntryUIs.Contains(entryUI))
			{
				_keyItemEntryUIs.Remove(entryUI);
			}
		}
	}
}
