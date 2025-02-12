using Items.Collectables;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace UI.Journal
{
    public class CodexEntryUI : MonoBehaviour
    {
        [SerializeField] private TMP_Text _codexNameText;
        [SerializeField] private TMP_Text _codexInformationText;
        [SerializeField] private TMP_Text _codexContentsText;


        public void SetupCodexEntry(CodexData codexData)
        {
            _codexNameText.text = codexData.CollectableName;
            _codexInformationText.text = codexData.CodexInformation;
            _codexContentsText.text = codexData.CodexContents;
        }
    }
}
