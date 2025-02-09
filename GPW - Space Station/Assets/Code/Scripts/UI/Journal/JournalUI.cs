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


        private void Awake()
        {
            // Destroy accidentally remaining UI elements.
            foreach(Transform existingElement in _codexEntryInstanceContainer)
            {
                Destroy(existingElement.gameObject);
            }

            _codexEntryUIInstanceList = new List<CodexEntryUI>();
        }
        private void OnEnable() => UpdateCollectedData();


        public void UpdateCollectedData()
        {
            // Find current Codex Data.
            List<CodexData> collectedCodexData = CollectableManager.GetCollectablesOfType<CodexData>();
            
            int codexEntryDataCount = collectedCodexData.Count;
            int codexEntryInstanceCount = _codexEntryUIInstanceList.Count;

            // Delete unnecessary CodexEntryUI Instances.
            if (codexEntryInstanceCount > codexEntryDataCount)
            {
                for(int i = codexEntryDataCount; i < codexEntryInstanceCount; ++i)
                {
                    Destroy(_codexEntryUIInstanceList[i].gameObject);
                    _codexEntryUIInstanceList.RemoveAt(i);
                    --codexEntryInstanceCount;
                }
            }

            // Update our CodexEntryUI Instances.
            for (int i = 0; i < collectedCodexData.Count; ++i)
            {
                if (i >= codexEntryInstanceCount)
                {
                    // Create a new CodexEntryUI Instance for this codex entry.
                    GameObject codexEntryInstanceGO = Instantiate(_codexEntryPrefab, _codexEntryInstanceContainer);

                    _codexEntryUIInstanceList.Add(codexEntryInstanceGO.GetComponent<CodexEntryUI>());
                    ++codexEntryInstanceCount;
                }

                _codexEntryUIInstanceList[i].SetupCodexEntry(collectedCodexData[i]);
            }
        }
    }
}
