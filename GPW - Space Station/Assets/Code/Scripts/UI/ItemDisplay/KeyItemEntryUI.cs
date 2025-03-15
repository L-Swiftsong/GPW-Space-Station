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
        private static List<KeyItemEntryUI> _allKeyItemEntryUIs = new List<KeyItemEntryUI>();

        [SerializeField] private Image _keyItemImage;
        [SerializeField] private Button _UseButton;

        public KeyItemData _currentKeyItem;
        private RepairSpotManager _repairSpotManager;

        public bool _isUsed = false;

		private void Awake()
		{
            if (!_allKeyItemEntryUIs.Contains(this))
                _allKeyItemEntryUIs.Add(this);

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
        }

        private void OnUseButtonClicked()
        {
			//KeyItemManager.Instance.EquipKeyItem(_currentKeyItem);

            if (_isUsed) return;

            if (_repairSpotManager != null)
            {
                _repairSpotManager.TryUseKeyItem(_currentKeyItem);
            }
        }

        public void RemoveItemFromUI(KeyItemData keyItemData)
        {
            if (_currentKeyItem == keyItemData)
            {
                _isUsed = true;
                _UseButton.interactable = false;
                _keyItemImage.color = new Color(1, 1, 1, 0.5f);
                _currentKeyItem = null;
                
                UnregisterKeyItemEntry(this);
            }
		}     

		public void UnregisterKeyItemEntry(KeyItemEntryUI entryUI)
		{
			if (_allKeyItemEntryUIs.Contains(entryUI))
			{
				_allKeyItemEntryUIs.Remove(entryUI);
			}
		}

		private void OnDestroy()
		{
			UnregisterKeyItemEntry(this);
		}
	}
}
