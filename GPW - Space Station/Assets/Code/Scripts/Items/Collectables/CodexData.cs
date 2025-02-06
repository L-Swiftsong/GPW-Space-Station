using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Items.Collectables
{
    [CreateAssetMenu(menuName = "Collectables/New Codex Data")]
    public class CodexData : CollectableData
    {
        [SerializeField] private string _codexInformation;
        [SerializeField] [TextArea(5, 15)] private string _codexContents;


        public string CodexInformation => _codexInformation;
        public string CodexContents => _codexContents;
    }
}
