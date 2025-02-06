using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Items.Collectables;

namespace UI.Journal
{
    public class JournalUI : MonoBehaviour
    {
        [Header("Codexes")]
        [SerializeField] private Transform _codexEntryInstanceContainer;

        [Space(5)]
        [SerializeField] private GameObject _codexEntryPrefab;
        private List<CodexEntryUI> _codexEntryUIInstanceList;


        [Header("Debug")]
        [SerializeField] private List<CodexData> _collectedCodexesList;
        [SerializeField] private List<CollectableData> _assortedDataList;


        private void Awake()
        {
            foreach(Transform existingElement in _codexEntryInstanceContainer)
            {
                Destroy(existingElement.gameObject);
            }

            _codexEntryUIInstanceList = new List<CodexEntryUI>();
        }


        [ContextMenu(itemName: "Reset Test")]
        private void ResetForTest()
        {
            // Delete existing children.
            foreach (Transform existingElement in _codexEntryInstanceContainer)
            {
                Destroy(existingElement.gameObject);
            }

            // Reset CodexEntryUI List.
            _codexEntryUIInstanceList = new List<CodexEntryUI>();
        }

        [ContextMenu(itemName: "Perform Test")]
        public void UpdateCollectedData()
        {
            // Find current collected data.
            

            // Update our CodexEntryUI Instances.
            int codexEntryInstanceCount = _codexEntryUIInstanceList.Count;
            for (int i = 0; i < _collectedCodexesList.Count; ++i)
            {
                if (i >= codexEntryInstanceCount)
                {
                    // Create a new CodexEntryUI Instance for this codex entry.
                    GameObject codexEntryInstanceGO = Instantiate(_codexEntryPrefab, _codexEntryInstanceContainer);

                    _codexEntryUIInstanceList.Add(codexEntryInstanceGO.GetComponent<CodexEntryUI>());
                    ++codexEntryInstanceCount;
                }

                _codexEntryUIInstanceList[i].SetupCodexEntry(_collectedCodexesList[i]);
            }
        }
    }
}
