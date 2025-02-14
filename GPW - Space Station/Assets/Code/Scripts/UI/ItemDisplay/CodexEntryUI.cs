using UnityEngine;
using TMPro;
using Items.Collectables;

namespace UI.ItemDisplay
{
    public class CodexEntryUI : ItemDisplaySegmentUI
    {
        [SerializeField] private TMP_Text _codexInformationText;
        [SerializeField] private TMP_Text _codexContentsText;


        public override void SetupCollectableEntry(CollectableData collectableData)
        {
            try
            {
                base.SetupCollectableEntry(collectableData);
                SetupCodexEntry(collectableData as CodexData);
            }
            catch (System.InvalidCastException e)
            {
                Debug.LogError("You are trying to supply a CodexEntryUI element with a CollectableData that doesn't inherit from CodexData.\n" +
                    $"Passed Item: {collectableData.name}\n" +
                    e.Message);
            }
        }
        public void SetupCodexEntry(CodexData codexData)
        {
            _codexInformationText.text = codexData.CodexInformation;
            _codexContentsText.text = codexData.CodexContents;
        }
    }
}
