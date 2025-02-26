using UnityEngine;
using UnityEngine.UI;
using Items.Collectables;
using Items;
using Items.KeyItem;
using UnityEngine.ProBuilder.MeshOperations;

namespace UI.ItemDisplay
{
    public class KeyItemEntryUI : ItemDisplaySegmentUI
    {
        [SerializeField] private Image _keyItemImage;
        [SerializeField] private Button _equipButton;

        private string itemName;

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
            itemName = keyItemData.name;
            _keyItemImage.sprite = keyItemData.KeyItemSprite;

            _equipButton.onClick.RemoveAllListeners();

            _equipButton.onClick.AddListener(OnEquipButtonClicked); 

        }

        public void OnEquipButtonClicked()
        {
            if (KeyItemManager.Instance != null)
            {
                KeyItemManager.Instance.EquipKeyItem(itemName);
            }
        }
    }
}
