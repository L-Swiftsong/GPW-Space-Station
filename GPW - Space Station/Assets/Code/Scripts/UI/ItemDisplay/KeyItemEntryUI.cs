using UnityEngine;
using UnityEngine.UI;
using Items.Collectables;
using Items;
using Items.KeyItem;

namespace UI.ItemDisplay
{
    public class KeyItemEntryUI : ItemDisplaySegmentUI
    {
        public static KeyItemEntryUI Instance {  get; private set; }

        [SerializeField] private Image _keyItemImage;
        [SerializeField] private Button _UseButton;
        private KeyItemData _currentKeyItem;

		private void Awake()
		{
			if (Instance == null)
				Instance = this;
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
            KeyItemManager.Instance.EquipKeyItem(_currentKeyItem);
            UseKeyItem.Instance.TryUseKeyItem(_currentKeyItem);
        }

        public void RemoveItemFromUI(KeyItemData keyItemData)
        {
            if (_currentKeyItem == keyItemData)
            {
                _keyItemImage.sprite = null;
                _currentKeyItem = null;
                _UseButton.gameObject.SetActive(false);
            }
        }     
    }
}
