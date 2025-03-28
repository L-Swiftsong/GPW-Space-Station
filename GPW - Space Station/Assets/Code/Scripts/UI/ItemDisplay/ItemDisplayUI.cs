
using System.Collections.Generic;
using UnityEngine;
using Items.Collectables;

namespace UI.ItemDisplay
{
    public class ItemDisplayUI : MonoBehaviour
    {
        [Header("Collectable Type")]
        [SerializeField] private CollectableDataType _collectableType; // Do not change at runtime.


        [Header("UI Elements")]
        [SerializeField] private Transform _itemDisplayInstanceContainer;

        [Space(5)]
        [SerializeField] private GameObject _itemDisplaySegmentPrefab;
        private ItemDisplayGenerator _itemDisplayGenerator;


        private void Awake()
        {
            // Destroy accidentally remaining UI elements.
            foreach(Transform existingElement in _itemDisplayInstanceContainer)
            {
                Destroy(existingElement.gameObject);
            }

            _itemDisplayGenerator = new ItemDisplayGenerator();
        }
        private void OnEnable() => _itemDisplayGenerator.UpdateCollectedData(_collectableType, _itemDisplaySegmentPrefab, _itemDisplayInstanceContainer);



#if UNITY_EDITOR

        private void OnValidate()
        {
            if (_itemDisplaySegmentPrefab != null)
            {
                ItemDisplayGenerator.Validate(_collectableType, _itemDisplaySegmentPrefab);
            }
        }

#endif

        private class ItemDisplayGenerator
        {
            private List<ItemDisplaySegmentUI> _itemDisplaySegmentUIInstances;

            public ItemDisplayGenerator()
            {
                _itemDisplaySegmentUIInstances = new List<ItemDisplaySegmentUI>();
            }
            ~ItemDisplayGenerator()
            {
                foreach (ItemDisplaySegmentUI existingElement in _itemDisplaySegmentUIInstances)
                {
                    Destroy(existingElement.gameObject);
                }
            }
            


            public void UpdateCollectedData(CollectableDataType _collectableType, GameObject itemDisplaySegmentPrefab, Transform itemDisplayInstanceContainer)
            {
                switch(_collectableType)
                {
                    case CollectableDataType.Codex:
                        UpdateCollectedData<CodexData, CodexEntryUI>(itemDisplaySegmentPrefab, itemDisplayInstanceContainer);
                        break;
                    case CollectableDataType.KeyItem:
                        UpdateCollectedData<KeyItemData, KeyItemEntryUI>(itemDisplaySegmentPrefab, itemDisplayInstanceContainer);
                        break;

                    default:
                        throw new System.NotImplementedException();
                }
            }
            public void UpdateCollectedData<TData, TSegmentType>(GameObject itemDisplaySegmentPrefab, Transform itemDisplayInstanceContainer)
                where TData : CollectableData
                where TSegmentType : ItemDisplaySegmentUI
            {
                // Find current Codex Data.
                List<TData> collectedCodexData = CollectableManager.GetCollectablesOfType<TData>();

                int codexEntryDataCount = collectedCodexData.Count;
                Debug.Log($"Collected {typeof(TData)} Count: {codexEntryDataCount}");
                int codexEntryInstanceCount = _itemDisplaySegmentUIInstances.Count;

                // Disable unnecessary CodexEntryUI Instances.
                if (codexEntryInstanceCount > codexEntryDataCount)
                {
                    for (int i = codexEntryDataCount; i < codexEntryInstanceCount; ++i)
                    {
                        _itemDisplaySegmentUIInstances[i].gameObject.SetActive(false);
                    }
                }

                // Update our CodexEntryUI Instances.
                for (int i = 0; i < collectedCodexData.Count; ++i)
                {
                    if (i >= codexEntryInstanceCount)
                    {
                        // Create a new ItemDisplaySegmentUI Instance for this item entry.
                        GameObject codexEntryInstanceGO = Instantiate(itemDisplaySegmentPrefab, itemDisplayInstanceContainer);

                        _itemDisplaySegmentUIInstances.Add(codexEntryInstanceGO.GetComponent<TSegmentType>());
                        ++codexEntryInstanceCount;
                    }
                    // Ensure the UI segment is enabled.
                    _itemDisplaySegmentUIInstances[i].gameObject.SetActive(true);

                    _itemDisplaySegmentUIInstances[i].SetupCollectableEntry(collectedCodexData[i]);
                }
            }


            #if UNITY_EDITOR

            public static void Validate(CollectableDataType _collectableType, GameObject itemDisplaySegmentPrefab)
            {
                switch (_collectableType)
                {
                    case CollectableDataType.Codex:
                        if (itemDisplaySegmentPrefab.GetComponent<CodexEntryUI>() == null)
                        {
                            Debug.LogError("The passed prefab doesn't contain an instance of type CodexEntryUI.");
                        }
                        break;
                    case CollectableDataType.KeyItem:
                        if (itemDisplaySegmentPrefab.GetComponent<KeyItemEntryUI>() == null)
                        {
                            Debug.LogError("The passed prefab doesn't contain an instance of type KeyItemEntryUI.");
                        }
                        break;

                    default:
                        throw new System.NotImplementedException();
                }
            }

            #endif
        }
    }
}