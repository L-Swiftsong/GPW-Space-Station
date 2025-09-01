using UnityEngine;
using UnityEngine.UI;
using Items.Collectables;
using System.Collections.Generic;
using System.Linq;

namespace UI.ItemDisplay
{
    public class KeyItemEntryUI : ItemDisplaySegmentUI
    {
        private static List<KeyItemEntryUI> _allKeyItemEntryUIs = new List<KeyItemEntryUI>();
        public static Dictionary<KeyItemData, bool> s_KeyItemDataUsedState = new Dictionary<KeyItemData, bool>(); // Find a new place for this that isn't in UI?


        public static void ResetUsedKeyItems()
        {
            if (s_KeyItemDataUsedState != null)
            {
                KeyItemData[] keyItemDatas = s_KeyItemDataUsedState.Keys.ToArray();
                for (int i = 0; i < keyItemDatas.Length; ++i)
                {
                    s_KeyItemDataUsedState[keyItemDatas[i]] = false;
                }
            }
        }
        public static void SetRequiredKeyItem(KeyItemData requiredKeyItemData)
        {
            s_requiredKeyItemData = requiredKeyItemData;
            OnRequiredKeyItemChanged?.Invoke();
        }
        private static KeyItemData s_requiredKeyItemData;
        private static event System.Action OnRequiredKeyItemChanged;


        [SerializeField] private Image _keyItemImage;
        [SerializeField] private Button _UseButton;

        public KeyItemData _currentKeyItem;
        private RepairSpotManager _repairSpotManager;


        [Header("Selection Colours")]
        [SerializeField] private Color _isRequiredPressedColour = new Color(0.21f, 1.0f, 0.21f);
        [SerializeField] private Color _isRequiredHighlightedColour = new Color(0.5f, 1.0f, 0.5f);

        [Space(5)]
        [SerializeField] private Color _isNotRequiredPressedColour = new Color(1.0f, 0.21f, 0.21f);
        [SerializeField] private Color _isNotRequiredHighlightedColour = new Color(1.0f, 0.5f, 0.5f);


		private void Awake()
		{
            if (!_allKeyItemEntryUIs.Contains(this))
                _allKeyItemEntryUIs.Add(this);

            _repairSpotManager = FindObjectOfType<RepairSpotManager>();
            OnRequiredKeyItemChanged += CheckIsRequired;
        }
        private void OnEnable()
        {
            CheckIsUsed();
            CheckIsRequired();
        }
        private void OnDestroy()
        {
            OnRequiredKeyItemChanged -= CheckIsRequired;
            UnregisterKeyItemEntry(this);
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

            CheckIsUsed();
            CheckIsRequired();
        }

        private void OnUseButtonClicked()
        {
            if (_repairSpotManager != null)
            {
                _repairSpotManager.TryUseKeyItem(_currentKeyItem);
            }
        }
        private void CheckIsUsed()
        {
            if (_currentKeyItem != null && s_KeyItemDataUsedState.TryGetValue(_currentKeyItem, out bool hasBeenUsed) && hasBeenUsed)
            {
                _UseButton.interactable = false;
                _keyItemImage.color = new Color(1, 1, 1, 0.5f);
                _currentKeyItem = null;
            }
        }
        private void CheckIsRequired()
        {
            // Copy our button's colours to a new struct as we cannot edit the individual values otherwise.
            ColorBlock newColourBlock = _UseButton.colors;

            if (_currentKeyItem != null && _currentKeyItem == s_requiredKeyItemData)
            {
                // This slot's Key Item is the currently required one.
                newColourBlock.pressedColor = _isRequiredPressedColour;
                newColourBlock.highlightedColor = _isRequiredHighlightedColour;
                newColourBlock.selectedColor = _isRequiredHighlightedColour;
            }
            else
            {
                // This slot's Key Item is NOT the currently required one.
                newColourBlock.pressedColor = _isNotRequiredPressedColour;
                newColourBlock.highlightedColor = _isNotRequiredHighlightedColour;
                newColourBlock.selectedColor = _isNotRequiredHighlightedColour;
            }

            // Set our button's colours.
            _UseButton.colors = newColourBlock;
        }

		public void UnregisterKeyItemEntry(KeyItemEntryUI entryUI)
		{
			if (_allKeyItemEntryUIs.Contains(entryUI))
			{
				_allKeyItemEntryUIs.Remove(entryUI);
			}
		}
	}
}
